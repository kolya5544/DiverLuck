using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiverLuckCore.Helpers
{
    public class InvokeOperator
    {
        public static MethodInfo GetMethodForInvoke(DiverLuck diver, Type classType, int declIndex)
        {
            DeclClass declType = null;

            foreach (var ns in diver.namespaces)
            {
                var x = ns.classDecl.FirstOrDefault((c) => c.name == classType.Name && c.methods.Count > declIndex);
                if (x != null)
                {
                    declType = x;
                    break;
                }
            }

            var callMethod = declType.methods[declIndex];
            var methods = classType.GetMethods();
            var realMethod = methods.FirstOrDefault((z) => z.ToString() == callMethod);
            return realMethod;
        }
    }
}
