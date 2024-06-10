using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vibes.Core;

namespace Vibes
{
    public static class Json
    {
        #region Public Serializers
        public static string SerializeKey(IVibeKey key) => JsonConvert.SerializeObject(key);
        public static T DeserializeKey<T>(string json) where T : IVibeKey => JsonConvert.DeserializeObject<T>(json);

        public static string SerializeTable(IVibeTable table) => JsonConvert.SerializeObject(table);
        public static T DeserializeTable<T>(string json) where T : IVibeTable => JsonConvert.DeserializeObject<T>(json);
        #endregion

        #region Fields
        public const string JSON_NAME = "Name";
        public const string JSON_TABLEDATA = "TableData";
        public const string JSON_TABLEDATA_KEY = "Key";
        public const string JSON_TABLEDATA_DATA = "Data";

        private readonly static ConstructorCache key_constructors = new ConstructorCache(new[] { typeof(string) });
        private readonly static ConstructorCache table_constructors = new ConstructorCache(new[] { typeof(IEnumerable<KeyValuePair<IVibeKey, VibeTable.Data>>) });
        #endregion

        #region JsonConverters
        public class IVibeKeyConverter : JsonConverter
        {
            public static string ReadKey(JsonReader reader)
            {
                JObject obj = JObject.Load(reader);
                return ReadKey(obj);
            }

            public static string ReadKey(JObject obj) => obj[JSON_NAME].Value<string>();

            public static void WriteKey(JsonWriter writer, IVibeKey value)
            {
                JObject obj = new JObject { { JSON_NAME, value.Name } };
                obj.WriteTo(writer);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) { return key_constructors.Invoke(objectType, ReadKey(reader)); }
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { WriteKey(writer, (IVibeKey)value); }
            public override bool CanConvert(Type objectType) { return typeof(IVibeKey).IsAssignableFrom(objectType) && key_constructors.Retrieve(objectType) != null; }
        }

        //TODO store and retreive table ID
        public class IVibeTableConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return typeof(IVibeKey).IsAssignableFrom(objectType) && table_constructors.Retrieve(objectType) != null;
            }

            public static IEnumerable<KeyValuePair<IVibeKey, VibeTable.Data>> ReadTableData(JsonReader reader)
            {
                JObject obj = JObject.Load(reader);
                JArray array = (JArray)obj[JSON_TABLEDATA];
                var tableData = array.Select(item =>
                        new KeyValuePair<IVibeKey, VibeTable.Data>(
                            item[JSON_TABLEDATA_KEY].ToObject<VibeKey>(), //TODO specific serialization to original key type
                            item[JSON_TABLEDATA_DATA].ToObject<VibeTable.Data>()
                        )
                    );

                return tableData;
            }

            public static void WriteTable(JsonWriter writer, IVibeTable value)
            {
                var data = value.GetTableData();
                JArray array = new JArray();
                foreach (var kvp in data)
                {
                    JObject kvpObj = new JObject
                    {
                        { JSON_TABLEDATA_KEY, JToken.FromObject(kvp.Key) },
                        { JSON_TABLEDATA_DATA, JToken.FromObject(kvp.Value) }
                    };
                    array.Add(kvpObj);
                }

                JObject obj = new JObject() { { JSON_TABLEDATA, array } };
                obj.WriteTo(writer);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return table_constructors.Invoke(objectType, ReadTableData(reader));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { WriteTable(writer, (IVibeTable)value); }
        }

        #endregion
        #region Helpers

        ///<summary>Allows us to retrieve specific type constructors using reflection, then cache them for performance.</summary>
        internal class ConstructorCache
        {
            public ConstructorCache(params Type[] constructorType) { this.constructorType = constructorType; }
            readonly Dictionary<Type, ConstructorInfo> cache = new Dictionary<Type, ConstructorInfo>();
            readonly Type[] constructorType;
            public ConstructorInfo Retrieve(Type objectType)
            {
                if (!cache.TryGetValue(objectType, out var constructor))
                {
                    constructor = objectType.GetConstructor(constructorType);
                    cache[objectType] = constructor;
                }

                return constructor;
            }

            public object Invoke(Type objectType, params object[] parameters)
            {
                return Retrieve(objectType).Invoke(parameters);
            }
        }

        #endregion
    }
}