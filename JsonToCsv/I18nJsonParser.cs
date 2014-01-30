using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToCsv
{
    public class I18nJsonParser
    {
        public void GenerateDictionary(System.Dynamic.ExpandoObject output, Dictionary<string, object> dict, string parent)
        {
            foreach (var v in output)
            {
                string key = parent + v.Key;
                object o = v.Value;

                if (o.GetType() == typeof(System.Dynamic.ExpandoObject))
                {
                    GenerateDictionary((System.Dynamic.ExpandoObject)o, dict, key + ".");
                }
                else
                {
                    if (!dict.ContainsKey(key))
                    {
                        dict.Add(key, o);
                    }
                }
            }
        }

        public object GenerateJsonObject(Dictionary<string, object> dict, string prefix = "")
        {
            object val;
            if (dict.TryGetValue(prefix, out val))
                return val;
            else
            {
                if (!string.IsNullOrEmpty(prefix))
                    prefix += ".";
                var children = new Dictionary<string, object>();
                foreach (var child in dict.Where(x => x.Key.StartsWith(prefix)).Select(x => x.Key.Substring(prefix.Length).Split(new[] { '.' }, 2)[0]).Distinct())
                {
                    children[child] = GenerateJsonObject(dict, prefix + child);
                }
                return children;
            }
        }
    }
}
