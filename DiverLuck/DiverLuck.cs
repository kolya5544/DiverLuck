using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Drawing;
using System.Collections;
using System.Security;
using System.Runtime;
using System.Windows;
using System.Net.Sockets;
using System.Reflection;

namespace DiverLuckCore
{
    public class DiverLuck
    {
        public List<Namespace> namespaces = new();

        public enum DiverLevel
        {
            None, Namespace, Class, SMethod, SField, DMethod, DField
        }
        public enum Command
        {
            Increment = '+', Decrement = '-', Right = '>', Left = '<', Push = '^', Pull = 'v', PrevLevel = '(', NextLevel = ')', ExecCmd = '.',
            ObjectSet = '=',
            ValueSet = '~',
            ObjectAdd = '@',
            ObjectRemove = '%',
            CreatePrimitive = '0', // creates a primitive based on current cell value
            CreateList = '1', // creates an empty list of an obj type currently in cell, or an ID if no object
            AccessList = '2', // accesses list (current object) by index (current value), puts the value in stack
            PushList = '3', // pushes the value from stack into current list (current object)
            RemoveList = '4', // removes the value from list (current object) by index (current value)
            CreateInstance = '5', // creates an instance of class by calling constructor of a currently chosen class. Puts the result into current obj
        }

        public List<MemoryCell> memory = new List<MemoryCell>(1024);
        public List<object> stack = new List<object>();
        public int memoryPointer = 0;
        public DiverLevelSetup currentLevel = new DiverLevelSetup();
        public List<string> functions = new List<string>();
        public static List<Type> primitiveTypes = new List<Type>();

        public DebugHelper debug = new DebugHelper();

        public DiverLuck() => Init();

        public void Init()
        {
            // don't forget to resolve dependencies
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            // init memory
            for (int i = 0; i < 1024; i++)
            {
                var z = new MemoryCell() { obj = null, value = 0 };
                memory.Add(z);
            }

            // init primitive types
            primitiveTypes = new List<Type>()
            {
                typeof(bool), typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
                typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(IntPtr),
                typeof(UIntPtr), typeof(char), typeof(double), typeof(float)
            };

            // init namespaces and classes
            //BuildNamespaceField();
            LoadNamespaceField();
        }

        public static void LoadAdditionalAssemblies(ref Assembly[] currentAssemblies)
        {
            var LinqAssembly = currentAssemblies.FirstOrDefault((z) => z.FullName.StartsWith("System.Linq"));
            var assemblyPath = Path.GetDirectoryName(LinqAssembly.Location);

            var allAssemblies = Directory.EnumerateFiles(assemblyPath, "System*.dll").ToList();
            var asmList = currentAssemblies.ToList();

            foreach (var asm in allAssemblies)
            {
                if (asm.Contains("Native")) continue; // only load IL code
                if (asmList.Any((z) => asm.Contains(z.ManifestModule.Name))) continue; // don't load code we've already loaded

                var dll = File.ReadAllBytes(asm);
                Assembly.Load(dll);
            }

            currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            var assembly = ((AppDomain)sender).GetAssemblies().FirstOrDefault((z) => z.FullName == args.Name);
            return assembly;
        }

        public void BuildNamespaceField()
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies();
            LoadAdditionalAssemblies(ref asm);
            asm.ToList().ForEach((z) =>
            {
                try
                {
                    z.ExportedTypes.ToList().ForEach((x) =>
                    {
                        var ns = namespaces.FirstOrDefault(c => c.name == x.Namespace);
                        if (ns is null)
                        {
                            ns = new Namespace() { name = x.Namespace, assemblyName = z.FullName.Split(',').First() };
                            namespaces.Add(ns);
                        }
                        ns.classes.Add(x);
                        List<string> methodNames = new List<string>();
                        x.GetMethods().ToList().ForEach((z) => methodNames.Add(z.ToString()));
                        List<string> propFields = new List<string>();
                        FieldOperator.GetFields(x).ToList().ForEach((z) => propFields.Add(z.GetName()));
                        ns.classDecl.Add(new DeclClass()
                        {
                            name = x.Name,
                            methods = methodNames,
                            propertiesFields = propFields
                        });
                    });
                }
                catch { }
            });
            namespaces = namespaces.Where(c =>
            {
                return !c.name.StartsWith("Microsoft.Extensions") &&
                       !c.name.StartsWith("DiverLuck"); // must exclude these
            }).OrderBy(c => c.name).ToList();
            namespaces.ForEach(c =>
            {
                c.classDecl = c.classDecl.OrderBy(z => z.name).ToList();
                c.classDecl.ForEach(x =>
                {
                    x.methods.OrderBy(v => v);
                    x.propertiesFields.OrderBy(v => v);
                });
            });

