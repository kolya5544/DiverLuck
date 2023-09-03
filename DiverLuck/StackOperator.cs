using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiverLuckCore
{
    public static class StackOperator
    {
        public static object Pull(this List<object> stack)
        {
            var obj = stack.Last();
            stack.RemoveAt(stack.Count - 1);
            return obj;
        }

        public static void Push(this List<object> stack, object obj) => stack.Add(obj);
    }
}
