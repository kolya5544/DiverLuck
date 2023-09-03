using Newtonsoft.Json;
using System.Reflection;

namespace DiverLuckLookup
{
    internal class Program
    {
        public static List<Namespace> namespaces;

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
        public static void LoadNamespaceField()
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

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            LoadNamespaceField();

            while (true)
            {
                Console.Write("Full method or property name:");
                var mname = Console.ReadLine();

                if (string.IsNullOrEmpty(mname)) continue;

                for (int z = 0; z < namespaces.Count; z++)
                {
                    var ns = namespaces[z];

                    for (int x = 0; x < ns.classDecl.Count; x++)
                    {
                        var cl = ns.classDecl[x];

                        for (int c = 0; c < cl.methods.Count; c++)
                        {
                            var mt = cl.methods[c];
                            
                            if (!string.IsNullOrEmpty(mt) && mt.Contains(mname, StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine($"{ns.name}.{cl.name}.{mt} -> namespace #{z}, class #{x}, method #{c}");
                            }
                        }
                    }
                }

                for (int z = 0; z < namespaces.Count; z++)
                {
                    var ns = namespaces[z];

                    for (int x = 0; x < ns.classDecl.Count; x++)
                    {
                        var cl = ns.classDecl[x];

                        for (int c = 0; c < cl.propertiesFields.Count; c++)
                        {
                            var mt = cl.propertiesFields[c];

                            if (!string.IsNullOrEmpty(mt) && mt.Contains(mname, StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine($"{ns.name}.{cl.name}.{mt} -> namespace #{z}, class #{x}, property/field #{c}");
                            }
                        }
                    }
                }
            }
        }
    }
}