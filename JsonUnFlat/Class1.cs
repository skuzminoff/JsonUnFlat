using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace JsonUnFlat {
    public class JsonFlatter {

        public void Flat (string path, JToken token, ref JObject res) {
            foreach (var item in token.Children ()) {
                if (item.HasValues) {
                    Flat (path, item, ref res);
                } else {
                    path += item.Path;
                    res[item.Path] = item;
                }
            }
        }

        public JToken Unflat (JObject flat) {
            JToken result = new JObject();
            var pattern = @"\.?([^.\[\]]+)|\[(\d+)\]";
            var regex = new Regex (pattern);
            JToken current = null;

            foreach (var p in flat) {
                current = result;
                string prop = "";
                foreach (Match seg in regex.Matches (p.Key)) {
                    //нет смены контекста на индексах
                    if (current[prop] != null)
                    {
                        current = current[prop];
                    }
                    else {
                        JToken r = null;
                        if (!string.IsNullOrEmpty (seg.Groups[2].Value)) {
                            r = new JArray();
                        } else {
                            r = new JObject();
                        }
                        current[prop] = r;
                    }
                    if(!string.IsNullOrEmpty(seg.Groups[2].Value)){
                        prop = seg.Groups[2].Value;
                    }
                    else {
                        prop = seg.Groups[1].Value;
                    }
                }
                current[prop] = flat[p.Key];
                result = current;
            }
            return result;
        }

        /*
    var regex = /\.?([^.\[\]]+)|\[(\d+)\]/g,
    resultholder = {};
  for (var p in data) {
    var cur = resultholder,
      prop = "",
      m;
    while (m = regex.exec(p)) {
      cur = cur[prop] || (cur[prop] = (m[2] ? [] : {}));
      prop = m[2] || m[1];
    }
    cur[prop] = data[p];
  }
     */

        // public JToken Unflat (JObject flat) {
        //     var keys = flat.Properties ().Where (p => p.Name.Contains (".")).Select (p => (p.Name, p.Value));

        //     if (!keys.Any ()) {
        //         return flat;
        //     }

        //     var dict = new Dictionary<string[], object> ();
        //     foreach (var k in keys) {
        //         var l = k.Item1.Split ('.').ToList ();
        //         dict.Add (l.ToArray (), k.Value);
        //     }

        //     var general = _handleDict(dict);

        //     return general;
        // }

        //схема со словарем <path, value> не работает на
        // элементах массива, имеющих больше одной проперти {"prop2[0].prop3":32, "prop2[0].prop4":"asd"}
        //на выходе получаются два элемента массива, вместо одного сдвоенного

        public static JToken _handleDict (Dictionary<string[], object> kvs) {
            JToken result = new JObject ();
            foreach (var kv in kvs) {
                // var hs = _handle (kv.Key, kv.Value);
                // result = hs;

            }

            return result;
        }

        public static JToken _handle (string[] arr, object val) {
            if (arr.Length > 1) {
                var ns = arr.Skip (1).ToArray ();
                if (arr[0].Contains ('[')) {
                    string key = _clean (arr[0]);
                    return new JObject (new JProperty (key, new JArray (_handle (ns, val))));
                } else {
                    return new JObject (new JProperty (arr[0], _handle (ns, val)));
                }
            }
            return new JObject (new JProperty (arr[0], val));
        }

        public static string _clean (string key) {
            string pattern = @"\[[\d]+\]";
            string replacement = "";
            Regex rgx = new Regex (pattern);
            string result = rgx.Replace (key, replacement);
            result = result.Replace ("[", "");
            result = result.Replace ("]", "");
            result = result.Replace ("'", "");
            return result;
        }

        // //схема со словарем <path, value> не работает на
        // // элементах массива, имеющих больше одной проперти {"prop2[0].prop3":32, "prop2[0].prop4":"asd"}
        // //на выходе получаются два элемента массива, вместо одного сдвоенного

        // public static JToken _handleDict (Dictionary<string[], object> kvs) {
        //     var lObj = new List<JToken> ();
        //     foreach (var kv in kvs) {
        //         var hs = _handle (kv.Key, kv.Value);
        //         lObj.Add (hs);
        //     }
        //     JObject general = lObj[0] as JObject;

        //     for (int j = 1; j < lObj.Count; j++) {
        //         general.Merge (lObj[j], new JsonMergeSettings {
        //             MergeArrayHandling = MergeArrayHandling.Union
        //         });
        //     }
        //     return general;
        // }

        // public static string _clean (string key) {
        //     string pattern = @"\[[\d]+\]";
        //     string replacement = "";
        //     Regex rgx = new Regex (pattern);
        //     string result = rgx.Replace (key, replacement);
        //     result = result.Replace ("[", "");
        //     result = result.Replace ("]", "");
        //     result = result.Replace ("'", "");
        //     return result;
        // }

        // public static JToken _handle (string[] arr, object val) {
        //     if (arr.Length > 1) {
        //         var ns = arr.Skip (1).ToArray ();
        //         if (arr[0].Contains ('[')) {
        //             string key = _clean (arr[0]);
        //             return new JObject (new JProperty (key, new JArray (_handle (ns, val))));
        //         } else {
        //             return new JObject (new JProperty (arr[0], _handle (ns, val)));
        //         }
        //     }
        //     return new JObject (new JProperty (arr[0], val));
        // }

    }
}