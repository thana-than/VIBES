using Xunit;

namespace Vibes.Core.Tests
{
    public class VibesTests_VibeTable
    {
        #region TableSetting

        [Theory]
        [InlineData("one", 1)]
        [InlineData("negativeOne", -1)]
        [InlineData("zero", 0)]
        [InlineData("maxValue", float.MaxValue)]
        [InlineData("minValue", float.MinValue)]
        public void Test_TableConstruction_ReturnsGivenValue(string key, float value)
        {
            VibeKey vibeKey = new VibeKey(key);
            var vibeFloatPair = new KeyValuePair<IVibeKey, float>(vibeKey, value);

            VibeTable table = new VibeTable(vibeFloatPair);

            float return_value = table.Get(vibeKey);
            Assert.Equal(value, return_value);
        }

        [Theory]
        [InlineData("one", 1)]
        [InlineData("negativeOne", -1)]
        [InlineData("zero", 0)]
        [InlineData("maxValue", float.MaxValue)]
        [InlineData("minValue", float.MinValue)]
        public void Test_TableAdd_ReturnsGivenValue(string key, float value)
        {
            VibeTable table = new VibeTable();
            VibeKey set_key = new VibeKey(key);

            table.Add(set_key, value);

            float return_value = table.Get(set_key);
            Assert.Equal(value, return_value);
        }

        [Theory]
        [InlineData("one", 1)]
        [InlineData("negativeOne", -1)]
        [InlineData("zero", 0)]
        [InlineData("maxValue", float.MaxValue)]
        [InlineData("minValue", float.MinValue)]
        public void Test_TableSet_ReturnsGivenValue(string key, float value)
        {
            VibeTable table = new VibeTable();
            VibeKey set_key = new VibeKey(key);

            table.Set(set_key, value);

            float return_value = table.Get(set_key);
            Assert.Equal(value, return_value);
        }

        [Theory]
        [InlineData("one", 1)]
        [InlineData("negativeOne", -1)]
        [InlineData("zero", 0)]
        [InlineData("maxValue", float.MaxValue)]
        [InlineData("minValue", float.MinValue)]
        public void Test_TableConstruction_StringEntry_ReturnsValue(string key, float value)
        {
            VibeTable table = new VibeTable(new KeyValuePair<string, float>(key, value));

            float return_value = table.Get(key);
            Assert.Equal(value, return_value);
        }

        [Theory]
        [InlineData("one", 1)]
        [InlineData("negativeOne", -1)]
        [InlineData("zero", 0)]
        [InlineData("maxValue", float.MaxValue)]
        [InlineData("minValue", float.MinValue)]
        public void Test_TableSet_StringEntry_ReturnsValue(string key, float value)
        {
            VibeTable table = new VibeTable();

            table.Set(key, value);

            float return_value = table.Get(key);
            Assert.Equal(value, return_value);
        }

        [Theory]
        [InlineData("one", 1)]
        [InlineData("negativeOne", -1)]
        [InlineData("zero", 0)]
        [InlineData("maxValue", float.MaxValue)]
        [InlineData("minValue", float.MinValue)]
        public void Test_TableAdd_StringEntry_ReturnsValue(string key, float value)
        {
            VibeTable table = new VibeTable();

            table.Add(key, value);

            float return_value = table.Get(key);
            Assert.Equal(value, return_value);
        }

        #endregion

        #region TableZeroReturns

        [Fact]
        public void Test_TableConstruction_InvalidVibe_NonZeroValue_ReturnZero()
        {
            VibeKey invalidVibe = new VibeKey();
            const int NON_ZERO_VALUE = 10;
            var vibeFloatPair = new KeyValuePair<IVibeKey, float>(invalidVibe, NON_ZERO_VALUE);

            VibeTable table = new VibeTable(vibeFloatPair);
            float v = table.Get(invalidVibe);

            Assert.True(v == 0, "Vibe is invalid and should be zero.");
        }

        [Fact]
        public void Test_TableSet_InvalidVibe_NonZeroValue_ReturnZero()
        {
            VibeKey invalidVibe = new VibeKey();
            const int NON_ZERO_VALUE = 10;
            VibeTable table = new VibeTable();

            table.Set(invalidVibe, NON_ZERO_VALUE);

            float v = table.Get(invalidVibe);
            Assert.True(v == 0, "Vibe is invalid and should be zero.");
        }

