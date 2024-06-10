using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Vibes.Core.Tests
{
    public class VibeTests_Json
    {
        #region VibeKey

        [Theory]
        [InlineData("vibekey")]
        [InlineData("")]
        [InlineData(VibeKey.INVALID_KEY_NAME)]
        public void Test_VibeKeySerialization(string key)
        {
            var preSerializedKey = new VibeKey(key);
            Assert.Equal(key, preSerializedKey.Name); //* name should match string

            string json = JsonConvert.SerializeObject(preSerializedKey);
            JObject jObject = JObject.Parse(json);

            Assert.True(jObject.Count == 1, "VibeKey Json should only have one property: " + Json.NAME_KEY);
            Assert.True(jObject[Json.NAME_KEY] != null, "VibeKey Json's property should be named: " + Json.NAME_KEY);
            Assert.Equal(key, jObject[Json.NAME_KEY].ToString()); //*Name should be the same
        }

        [Theory]
        [InlineData("vibekey")]
        [InlineData("")]
        [InlineData(VibeKey.INVALID_KEY_NAME)]
        public void Test_VibeKeyDeserialization(string key)
        {
            string json = JsonConvert.SerializeObject(new VibeKey(key));
            var vibeKey = JsonConvert.DeserializeObject<VibeKey>(json);

            Assert.Equal(key, vibeKey.Name);
        }

        #endregion

        #region VibeKeyObject

        [Theory]
        [InlineData("vibekey")]
        [InlineData("")]
        [InlineData(VibeKey.INVALID_KEY_NAME)]
        public void Test_VibeKeyObjectSerialization(string key)
        {
            var preSerializedKey = new VibeKeyObject(key);
            Assert.Equal(key, preSerializedKey.Name); //* name should match string

            string json = JsonConvert.SerializeObject(preSerializedKey);
            JObject jObject = JObject.Parse(json);

            Assert.True(jObject.Count == 1, "VibeKeyObject Json should only have one property: " + Json.NAME_KEY);
            Assert.True(jObject[Json.NAME_KEY] != null, "VibeKeyObject Json's property should be named: " + Json.NAME_KEY);
            Assert.Equal(key, jObject[Json.NAME_KEY].ToString()); //*Name should be the same
        }

        [Theory]
        [InlineData("vibekey")]
        [InlineData("")]
        [InlineData(VibeKey.INVALID_KEY_NAME)]
        public void Test_VibeKeyObjectDeserialization(string key)
        {
            string json = JsonConvert.SerializeObject(new VibeKeyObject(key));
            var vibeKey = JsonConvert.DeserializeObject<VibeKeyObject>(json);

            Assert.Equal(key, vibeKey.Name);
        }

        #endregion
    }
}