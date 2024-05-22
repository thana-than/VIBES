using Xunit;

namespace Vibes.Core.Tests
{
    public class VibesTests_Vibe
    {
        [Fact]
        public void Test_VibeDefaultConstructor_ReturnInvalid()
        {
            VibeKey default_vibe = new VibeKey();
            Assert.False(default_vibe.isValid, "Default constructor vibe should be invalid.");
        }

        [Fact]
        public void Test_VibeKeyConstructor_ReturnValid()
        {
            VibeKey constructed_vibe = new VibeKey("TestVibe");
            Assert.True(constructed_vibe.isValid, "Properly constructed vibe should be valid.");
        }

        [Theory]
        [InlineData("key")]
        [InlineData("check")]
        [InlineData("null")]
        public void Test_NewStructsSameKey_Equal(string key)
        {
            Assert.Equal(new VibeKey(key), new VibeKey(key));
        }

        [Theory]
        [InlineData("key", "key2")]
        [InlineData("check", "test")]
        [InlineData("null", "zero")]
        public void Test_NewStructsDifferentKey_Inequal(string key1, string key2)
        {
            Assert.NotEqual(new VibeKey(key1), new VibeKey(key2));
        }
    }
}