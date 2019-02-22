using Newtonsoft.Json.Linq;

namespace JsonUnFlat
{
    /// <summary>
    /// Transformates hierarchichal json into flat one
    /// </summary>
    public class Flatter
    {
        /// <summary>
        /// Flat hierachichal json
        /// </summary>
        /// <param name="jObj">JObject to flat</param>
        /// <returns></returns>
        public JObject Flat(JObject jObj)
        {
            var result = new JObject();
            _flat("", jObj, ref result);
            return result;
        }

        /// <summary>
        /// Recursively flats the specified json, store result in result JObject
        /// </summary>
        /// <param name="path">current path</param>
        /// <param name="token">current path value</param>
        /// <param name="result">result json</param>
        private void _flat(string path, JToken token, ref JObject result)
        {
            foreach (var item in token.Children())
            {
                if (item.HasValues)
                {
                    _flat(path, item, ref result);
                }
                else
                {
                    path += item.Path;
                    result[item.Path] = item;
                }
            }
        }

    }
}