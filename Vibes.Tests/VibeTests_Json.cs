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

            string json = Vibes.Json.SerializeKey(preSerializedKey);
            JObject jObject = JObject.Parse(json);

            Assert.True(jObject.Count == 1, "VibeKey Json should only have one property: " + Vibes.Json.JSON_NAME);
            Assert.True(jObject[Vibes.Json.JSON_NAME] != null, "VibeKey Json's property should be named: " + Vibes.Json.JSON_NAME);
            Assert.Equal(key, jObject[Vibes.Json.JSON_NAME].ToString()); //*Name should be the same
        }

        [Theory]
        [InlineData("vibekey")]
        [InlineData("")]
        [InlineData(VibeKey.INVALID_KEY_NAME)]
        public void Test_VibeKeyDeserialization(string key)
        {
            string json = Vibes.Json.SerializeKey(CreateKey(key));
            var vibeKey = Vibes.Json.DeserializeKey<T>(json);

            Assert.Equal(key, vibeKey.Name);
        }
    }

    #endregion

    public class VibeTests_VibeTableJson
    {
        static readonly Random random = new Random();
        static readonly Array operationValues = Enum.GetValues(typeof(VibeTable.ScalingAlgorithms.Operation));
        public static KeyValuePair<IVibeKey, VibeTable.Data>[] GenerateTableData(int tableSize)
        {
            var tableData = new KeyValuePair<IVibeKey, VibeTable.Data>[tableSize];
            for (int i = 0; i < tableSize; i++)
            {
                var key = new VibeKey("key_" + i);
                var value = new VibeTable.Data()
                { value = i, scale = i * 10, operation = (VibeTable.ScalingAlgorithms.Operation)operationValues.GetValue(random.Next(operationValues.Length)) };
                tableData[i] = new(key, value);
            }
            return tableData;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public void Test_VibeTableSerialization(int tableSize)
        {
            var tableData = GenerateTableData(tableSize);
            VibeTable table = new VibeTable(tableData);

            string json = Vibes.Json.SerializeTable(table);
            JObject obj = JObject.Parse(json);
            JArray array = (JArray)obj[Vibes.Json.JSON_TABLEDATA];

            //*Validate data
            ValidateTables(array, tableData);
        }

        internal static void ValidateTables(JArray tableArray, KeyValuePair<IVibeKey, VibeTable.Data>[] tableData)
        {
            int size = tableData.Length;
            Assert.True(tableArray.Count == size, "Table should be the same size as given parameter " + size);
            for (int i = 0; i < size; i++)
            {
                const string KEY = Vibes.Json.JSON_TABLEDATA_KEY;
                Assert.Equal(tableArray[i][KEY][Vibes.Json.JSON_NAME], tableData[i].Key.Name);

                const string VALUE = Vibes.Json.JSON_TABLEDATA_DATA;
                const int DATA_PARAMS = 3;
                Assert.True(tableArray[i][VALUE].Count() == DATA_PARAMS, "Data should only hold " + DATA_PARAMS + " parameters.");
                Assert.Equal(tableArray[i][VALUE][nameof(VibeTable.Data.value)], tableData[i].Value.value);
                Assert.Equal(tableArray[i][VALUE][nameof(VibeTable.Data.scale)], tableData[i].Value.scale);
                Assert.Equal(tableArray[i][VALUE][nameof(VibeTable.Data.operation)], (int)tableData[i].Value.operation);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public void Test_VibeTableDeserialization(int tableSize)
        {
            var tableData = GenerateTableData(tableSize);
            VibeTable table = new VibeTable(tableData);
            string json = Vibes.Json.SerializeTable(table);
            var newTable = Vibes.Json.DeserializeTable<VibeTable>(json);

            Assert.True(newTable.Count == tableSize, "Table should be the same size as given parameter " + tableSize);
            foreach (var originalItem in tableData)
            {
                Assert.Equal(originalItem.Value, newTable.GetData(originalItem.Key));
            }
            //Assert.Equal(table, newTable); //TODO actual equality check between object instances
        }
    }

    public class VibeTests_VibePoolJson
    {
        internal static VibePool GeneratePool(int numberOfTables, int tableSize, int stackSize)
        {
            KeyValuePair<IVibeTable, float>[] data = new KeyValuePair<IVibeTable, float>[numberOfTables];
            for (int i = 0; i < numberOfTables; i++)
            {
                var tableData = VibeTests_VibeTableJson.GenerateTableData(tableSize);
                data[i] = new(new VibeTable(tableData), stackSize);
            }
            return new VibePool(data);
        }

        [Theory]
        [InlineData(0, 1, 1)]
        [InlineData(1, 10, 2)]
        [InlineData(1, 0, 1)]
        [InlineData(10, 1, 1)]
        [InlineData(10, 1, 0)]
        public void Test_VibeTableSerialization(int numberOfTables, int tableSize, int stackSize)
        {
            VibePool pool = GeneratePool(numberOfTables, tableSize, stackSize);
            var data = pool.GetData().ToArray();
            int poolSize = pool.Count;

            string json = Vibes.Json.SerializePool(pool);
            JObject obj = JObject.Parse(json);
            JArray array = (JArray)obj[Vibes.Json.JSON_POOLDATA];

            //*Validate data
            Assert.True(array.Count == poolSize, "Json Pool should be the same size as original. Expected: " + poolSize + ", Actual: " + array.Count);
            for (int i = 0; i < poolSize; i++)
            {
                const string TABLE = Vibes.Json.JSON_POOLDATA_TABLE;
                JArray tableArray = (JArray)array[i][TABLE][Vibes.Json.JSON_TABLEDATA];
                VibeTests_VibeTableJson.ValidateTables(tableArray, data[i].Key.GetTableData().ToArray());

                const string STACKS = Vibes.Json.JSON_POOLDATA_STACKS;
                float stacks = array[i][STACKS].ToObject<float>();
                Assert.Equal(data[i].Value, stacks);
            }
        }

        [Theory]
        [InlineData(0, 1, 1)]
        [InlineData(1, 10, 2)]
        [InlineData(1, 0, 1)]
        [InlineData(10, 1, 1)]
        [InlineData(10, 1, 0)]
        public void Test_VibePoolDeserialization(int numberOfTables, int tableSize, int stackSize)
        {
            VibePool pool = GeneratePool(numberOfTables, tableSize, stackSize);
            var firstPoolData = pool.GetData().ToArray();
            int poolSize = pool.Count;
            string json = Vibes.Json.SerializePool(pool);
            var newPool = Vibes.Json.DeserializePool<VibePool>(json);
            var newPoolData = newPool.GetData().ToArray();

            Assert.True(newPool.Count == poolSize, "Json Pool should be the same size as original. Expected: " + poolSize + ", Actual: " + newPool.Count);
            for (int i = 0; i < poolSize; i++)
            {
                //*Ensure stacks are the same
                float expectedStacks = firstPoolData[i].Value;
                float actualStacks = newPoolData[i].Value;
                Assert.Equal(expectedStacks, actualStacks);

                //*Ensure table data is the same
                var expectedTableData = firstPoolData[i].Key.GetTableData();
                var actualTable = (VibeTable)newPoolData[i].Key;
                foreach (var expectedItem in expectedTableData)
                {
                    Assert.Equal(expectedItem.Value, actualTable.GetData(expectedItem.Key));
                }
            }
            //Assert.Equal(pool, newPool); //TODO actual equality check between object instances
        }
    }
}