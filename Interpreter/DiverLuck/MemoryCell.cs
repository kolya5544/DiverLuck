using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiverLuckCore
{
    public class MemoryCell
    {
        public int value = 0;
        public object obj = null;

        public override string ToString()
        {
            return $"{value} - {obj}";
        }
    }
}
