using Xunit;

namespace Vibes.Tests
{
    public class VibesTests_Vibe
    {
        [Fact]
        public void Test_VibeDefaultConstructor_ReturnInvalid()
        {
            Vibe default_vibe = new Vibe();
            Assert.False(default_vibe.isValid, "Default constructor vibe should be invalid.");
        }

        [Fact]
        public void Test_VibeKeyConstructor_ReturnValid()
        {
            Vibe constructed_vibe = new Vibe("TestVibe");
            Assert.True(constructed_vibe.isValid, "Properly constructed vibe should be valid.");
        }

        [Theory]
        [InlineData("key")]
        [InlineData("check")]
        [InlineData("null")]
        public void Test_NewStructsSameKey_Equal(string key)
        {
            Assert.Equal(new Vibe(key), new Vibe(key));
        }

        [Theory]
        [InlineData("key", "key2")]
        [InlineData("check", "test")]
        [InlineData("null", "zero")]
        public void Test_NewStructsDifferentKey_Inequal(string key1, string key2)
        {
            Assert.NotEqual(new Vibe(key1), new Vibe(key2));
        }
    }
}