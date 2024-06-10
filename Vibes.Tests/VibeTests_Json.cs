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
        Random random = new Random();
        Array operationValues = Enum.GetValues(typeof(VibeTable.ScalingAlgorithms.Operation));
        KeyValuePair<IVibeKey, VibeTable.Data>[] GenerateTableData(int tableSize)
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
            Assert.True(array.Count == tableSize, "Table should be the same size as given parameter " + tableSize);
            for (int i = 0; i < tableSize; i++)
            {
                const string KEY = Vibes.Json.JSON_TABLEDATA_KEY;
                Assert.Equal(array[i][KEY][Vibes.Json.JSON_NAME], tableData[i].Key.Name);

                const string VALUE = Vibes.Json.JSON_TABLEDATA_DATA;
                const int DATA_PARAMS = 3;
                Assert.True(array[i][VALUE].Count() == DATA_PARAMS, "Data should only hold " + DATA_PARAMS + " parameters.");
                Assert.Equal(array[i][VALUE][nameof(VibeTable.Data.value)], tableData[i].Value.value);
                Assert.Equal(array[i][VALUE][nameof(VibeTable.Data.scale)], tableData[i].Value.scale);
                Assert.Equal(array[i][VALUE][nameof(VibeTable.Data.operation)], (int)tableData[i].Value.operation);
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
}