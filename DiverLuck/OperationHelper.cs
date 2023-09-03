using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiverLuckCore
{
    public static class OperationHelper
    {
        public static MemoryCell GetCell(this DiverLuck diver)
        {
            return diver.memory[diver.memoryPointer];
        }

        public static Tuple<ConstructorInfo, int> FindBestMatch(this DiverLuck diver, Type classType)
        {
            // find all constructors
            var constructors = classType.GetConstructors().ToList();

            ConstructorInfo bestMatch = null;
            
            // check for perfect match
            for (int i = diver.stack.Count - 1; i >= 0; i--)
            {
                // take the entire stack and compare it against arguments accepted
                var tempStack = diver.stack.GetRange(diver.stack.Count - i - 1, i + 1);

                constructors.ForEach((z) =>
                {
                    var paramsInfo = z.GetParameters();

                    if (paramsInfo.Length != tempStack.Count) return;

                    bool fitsFlag = true;
                    
                    for (int x = 0; x < paramsInfo.Length; x++)
                    {
                        var prm = paramsInfo[x];
                        var st = tempStack[x];

                        if (prm.ParameterType != st.GetType() && !st.GetType().IsCastableTo(prm.ParameterType))
                        {
                            fitsFlag = false; break;
                        }
                    }

                    if (fitsFlag)
                    {
                        bestMatch = z;
                    }
                });

                if (bestMatch != null) return new Tuple<ConstructorInfo, int>(bestMatch, tempStack.Count);
            }
            var emptyConstructor = constructors.FirstOrDefault(z => z.GetParameters().Length == 0);
            if (emptyConstructor is not null) return new Tuple<ConstructorInfo, int>(emptyConstructor, 0);
            return new Tuple<ConstructorInfo, int>(bestMatch, 0);
        }

        public static bool IsCastableTo(this Type from, Type to)
        {
            if (to.IsAssignableFrom(from))
            {
                return true;
            }
            return from.GetMethods(BindingFlags.Public | BindingFlags.Static)
                              .Any(
                                  m => m.ReturnType == to &&
                                       (m.Name == "op_Implicit" ||
                                        m.Name == "op_Explicit")
                              );
        }
    }
}
