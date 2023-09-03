using Newtonsoft.Json;
using System.Reflection;

namespace DiverLuckCore
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
        public List<Type> classes = new();
    }

    public class DeclClass
    {
        public string name;
        public List<string> methods = new List<string>();
        public List<string> propertiesFields = new List<string>();
    }
}