            // save the field
            string json = JsonConvert.SerializeObject(namespaces);
            File.WriteAllText("diverluck.dat", json);
        }

        public void LoadNamespaceField()
        {
            // load the field
            namespaces = JsonConvert.DeserializeObject<List<Namespace>>(File.ReadAllText("diverluck.dat"));

            // load all the classes
            var asm = AppDomain.CurrentDomain.GetAssemblies();
            LoadAdditionalAssemblies(ref asm);
            foreach (var ns in namespaces)
            {
                var asmNs = asm.Where((z) => z.FullName.Split(',').First() == ns.name || z.FullName.Split(',').First() == ns.assemblyName); // find the corresponding assemblies

                foreach (var realAsm in asmNs)
                {
                    if (realAsm is null)
                    {
                        continue;
                    }

                    for (int i = 0; i < ns.classDecl.Count; i++)
                    {
                        var cl = ns.classDecl[i];

                        var clAsm = realAsm.ExportedTypes.FirstOrDefault((z) => z.Name == cl.name);

                        if (ns.classes.Count > i && ns.classes[i] == null)
                        {
                            ns.classes[i] = clAsm;
                        }
                        else
                        {
                            ns.classes.Add(clAsm);
                        }
                    }
                }
            }

            namespaces = namespaces.OrderBy(c => c is null ? "" : c.name).ToList();
            namespaces.ForEach(c =>
            {
                c.classDecl = c.classDecl.OrderBy(z => z.name).ToList();
                c.classDecl.ForEach(x =>
                {
                    x.methods.OrderBy(v => v);
                    x.propertiesFields.OrderBy(v => v);
                });
            });
        }

        public void ExecuteCommand(Command command)
        {
            // core commands exec
            // get appropriate method
            string mName = command.ToString().Split('.').LastOrDefault();
            var method = typeof(CommandExecutor).GetMethod(mName);
            method.Invoke(null, new object[] { this });
        }

        public bool ExecuteProgram(string program)
        {
            // todo proper execution
            for (int i = 0; i < program.Length; i++)
            {
                debug.executionPointer = i;
                debug.programCode = program;

                char cmd = program[i];

                // handle complex commands
                if (cmd == '&') // beginning of a function
                {
                    // parse the entire function
                    string function = ParseHelper.ParseFunction(program, i);

                    // add the function to the list of functions
                    functions.Add(function);
                    i += function.Length + 1;
                    continue;
                } else if (cmd == 'b') // break!
                {
                    return false;
                } else if (cmd == '[')
                {
                    // run infinite loop
                    var offset = ParseHelper.RunInfiniteLoop(this, program, i);

                    i += offset + 1;
                    continue;
                } else if (cmd == 'c') // continue!
                {
                    return true;
                } else if (cmd == 'i')
                {
                    // parse and check the entire IF block
                    var block = ParseHelper.ParseIFblock(this, program, i);

                    if (!block.Item2)
                    {
                        i += block.Item3 + 1;
                        continue;
                    }

                    // only run if true
                    bool breakCode = ExecuteProgram(block.Item1);
                    i += block.Item3 + 1;
                    if (!breakCode) return false;
                    continue;
                } else if (cmd == '#')
                {
                    // run user-defined function!
                    var fun = ParseHelper.ParseUserDefinedFunction(this, program, i);

                    bool breakCode = ExecuteProgram(fun.Item1);
                    if (!breakCode) return false;
                    i += fun.Item2 + 1;
                    continue;
                }

                // execute generic command
                Command cmdEnum = (Command)cmd;
                ExecuteCommand(cmdEnum);
            }
            return true;
        }
    }
}
