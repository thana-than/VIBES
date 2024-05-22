using Xunit;

namespace Vibes.Tests
{
    public class VibesTests_VibePool
    {
        #region PoolSetting

        [Theory]
        [InlineData("one", 1)]
        [InlineData("negativeOne", -1)]
        [InlineData("zero", 0)]
        [InlineData("maxValue", float.MaxValue)]
        [InlineData("minValue", float.MinValue)]
        public void Test_PoolConstruction_ReturnsGivenValue(string key, float value)
        {
            Vibe vibeKey = new Vibe(key);

            VibeTable table = new VibeTable();
            table.Add(vibeKey, value);
            VibePool pool = new VibePool(table);

            float return_value = pool.Get(vibeKey);
            Assert.Equal(value, return_value);
        }

        [Theory]
        [InlineData("one", 1)]
        [InlineData("negativeOne", -1)]
        [InlineData("zero", 0)]
        [InlineData("maxValue", float.MaxValue)]
        [InlineData("minValue", float.MinValue)]
        public void Test_PoolAddition_ReturnsGivenValue(string key, float value)
        {
            Vibe vibeKey = new Vibe(key);
            VibeTable table = new VibeTable();
            VibePool pool = new VibePool();

            table.Add(vibeKey, value);
            pool.Add(table);

            float return_value = pool.Get(vibeKey);
            Assert.Equal(value, return_value);
        }

        [Theory]
        [InlineData("one", 1)]
        [InlineData("negativeOne", -1)]
        [InlineData("zero", 0)]
        [InlineData("maxValue", float.MaxValue)]
        [InlineData("minValue", float.MinValue)]
        public void Test_PoolSet_ReturnsGivenValue(string key, float value)
        {
            Vibe vibeKey = new Vibe(key);
            VibeTable table = new VibeTable();
            VibePool pool = new VibePool();

            table.Add(vibeKey, value);
            pool.Set(table);

            float return_value = pool.Get(vibeKey);
            Assert.Equal(value, return_value);
        }

        [Theory]
        [InlineData("one", 1)]
        [InlineData("negativeOne", -1)]
        [InlineData("zero", 0)]
        [InlineData("maxValue", float.MaxValue)]
        [InlineData("minValue", float.MinValue)]
        public void Test_PoolAdditionAfterTableChange_ReturnsGivenValue(string key, float value)
        {
            Vibe vibeKey = new Vibe(key);
            VibeTable table = new VibeTable();
            VibePool pool = new VibePool();

            pool.Add(table);
            table.Add(vibeKey, value); //*Table changes after pool should NOT make a difference

            float return_value = pool.Get(vibeKey);
            Assert.Equal(value, return_value);
        }

        #endregion

