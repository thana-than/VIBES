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
            private static ConstructorCache constructors = new ConstructorCache(new[] { typeof(string) });

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

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) { return constructors.Invoke(objectType, ReadKey(reader)); }
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { WriteKey(writer, (IVibeKey)value); }
            public override bool CanConvert(Type objectType) { return typeof(IVibeKey).IsAssignableFrom(objectType) && constructors.Retrieve(objectType) != null; }
        }

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