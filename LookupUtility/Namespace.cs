using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DiverLuckLookup
{
    public class Namespace
    {
        public string name;
        public string assemblyName;
        public List<DeclClass> classDecl = new List<DeclClass>();

        public override string ToString()
        {
            return name;
        }

        [JsonIgnore]
        public Assembly asm;
        [JsonIgnore]
        public List<Type> classes = new();
    }

    public class DeclClass
    {
        public string name;
        public List<string> methods = new List<string>();
        public List<string> propertiesFields = new List<string>();
    }
}