        [Fact]
        public void Test_TableEmpty_GetVibe_ReturnZero()
        {
            VibeKey vibe = new VibeKey("key");
            VibeTable table = new VibeTable();

            float v = table.Get(vibe);
            Assert.True(v == 0, "Table is empty and returned vibe should be zero.");
        }

        [Fact]
        public void Test_TableConstructThenRemove_ReturnsZero()
        {
            const int NON_ZERO_VALUE = 10;
            VibeKey key = new VibeKey("key");
            var vibeFloatPair = new KeyValuePair<IVibeKey, float>(key, NON_ZERO_VALUE);

            VibeTable table = new VibeTable(vibeFloatPair);

            table.Remove(key);

            float return_value = table.Get(key);
            Assert.True(return_value == 0, "Table had key removed. Is now empty and returned vibe should be zero.");
        }

        [Fact]
        public void Test_TableConstructThenClear_ReturnsZero()
        {
            const int NON_ZERO_VALUE = 10;
            VibeKey key = new VibeKey("key");
            var vibeFloatPair = new KeyValuePair<IVibeKey, float>(key, NON_ZERO_VALUE);

            VibeTable table = new VibeTable(vibeFloatPair);
            table.Clear();

            float return_value = table.Get(key);
            Assert.True(return_value == 0, "Table had keys cleared. Is now empty and returned vibe should be zero.");
        }

        [Fact]
        public void Test_TableConstructThenClear_CountRemainsZero()
        {
            const int NON_ZERO_VALUE = 10;
            VibeKey key = new VibeKey("key");
            var vibeFloatPair = new KeyValuePair<IVibeKey, float>(key, NON_ZERO_VALUE);

            VibeTable table = new VibeTable(vibeFloatPair);
            table.Clear();

            Assert.True(table.StoredKeys.Count == 0, "Table had keys cleared. Is now empty and returned count should be zero.");
        }

        #endregion

        #region TableManipulation

