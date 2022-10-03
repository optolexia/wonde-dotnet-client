using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace Wonde.Helpers
{
    /// <summary>
    /// Helper class to serialize/deserialize string into object or vice-versa
    /// </summary>
    internal class StringHelper
    {
        /// <summary>
        /// Converts a Json string into Key/Value pair representation of Dictionary object
        /// </summary>
        /// <param name="jsonString">The Json String to format as Object</param>
        /// <returns>Json data decoded as Key/Value pair representation of Dictionary object</returns>
        internal static Dictionary<string, object> getJsonAsDictionary(string jsonString)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString, new JsonConverter[] { new JavaScriptSerializerConverter() });
        }

        /// <summary>
        /// Converts an object to JsonString
        /// </summary>
        /// <param name="arrayObj">The object to be converted with its public properties</param>
        /// <returns>Json formated string</returns>
        internal static string formatObjectAsJson(object arrayObj)
        {
            if (arrayObj == null)
                return "{}";
            return JsonConvert.SerializeObject(arrayObj);
        }

        internal static string buildHttpQueryString(Dictionary<string, string> data, string delimeter = "&")
        {
            string query = "";
            if(data!= null && data.Count  > 0)
            {
                foreach(KeyValuePair<string, string> kv in data)
                {
                    query += kv.Key + "=" + HttpUtility.UrlEncode(kv.Value) + delimeter;
                }
            }

            return query.Substring(0, query.Length - 1);
        }

        // A Converter that mimics the JavaScriptSerializer behaviour
        private class JavaScriptSerializerConverter : CustomCreationConverter<IDictionary<string, object>>
        {
            public override IDictionary<string, object> Create(Type objectType)
            {
                return new Dictionary<string, object>();
            }

            public override bool CanConvert(Type objectType)
            {
                // In addition to handling IDictionary<string, object> we want to handle the deserialization of dict value which is of type object
                return objectType == typeof(object) || base.CanConvert(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                    case JsonToken.Null:
                        return base.ReadJson(reader, objectType, existingValue, serializer);
                    case JsonToken.StartArray:
                        // If it's an array serialize it as a ArrayList of dictionaries
                        return new ArrayList(serializer.Deserialize<List<object>>(reader) ?? throw new InvalidOperationException());
                    default:
                        // If the next token is not an object then fall back on standard deserializer (strings, numbers etc.)
                        return serializer.Deserialize(reader);
                }
            }
        }
    }
}
