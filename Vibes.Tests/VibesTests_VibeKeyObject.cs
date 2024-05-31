using Xunit;

namespace Vibes.Core.Tests
{
    public class VibesTests_VibeKeyObject
    {
        [Fact]
        public void Test_VibeKeyConstructor_ReturnValid()
        {
            VibeKeyObject constructed_vibe = new VibeKeyObject("TestVibe");
            Assert.True(constructed_vibe.IsValid(), "Properly constructed vibe should be valid.");
        }

        [Theory]
        [InlineData("key")]
        [InlineData("check")]
        [InlineData("null")]
        public void Test_NewObjectsSameKey_Equal(string key)
        {
            Assert.Equal(new VibeKeyObject(key), new VibeKeyObject(key));
        }

        [Theory]
        [InlineData("key", "key2")]
        [InlineData("check", "test")]
        [InlineData("null", "zero")]
        public void Test_NewObjectsDifferentKey_Inequal(string key1, string key2)
        {
            Assert.NotEqual(new VibeKeyObject(key1), new VibeKeyObject(key2));
        }
    }
}