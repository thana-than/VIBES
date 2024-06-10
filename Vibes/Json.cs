using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Vibes
{
    public static class Json
    {
        public const string NAME_KEY = "Name";
        public class IVibeKeyConverter : JsonConverter
        {
            //* We use reflection to cast our vibekey into the proper type, and cache the constructor so that reflection isn't used over and over again
            private static readonly Dictionary<Type, ConstructorInfo> ConstructorCache = new Dictionary<Type, ConstructorInfo>();
            static readonly Type[] CONSTRUCTOR_TYPE = new[] { typeof(string) };
            static ConstructorInfo RetrieveConstructor(Type objectType)
            {
                if (!ConstructorCache.TryGetValue(objectType, out var constructor))
                {
                    constructor = objectType.GetConstructor(CONSTRUCTOR_TYPE);
                    ConstructorCache[objectType] = constructor;
                }

                return constructor;
            }

            public static string ReadKey(JsonReader reader)
            {
                JObject obj = JObject.Load(reader);
                return obj[NAME_KEY].Value<string>();
            }

            public static void WriteKey(JsonWriter writer, IVibeKey value)
            {
                JObject obj = new JObject { { NAME_KEY, value.Name } };
                obj.WriteTo(writer);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var constructor = RetrieveConstructor(objectType);
                var key = new object[] { ReadKey(reader) };
                object obj = constructor.Invoke(key);

                return obj;
            }
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { WriteKey(writer, (IVibeKey)value); }
            public override bool CanConvert(Type objectType) { return typeof(IVibeKey).IsAssignableFrom(objectType) && RetrieveConstructor(objectType) != null; }
        }
    }
}