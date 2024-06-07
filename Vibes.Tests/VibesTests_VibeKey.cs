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
        public void Test_VibeDefaultConstructor_InvalidName_ReturnInvalid()
        {
            VibeKey invalidKey_vibe = new VibeKey(VibeKey.INVALID_KEY_NAME);
            Assert.False(invalidKey_vibe.isValid, "Vibe constructor with name parameter " + VibeKey.INVALID_KEY_NAME + " should remain invalid.");
        }

        [Fact]
        public void Test_VibeKeyConstructor_ReturnValid()
        {
            VibeKey constructed_vibe = new VibeKey("TestVibe");
            Assert.True(constructed_vibe.isValid, "Properly constructed vibe should be valid.");
        }

        [Fact]
        public void Test_VibeKeyBothInvalid_Equal()
        {
            VibeKey key1 = new VibeKey();
            VibeKey key2 = new VibeKey();

            Assert.True(key1 == key2, "Both keys are invalid and should match.");
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