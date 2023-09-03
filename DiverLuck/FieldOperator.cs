using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiverLuckCore
{
    public class FieldOperator
    {
        public static List<RealField> GetFields(Type t)
        {
            var fo = new List<RealField>();
            t.GetProperties().ToList().ForEach(z => fo.Add(new RealField() { property = z }));
            t.GetFields().ToList().ForEach(z => fo.Add(new RealField() { field = z }));
            fo.OrderBy((z) => z.GetName());
            return fo;
        }
    }

    public class RealField
    {
        public FieldInfo? field;
        public PropertyInfo? property;

        public void SetValue(object? obj, object? newValue)
        {
            if (field is not null)
            {
                field.SetValue(obj, newValue); return;
            }
            if (property is not null)
            {
                property.SetValue(obj, newValue); return;
            }
        }

        public object? GetValue(object? obj)
        {
            if (field is not null)
            {
                return field.GetValue(obj);
            }
            if (property is not null)
            {
                return property.GetValue(obj);
            }
            return null;
        }

        public string? GetName()
        {
            if (field is not null)
            {
                return field.Name;
            }
            if (property is not null)
            {
                return property.Name;
            }
            return null;
        }
    }
}
