using DiverLuckCore.Helpers;
using DiverLuckInterpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DiverLuckCore
{
    public static class CommandExecutor
    {
        public static void Increment(this DiverLuck diver)
        {
            diver.GetCell().value += 1;
        }

        public static void Decrement(this DiverLuck diver)
        {
            diver.GetCell().value -= 1;
        }

        public static void Left(this DiverLuck diver)
        {
            diver.memoryPointer -= 1;
            if (diver.memoryPointer < 0) diver.memoryPointer = diver.memory.Count - 1;
        }

        public static void Right(this DiverLuck diver)
        {
            diver.memoryPointer = (diver.memoryPointer + 1) % 1024;
        }

        public static void Push(this DiverLuck diver)
        {
            var obj = diver.GetCell().obj;
            diver.debug.PushDebug(obj);
            if (diver.currentLevel.lvl == DiverLuck.DiverLevel.SField ||
                diver.currentLevel.lvl == DiverLuck.DiverLevel.DField)
            {
                // we shall push the value of some field or property
                var o = diver.GetCell();
                var fields = FieldOperator.GetFields(o.obj.GetType());

                if (diver.currentLevel.lvl == DiverLuck.DiverLevel.SField) diver.stack.Push(fields[diver.currentLevel.MiscValue].GetValue(null));
                if (diver.currentLevel.lvl == DiverLuck.DiverLevel.DField) diver.stack.Push(fields[diver.currentLevel.MiscValue].GetValue(o.obj));
            }
            else
            {
                diver.stack.Push(obj);
            }
        }

        public static void Pull(this DiverLuck diver)
        {
            var obj = diver.stack.Pull();
            diver.debug.PullDebug(obj);
            if (diver.currentLevel.lvl == DiverLuck.DiverLevel.SField ||
                diver.currentLevel.lvl == DiverLuck.DiverLevel.DField)
            {
                // we shall change the value of some field or property
                var o = diver.GetCell();
                var fields = FieldOperator.GetFields(o.obj.GetType());

                if (diver.currentLevel.lvl == DiverLuck.DiverLevel.SField) fields[diver.currentLevel.MiscValue].SetValue(null, obj);
                if (diver.currentLevel.lvl == DiverLuck.DiverLevel.DField) fields[diver.currentLevel.MiscValue].SetValue(o.obj, obj);
            }
            else
            {
                diver.GetCell().obj = obj;
            }
        }

        public static void PrevLevel(this DiverLuck diver)
        {
            var currLevel = (int)diver.currentLevel.lvl;
            currLevel = Math.Max(currLevel - 1, 0); 
            diver.currentLevel.lvl = (DiverLuck.DiverLevel)currLevel;
            diver.UpdateLevel();
        }

        public static void NextLevel(this DiverLuck diver)
        {
            var lvl = diver.currentLevel;
            var currLevel = (int)lvl.lvl;
            var newLvl = Math.Min(currLevel + 1, Enum.GetValues(typeof(DiverLuck.DiverLevel)).Length - 1);

            lvl.lvl = (DiverLuck.DiverLevel)newLvl;

            // we need to update the vars in accordance
            diver.UpdateLevel();
        }

        public static void UpdateLevel(this DiverLuck diver)
        {
            var lvl = diver.currentLevel;
            switch (lvl.lvl)
            {
                case DiverLuck.DiverLevel.Namespace:
                    lvl.NamespaceValue = diver.GetCell().value; break;
                case DiverLuck.DiverLevel.Class:
                    lvl.ClassValue = diver.GetCell().value; break;
                case DiverLuck.DiverLevel.DField:
                case DiverLuck.DiverLevel.SField:
                case DiverLuck.DiverLevel.DMethod:
                case DiverLuck.DiverLevel.SMethod:
                    lvl.MiscValue = diver.GetCell().value; break;
            }
        }

        public static void CreatePrimitive(this DiverLuck diver)
        {
            var cont = diver.GetCell();
            var primId = cont.value;
            var primType = DiverLuck.primitiveTypes[primId];
            var instance = Activator.CreateInstance(primType);
            cont.obj = instance;
        }

        public static void CreateList(this DiverLuck diver)
        {
            var cont = diver.GetCell();

            var listType = typeof(List<>);

            if (cont.obj is null)
            {
                diver.CreatePrimitive();
            }

            var primType = cont.obj.GetType();
            var constructedListType = listType.MakeGenericType(primType);
            var instance = Activator.CreateInstance(constructedListType);
            cont.obj = instance;
        }

        public static void AccessList(this DiverLuck diver)
        {
            var cont = diver.GetCell();

            var list = cont.obj as List<object>;
            var index = cont.value;

            diver.stack.Push(list[index]);
        }

        public static void PushList(this DiverLuck diver)
        {
            var cont = diver.GetCell();

            var val = diver.stack.Pull();
            var list = cont.obj as List<object>;
            list.Add(val);
        }

        public static void RemoveList(this DiverLuck diver)
        {
            var cont = diver.GetCell();

            var index = cont.value;
            var list = cont.obj as List<object>;
            list.RemoveAt(index);
        }

        public static void CreateInstance(this DiverLuck diver)
        {
            var cont = diver.GetCell();
            var nspace = diver.namespaces[diver.currentLevel.NamespaceValue];
            var diverclass = nspace.classes[diver.currentLevel.ClassValue];

            // find the best matching constructor
            Tuple<ConstructorInfo, int> bestMatch = diver.FindBestMatch(diverclass);
            // grab all N params
            List<object> param = new();
            for (int i = 0; i < bestMatch.Item2; i++)
            {
                param.Add(diver.stack.Pull());
            }
            // invoke
            var newObj = bestMatch.Item1.Invoke(param.ToArray());
            cont.obj = newObj;
        }

        public static void ObjectSet(this DiverLuck diver)
        {
            var cont = diver.GetCell();

            if (cont.obj == null)
            {
                cont.obj = (int)cont.value; return;
            }

            var type = cont.obj.GetType();
            if (type == typeof(bool)) { cont.obj = cont.value != 0; return; }

            if (DiverLuck.primitiveTypes.Contains(type))
            {
                cont.obj = Convert.ChangeType(cont.value, type);
            }
            else
            {
                cont.obj = (int)cont.value;
            }
        }

        public static void ValueSet(this DiverLuck diver)
        {
            var cont = diver.GetCell();
            cont.value = (int)Convert.ChangeType(cont.obj, typeof(int));
        }

        public static void ObjectAdd(this DiverLuck diver)
        {
            var cont = diver.GetCell();
            var type = cont.obj.GetType();
            if (DiverLuck.primitiveTypes.Contains(type))
            {
                cont.obj = Convert.ChangeType(cont.value + (int)Convert.ChangeType(cont.obj, typeof(int)), type);
            }
            else
            {
                cont.obj = (int)Convert.ChangeType(cont.obj, typeof(int)) + cont.value;
            }
        }


        public static void ObjectRemove(this DiverLuck diver)
        {
            var cont = diver.GetCell();
            var type = cont.obj.GetType();
            if (DiverLuck.primitiveTypes.Contains(type))
            {
                cont.obj = Convert.ChangeType(cont.value - (int)Convert.ChangeType(cont.obj, typeof(int)), type);
            }
            else
            {
                cont.obj = (int)Convert.ChangeType(cont.obj, typeof(int)) - cont.value;
            }
        }

        public static void ExecCmd(this DiverLuck diver)
        {
            // probably the most difficult thing now

            if (diver.currentLevel.lvl == DiverLuck.DiverLevel.SMethod)
            {
                // we need to resolve the level of DiverLuck to a namespace, then class, then method
                var nspace = diver.namespaces[diver.currentLevel.NamespaceValue];
                var diverclass = nspace.classes[diver.currentLevel.ClassValue];

                if (diver.currentLevel.NamespaceValue == 0 && diver.currentLevel.ClassValue == 0)
                {
                    // this is a custom user-defined function call!! let's handle it ourselves.

                    var fun = diver.functions[diver.currentLevel.MiscValue];
                    diver.ExecuteProgram(fun);
                }
                else
                {
                    // invoke fun call
                    var methodRaw = InvokeOperator.GetMethodForInvoke(diver, diverclass, diver.currentLevel.MiscValue); //diverclass.GetMethods()[diver.currentLevel.MiscValue];
                    diver.debug.InvokeDebug(methodRaw);

                    // get method parameters
                    var methodParams = PrepareParams(diver, methodRaw);

                    // invoke the method
                    object? ret = null;
                    if (methodRaw.IsGenericMethod)
                    {
                        var generic = methodRaw.MakeGenericMethod((Type)methodParams[0]);
                        methodParams.RemoveAt(0);

                        ret = generic.Invoke(null, methodParams.ToArray());
                    }
                    else
                    {
                        ret = methodRaw.Invoke(null, methodParams.ToArray());
                    }

                    // put the result back into stack if return type is not void
                    if (methodRaw.ReturnType != typeof(void))
                    {
                        diver.stack.Push(ret);
                    }
                }
            } else if (diver.currentLevel.lvl == DiverLuck.DiverLevel.DMethod)
            {
                // we will only need to resolve the method user is trying to access
                var currObj = diver.GetCell().obj;
                var objType = currObj.GetType();
                //var objMethods = objType.GetMethods();
                var chosenMethod = InvokeOperator.GetMethodForInvoke(diver, objType, diver.currentLevel.MiscValue);
                diver.debug.InvokeDebug(chosenMethod);

                // get method parameters
                var methodParams = PrepareParams(diver, chosenMethod);

                // invoke the method
                var ret = chosenMethod.Invoke(currObj, methodParams.ToArray());

                // put the result back into stack if return type is not void
                if (chosenMethod.ReturnType != typeof(void))
                {
                    diver.stack.Push(ret);
                }
            }
        }

        public static List<object> PrepareParams(DiverLuck diver, MethodInfo method)
        {
            var args = method.GetParameters().ToList();
            var methodParams = new List<object>(); // prepare the parameters

            foreach (var param in args)
            {
                methodParams.Add(diver.stack.Pull()); // pull from stack
            }
            methodParams.Reverse(); // reverse

            return methodParams;
        }
    }
}
