using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace i18n.Helper
{
    public class I18NJsonParser
    {
        public void GenerateDictionary(ExpandoObject output, Dictionary<string, object> dict, string parent)
        {
            foreach (var v in output)
            {
                string key = parent + v.Key;
                object o = v.Value;

                if (o is ExpandoObject)
                {
                    GenerateDictionary((ExpandoObject)o, dict, key + ".");
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
            
            if (!string.IsNullOrEmpty(prefix))
                prefix += ".";
            var children = new Dictionary<string, object>();
            foreach (var child in dict.Where(x => x.Key.StartsWith(prefix)).Select(x => prefix != null ? x.Key.Substring(prefix.Length).Split(new[] { '.' }, 2)[0] : null).Distinct())
            {
                children[child] = GenerateJsonObject(dict, prefix + child);
            }
            return children;
        }
    }
}
