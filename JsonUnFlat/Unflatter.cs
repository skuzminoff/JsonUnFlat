using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace JsonUnFlat
{
    /// <summary>
    /// Transformates flat json into hierarchichal one
    /// </summary>
    public class Unflatter
    {
        /// <summary>
        /// Unflat the specified flat json
        /// </summary>
        /// <returns>Hierarchichal json</returns>
        /// <param name="flat">flat json</param>
        public JToken Unflat(JObject flat)
        {
            var keys = flat.Properties()
                .Where(p => _isObject(p.Name) || _isArray(p.Name))
                .Select(p => (p.Name, p.Value));

            if (!keys.Any())
            {
                return flat;
            }

            var dict = new Dictionary<string[], object>();
            foreach (var k in keys)
            {
                var l = k.Item1.Split('.').ToList();
                dict.Add(l.ToArray(), k.Value);
            }

            var result = new JObject();
            foreach (var kv in dict)
            {
                _handle(kv.Key, kv.Value, ref result);
            }
            return result;
        }

        /// <summary>
        /// Recursively process flat json, element by element, store results in result Jobject
        /// </summary>
        /// <returns>Tuple (last handled path segment, its value as JToken)</returns>
        /// <param name="pathSegments">path to leaf value</param>
        /// <param name="leafValue">leaf value</param>
        /// <param name="result">Result json</param>
        private(string, JToken)_handle(string[] pathSegments, object leafValue, ref JObject result)
        {
            var currentKey = pathSegments[0];
            if (pathSegments.Length > 1)
            {
                var reducedPathSegments = pathSegments.Skip(1).ToArray();
                if (_isArray(currentKey))
                {
                    return _handleArray(currentKey, pathSegments, reducedPathSegments, leafValue, ref result);
                }
                else
                {
                    return _handleObject(currentKey, reducedPathSegments, leafValue, ref result);
                }
            }
            return _handleLeafValue(currentKey, pathSegments, leafValue, ref result);
        }

        private (string, JToken) _handleObject(
            string currentKey,
            string[] reducedPathSegments,
            object leafValue,
            ref JObject result)
        {
            (string innerKey, JToken toAdd) = _handle(reducedPathSegments, leafValue, ref result);
            var newObj = new JObject(new JProperty(innerKey, toAdd));

            //remove utility records
            if (result.ContainsKey(innerKey))
            {
                result.Remove(innerKey);
            }

            var newProp = new JProperty(currentKey, newObj);
            //добавляем  такой объект
            result.Add(newProp);
            return (currentKey, newObj);
        }

        private (string, JToken) _handleArray(
            string currentKey,
            string[] pathSegments,
            string[] reducedPathSegments,
            object leafValue,
            ref JObject result)
        {
            (string arrayKey, int index) = _getIndexRemoveBrackets(currentKey);
            if (result[arrayKey] != null)
            {
                var jarr = result[arrayKey];
                (string innerKey, JToken toAdd) = _handle(reducedPathSegments, leafValue, ref result);

                var jArr = (jarr as JArray);
                if (jArr != null)
                {
                    if (jArr.Count() > index)
                    {
                        var innerObj = jArr[index];
                        innerObj[innerKey] = toAdd;
                    }
                    else
                    {
                        var newObj = new JObject(new JProperty(innerKey, toAdd));
                        jArr.Add(newObj);
                    }
                }
                return _handleLeafValue(currentKey, pathSegments, leafValue, ref result);
            }
            else
            {
                (string innerKey, JToken toAdd) = _handle(reducedPathSegments, leafValue, ref result);
                var newArray = new JArray(new JObject(new JProperty(innerKey, toAdd)));
                result.Add(new JProperty(arrayKey, newArray));

                return (arrayKey, newArray);
            }
        }

        private(string, JToken)_handleLeafValue(
            string currentKey,
            string[] pathSegments,
            object leafValue,
            ref JObject result)
        {
            if (_isArray(currentKey) && pathSegments.Length == 1)
            {
                (string key, int index) = _getIndexRemoveBrackets(currentKey);
                if (result[key] != null)
                {
                    var jarr = result[key];
                    var toAdd = JToken.FromObject(leafValue);

                    var jArr = (jarr as JArray);
                    if (jArr != null)
                    {
                        jArr.Add(toAdd);
                    }
                }
                else
                {
                    var newArray = new JArray(JToken.FromObject(leafValue));
                    var newProp = new JProperty(key, newArray);
                    result.Add(newProp);
                    return (key, newArray);
                }
            }
            return (currentKey, JToken.FromObject(leafValue));
        }

        private (string, int) _getIndexRemoveBrackets(string key)
        {
            string bracketsAndNumber = @"\[[\d]+\]";
            string numbers = @"\d+";
            Regex rgx = new Regex(bracketsAndNumber);
            var numberInBrackets = Regex.Match(key, bracketsAndNumber);
            var number = Regex.Match(numberInBrackets.Value, numbers);
            var cleanPropName = rgx.Replace(key, "");
            return (cleanPropName, Convert.ToInt32(number.Value));
        }

        private bool _isArray(string segment)
        {
            return segment.Contains('[') && segment.Contains(']');
        }

        private bool _isObject(string segment)
        {
            return segment.Contains('.');
        }

    }
}