        [Theory]
        [InlineData(1, 2)]
        [InlineData(5, 1)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(-1, 2)]
        [InlineData(float.MinValue, float.MaxValue)]
        public void Test_TableAddThenSet_ReturnSetValue(float startAddValue, float setValue)
        {
            VibeKey key = new VibeKey("key");
            VibeTable table = new VibeTable();

            table.Add(key, startAddValue);
            table.Set(key, setValue);

            float return_value = table.Get(key);
            Assert.Equal(setValue, return_value);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(5, 1)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(-1, 2)]
        [InlineData(float.MinValue, float.MaxValue)]
        public void Test_TableSetThenAdd_ReturnSumOfValues(float startSetValue, float addedValue)
        {
            VibeKey key = new VibeKey("key");
            VibeTable table = new VibeTable();
            float expected_sum = startSetValue + addedValue;

            table.Set(key, startSetValue);
            table.Add(key, addedValue);

            float return_value = table.Get(key);
            Assert.Equal(expected_sum, return_value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Test_TableAdditions_ConfirmKeyListCountMatchesGivenPositiveValue(int keyCount)
        {
            if (keyCount < 0) return;
            VibeTable table = new VibeTable();

            for (int i = 0; i < keyCount; i++)
                table.Add(new VibeKey(i.ToString()), 1); //*Unique keys are simply current index

            Assert.True(table.StoredKeys.Count == keyCount, "StoredKeys should start with given count");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(50)]
        public void Test_TableAdditions_DuplicateKey_ReturnSum(int keyCount)
        {
            if (keyCount <= 0) return;
            VibeKey key = new VibeKey("key");
            VibeTable table = new VibeTable();

            for (int i = 0; i < keyCount; i++)
                table.Add(key, 1);

            Assert.True(table.StoredKeys.Count == 1, "All added keys were duplicate, should only return 1 table addition.");

            Assert.Equal(keyCount, table.Get(key)); //*Every key added should add 1 value instead of adding the same key to storage
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        [InlineData(2, 2)]
        [InlineData(1, 2)]
        [InlineData(50, 5)]
        public void Test_TableAdditionsThenRemovals_CountReturnsDifference_ZeroOrGreater(int additions, int removals)
        {
            int difference = Math.Max(0, additions - removals);
            VibeTable table = new VibeTable();

            for (int i = 0; i < additions; i++)
                table.Add(new VibeKey(i.ToString()), 1); //*Unique keys are simply current index

            for (int i = 0; i < removals; i++)
                table.Remove(new VibeKey(i.ToString())); //*Remove keys starting from 0 towards removals int

            Assert.True(table.StoredKeys.Count == difference, "StoredKeys.Count should be equal to the difference between the additions and removals to the table.");
        }

        #endregion

        #region TableStackOperations

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-1.5f)]
        [InlineData(int.MinValue)]
        public void Test_TableGet_StacksZeroOrLess_ReturnZero(float valueZeroOrLess)
        {
            if (valueZeroOrLess > 0) return;

            VibeTable table = new VibeTable();
            VibeKey vibe = new VibeKey("key");
            const int NON_ZERO_VALUE = 10;

            table.Add(vibe, NON_ZERO_VALUE);

            float returned_value = table.Get(vibe, valueZeroOrLess);
            Assert.True(returned_value == 0, "Attempted to get a vibe with stack less than or equal to 0. Should return 0");
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1.5f, 3)]
        [InlineData(3, 2.2f)]
        [InlineData(-1, 4)]
        [InlineData(0, 10)]
        public void Test_TableGetDefaultOperation_ReturnLinearValue(float value, float stack)
        {
            if (stack <= 0) return;

            VibeTable table = new VibeTable();
            VibeKey key = new VibeKey("key");
            float expected_value = VibeTable.ScalingAlgorithms.Linear(stack, value, 1);

            table.Set(key, value);
            float return_value = table.Get(key, stack);

            Assert.Equal(expected_value, return_value);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1.5f, 3)]
        [InlineData(3, 2.2f)]
        [InlineData(-1, 4)]
        [InlineData(0, 10)]
        public void Test_TableGetDefaultOperation_StringEntry_ReturnLinearValue(float value, float stack)
        {
            if (stack <= 0) return;
            const string KEY_STR = "key";
            VibeTable table = new VibeTable();
            float expected_value = VibeTable.ScalingAlgorithms.Linear(stack, value, 1);

            table.Set(KEY_STR, value);
            float return_value = table.Get(KEY_STR, stack);

            Assert.Equal(expected_value, return_value);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(1.5f, 3, 4.6f)]
        [InlineData(3, 2.2f, -5)]
        [InlineData(-1, 4, 2)]
        [InlineData(0, 10, 0)]
        public void Test_TableGetLinearOperation_ReturnLinearValue(float value, float stack, float scale)
        {
            if (stack <= 0) return;

            VibeTable table = new VibeTable();
            VibeKey key = new VibeKey("key");
            float expected_value = VibeTable.ScalingAlgorithms.Linear(stack, value, scale);

            table.Set(key, value, VibeTable.ScalingAlgorithms.Operation.linear, scale);
            float return_value = table.Get(key, stack);

            Assert.Equal(expected_value, return_value);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(1.5f, 3, 4.6f)]
        [InlineData(3, 2.2f, -5)]
        [InlineData(-1, 4, 2)]
        [InlineData(0, 10, 0)]
        public void Test_TableGetExponentialOperation_ReturnExponentialValue(float value, float stack, float scale)
        {
            if (stack <= 0) return;

            VibeTable table = new VibeTable();
            VibeKey key = new VibeKey("key");
            float expected_value = VibeTable.ScalingAlgorithms.Exponential(stack, value, scale);

            table.Set(key, value, VibeTable.ScalingAlgorithms.Operation.exponential, scale);
            float return_value = table.Get(key, stack);

            Assert.Equal(expected_value, return_value);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(1.5f, 3, 4.6f)]
        [InlineData(3, 2.2f, -5)]
        [InlineData(-1, 4, 2)]
        [InlineData(0, 10, 0)]
        public void Test_TableGetHyperbolicOperation_ReturnHyperbolicValue(float value, float stack, float scale)
        {
            if (stack <= 0) return;

            VibeTable table = new VibeTable();
            VibeKey key = new VibeKey("key");
            float expected_value = VibeTable.ScalingAlgorithms.Hyperbolic(stack, value, scale);

            table.Set(key, value, VibeTable.ScalingAlgorithms.Operation.hyperbolic, scale);
            float return_value = table.Get(key, stack);

            Assert.Equal(expected_value, return_value);
        }
        #endregion
    }
}