        #region PoolZeroReturns

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(float.MinValue)]
        public void Test_PoolConstruction_TableWithStackZeroOrLess_ReturnsZero(float stackLessThanZero)
        {
            if (stackLessThanZero > 0) return;

            const int NON_ZERO_VALUE = 10;
            Vibe key = new Vibe("key");
            VibeTable table = new VibeTable();
            table.Add(key, NON_ZERO_VALUE);

            VibePool pool = new VibePool(new KeyValuePair<IVibeTable, float>(table, stackLessThanZero));

            float returned_value = pool.Get(key);
            Assert.True(returned_value == 0, "Stacks are negative and should be zero.");
            Assert.True(pool.StoredKeys.Count == 0, "Negative stack application should ensure item is removed. Pool keys list should be empty.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(float.MinValue)]
        public void Test_PoolAdd_TableWithStackZeroOrLess_ReturnsZero(float stackLessThanZero)
        {
            if (stackLessThanZero > 0) return;

            Vibe key = new Vibe("key");
            const int NON_ZERO_VALUE = 10;
            VibeTable table = new VibeTable();
            table.Add(key, NON_ZERO_VALUE);
            VibePool pool = new VibePool();

            pool.Add(table, stackLessThanZero);

            float returned_value = pool.Get(key);
            Assert.True(returned_value == 0, "Stacks are negative and should be zero.");
            Assert.True(pool.StoredKeys.Count == 0, "Negative stack application should ensure item is removed. Pool keys list should be empty.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(float.MinValue)]
        public void Test_PoolAddThenSet_TableWithStackZeroOrLess_ReturnsZero(float stackLessThanZero)
        {
            if (stackLessThanZero > 0) return;

            Vibe key = new Vibe("key");
            const int NON_ZERO_VALUE = 10;
            VibeTable table = new VibeTable();
            table.Add(key, NON_ZERO_VALUE);
            VibePool pool = new VibePool();

            const float STACK_ABOVE_ZERO = 10;
            pool.Add(table, STACK_ABOVE_ZERO);
            pool.Set(table, stackLessThanZero);

            float returned_value = pool.Get(key);
            Assert.True(returned_value == 0, "Stacks are negative and should be zero.");
            Assert.True(pool.StoredKeys.Count == 0, "Negative stack application should ensure item is removed. Pool keys list should be empty.");
        }

        [Fact]
        public void Test_PoolAddThenRemove_ReturnsZero()
        {
            Vibe key = new Vibe("key");
            const int NON_ZERO_VALUE = 10;
            VibeTable table = new VibeTable();
            table.Add(key, NON_ZERO_VALUE);
            VibePool pool = new VibePool();

            const float STACK_ABOVE_ZERO = 10;
            pool.Add(table, STACK_ABOVE_ZERO);
            pool.Remove(table);

            float returned_value = pool.Get(key);
            Assert.True(returned_value == 0, "Pool is empty and should be zero.");
            Assert.True(pool.StoredKeys.Count == 0, "Pool is empty and key list should be empty as well.");
        }

        [Fact]
        public void Test_PoolAddThenClear_ReturnsZero()
        {
            Vibe key = new Vibe("key");
            const int NON_ZERO_VALUE = 10;
            VibeTable table = new VibeTable();
            table.Add(key, NON_ZERO_VALUE);
            VibePool pool = new VibePool();

            const float STACK_ABOVE_ZERO = 10;
            pool.Add(table, STACK_ABOVE_ZERO);
            pool.Clear();

            float returned_value = pool.Get(key);
            Assert.True(returned_value == 0, "Pool is empty and should be zero.");
            Assert.True(pool.StoredKeys.Count == 0, "Pool is empty and key list should be empty as well.");
        }

        #endregion

        #region Manipulation

        [Theory]
        [InlineData(1, 2)]
        [InlineData(5, 1)]
        [InlineData(0, 1)]
        [InlineData(float.MinValue, float.MaxValue)]
        public void Test_PoolAddThenSetSameTableAndKey_ReturnSetValue(float startAddValue, float setValue)
        {
            Vibe key = new Vibe("key");
            VibeTable table = new VibeTable();
            VibePool pool = new VibePool(table);

            table.Add(key, startAddValue);
            table.Set(key, setValue);

            float return_value = pool.Get(key);
            //*Returned value should be equal to the two values summed together
            Assert.Equal(setValue, return_value);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(5, 1.5f)]
        [InlineData(0, 1)]
        [InlineData(-2, 1)]
        public void Test_PoolAddTwice_ReturnStacksEquallingSum(float value1, float value2)
        {
            float expected_sum = Math.Max(0, value1) + Math.Max(0, value2); //* Stacks reduced below zero will be removed from the pool, so each value should recognize less than zero as simply zero
            Vibe key = new Vibe("key");
            VibeTable table = new VibeTable();
            table.Add(key, 1); //* Add a whatever key with whatever value

            VibePool pool = new VibePool();
            pool.Add(table, value1);
            pool.Add(table, value2);

            float return_value = pool.GetStacks(table);
            //*Returned value should be equal to the two values summed together
            Assert.Equal(expected_sum, return_value);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(1, 1.5f)]
        [InlineData(10, 1)]
        [InlineData(0, 10)]
        [InlineData(1, 10)]
        [InlineData(1, 10.5f)]
        [InlineData(10, 10)]
        [InlineData(0, -1)]
        [InlineData(1, -1)]
        [InlineData(1, -1.5f)]
        [InlineData(10, -1)]
        public void Test_PoolAddDuplicateTables_ReturnsCountAsStackValue(int duplicateTablesCount, float stackPerTable)
        {
            if (duplicateTablesCount < 0) return;
            float expected_value = Math.Max(0, duplicateTablesCount * stackPerTable);

            VibeTable table = new VibeTable();
            VibePool pool = new VibePool();

            for (int i = 0; i < duplicateTablesCount; i++)
                pool.Add(table, stackPerTable);

            float return_value = pool.GetStacks(table);
            //*Returned stack count should be equal to duplicateTablesCount * stackPerTable
            //*This is because every duplicate table added internally adds 1 stack to the dictionary storage
            Assert.Equal(expected_value, return_value);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        [InlineData(2, 2)]
        [InlineData(1, 2)]
        [InlineData(50, 5)]
        public void Test_PoolAdditionsThenRemovals_CountReturnsDifference_ZeroOrGreater(int additions, int removals)
        {
            int tableLen = Math.Max(additions, removals);
            if (tableLen < 0) return;
            VibePool pool = new VibePool();

            //*Populate tables arrays
            var GENERIC_VIBE_FLOAT_PAIR = new KeyValuePair<Vibe, float>(new Vibe("key"), 1);
            VibeTable[] tables = new VibeTable[tableLen];
            for (int i = 0; i < tableLen; i++) tables[i] = new VibeTable(GENERIC_VIBE_FLOAT_PAIR);

            int difference = Math.Max(0, additions - removals);

            for (int i = 0; i < additions; i++)
                pool.Add(tables[i]); //*Pull from tables array

            for (int i = 0; i < removals; i++)
                pool.Remove(tables[i]); //*Remove keys starting from 0 towards removals int

            Assert.True(pool.StoredKeys.Count == difference, "StoredKeys.Count should be equal to the difference between the additions and removals to the pool.");
        }


        #endregion

        #region Operations


        [Theory]
        [InlineData(1, 2)]
        [InlineData(5, 1)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(-1, 2)]
        [InlineData(float.MinValue, float.MaxValue)]
        public void Test_PoolAddTwoTablesSameKey_ReturnSumValue(float value1, float value2)
        {
            float expected_sum = value1 + value2;
            Vibe key = new Vibe("key");
            VibeTable tableA = new VibeTable();
            VibeTable tableB = new VibeTable();

            VibePool pool = new VibePool(tableA, tableB);
            tableA.Add(key, value1);
            tableB.Add(key, value2);

            float return_value = pool.Get(key);
            //*Returned value should be equal to the two values summed together
            Assert.Equal(expected_sum, return_value);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(1.5f, 3, 4.6f)]
        [InlineData(3, 2.2f, -5)]
        [InlineData(-1, 4, 2)]
        [InlineData(0, 10, 0)]
        public void Test_PoolAddTable_ReturnsLinearCalculationOfValueAndStack(float value, float stack, float scale)
        {
            float expected_value = VibeTable.ScalingAlgorithms.Linear(stack, value, scale); //*Value that will be calculated from table data
            Vibe key = new Vibe("key");
            VibeTable table = new VibeTable();
            VibePool pool = new VibePool();

            table.Set(key, value, VibeTable.ScalingAlgorithms.Operation.linear, scale); //*Set value and scaling
            pool.Add(table, stack); //*Add table

            float return_value = pool.Get(key); //* Pool should request calculation from each table (in this case only 1) then provide the value
            Assert.Equal(expected_value, return_value);
        }

        #endregion

    }
}