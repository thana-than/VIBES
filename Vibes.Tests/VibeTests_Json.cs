using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Vibes.Core.Tests.Json
{
    #region Keys

    public class VibeTests_VibeKeyJson : BaseIVibeKeyTest<VibeKey> { public override VibeKey CreateKey(string key) => new(key); }
    public class VibeTests_VibeKeyObjectJson : BaseIVibeKeyTest<VibeKeyObject> { public override VibeKeyObject CreateKey(string key) => new(key); }
    public abstract class BaseIVibeKeyTest<T> where T : IVibeKey
    {
        public abstract T CreateKey(string key);

        [Theory]
        [InlineData("vibekey")]
        [InlineData("")]
        [InlineData(VibeKey.INVALID_KEY_NAME)]
        public void Test_VibeKeySerialization(string key)
        {
            var preSerializedKey = CreateKey(key);
            Assert.Equal(key, preSerializedKey.Name); //* name should match string

            string json = JsonConvert.SerializeObject(preSerializedKey);
            JObject jObject = JObject.Parse(json);

            Assert.True(jObject.Count == 1, "VibeKey Json should only have one property: " + Vibes.Json.NAME_KEY);
            Assert.True(jObject[Vibes.Json.NAME_KEY] != null, "VibeKey Json's property should be named: " + Vibes.Json.NAME_KEY);
            Assert.Equal(key, jObject[Vibes.Json.NAME_KEY].ToString()); //*Name should be the same
        }

        [Theory]
        [InlineData("vibekey")]
        [InlineData("")]
        [InlineData(VibeKey.INVALID_KEY_NAME)]
        public void Test_VibeKeyDeserialization(string key)
        {
            string json = JsonConvert.SerializeObject(CreateKey(key));
            var vibeKey = JsonConvert.DeserializeObject<T>(json);

            Assert.Equal(key, vibeKey.Name);
        }
    }

    #endregion
}