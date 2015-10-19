using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Reflection;

namespace GoEat.Utility
{
    static public class EnumConverter
    {
        static public T? EnumFromDescription<T>(string s) where T : struct
        {
            try
            {
                foreach (T value in Enum.GetValues(typeof(T)))
                {
                    Type type = value.GetType();
                    string name = Enum.GetName(type, value);
                    if (string.IsNullOrEmpty(name) == false)
                    {
                        FieldInfo field = type.GetField(name);
                        if (field != null)
                        {
                            var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                            if (attr != null && string.Compare(attr.Description, s, true) == 0)
                            {
                                return value;
                            }
                        }
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    public static class JsonHelper
    {
        public static T DeserializeObject<T>(string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch
            {
                return default(T);
            }
        }
    }
}
