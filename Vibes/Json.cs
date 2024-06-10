using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Vibes.Core;

namespace Vibes
{
    public static class Json
    {
        public const string NAME_KEY = "Name";
        public class IVibeKeyConverter : JsonConverter<IVibeKey>
        {
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

            public override IVibeKey ReadJson(JsonReader reader, Type objectType, IVibeKey existingValue, bool hasExistingValue, JsonSerializer serializer) { return new VibeKey(ReadKey(reader)); }
            public override void WriteJson(JsonWriter writer, IVibeKey value, JsonSerializer serializer) { WriteKey(writer, value); }
        }

        public class IVibeKeyObjectConverter : JsonConverter<VibeKeyObject>
        {
            public override VibeKeyObject ReadJson(JsonReader reader, Type objectType, VibeKeyObject existingValue, bool hasExistingValue, JsonSerializer serializer) { return new VibeKeyObject(IVibeKeyConverter.ReadKey(reader)); }
            public override void WriteJson(JsonWriter writer, VibeKeyObject value, JsonSerializer serializer) { IVibeKeyConverter.WriteKey(writer, value); }
        }
    }
}