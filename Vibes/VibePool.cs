using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Vibes.Core
{
    [Serializable]
    [JsonConverter(typeof(Json.IVibePoolConverter))]
    public class VibePool : IVibePool
    {
        protected readonly Dictionary<IVibeTable, float> tableData = new Dictionary<IVibeTable, float>();
        protected readonly List<IVibeTable> tableKeys = new List<IVibeTable>();
        public ReadOnlyCollection<IVibeTable> StoredKeys => tableKeys.AsReadOnly();
        public IEnumerable<KeyValuePair<IVibeTable, float>> GetData() => tableData;
        public static bool StacksValid(float stacks) => stacks > 0;
        public int Count => tableData.Count;

        public VibePool() { }
        public VibePool(params IVibeTable[] tables)
        {
            //*Sum all the stacks provided, since duplicate tables may be provided
            //* Since the stack value has not been provided as a parameter, we assume each provided table is 1 stack. 
            foreach (var table in tables)
            {
                if (!TryNew(table, 1))
                    tableData[table]++;
            }
        }

        public VibePool(params KeyValuePair<IVibeTable, float>[] tableStacks)
        {
            //*Sum all the stacks provided, since duplicate tables may be provided
            int len = tableStacks.Length;
            for (int i = 0; i < len; i++)
            {
                var table = tableStacks[i].Key;
                float stack = tableStacks[i].Value;
                if (!TryNew(table, stack))
                    tableData[table] += stack;
            }

            //*Validate to ensure there are no invalid tables after the stacks have been summed
            ValidateTables();
        }

        public VibePool(IEnumerable<KeyValuePair<IVibeTable, float>> tableStacks)
        {
            //*Sum all the stacks provided, since duplicate tables may be provided
            foreach (var item in tableStacks)
            {
                var table = item.Key;
                float stack = item.Value;
                if (!TryNew(table, stack))
                    tableData[table] += stack;
            }

            //*Validate to ensure there are no invalid tables after the stacks have been summed
            ValidateTables();
        }

        void ValidateTables()
        {
            //*Ensure all our table stacks are a valid number (> 0), if they aren't remove!!!
            for (int i = tableKeys.Count - 1; i >= 0; i--)
            {
                var key = tableKeys[i];
                if (!StacksValid(tableData[key]))
                    Remove(key);
            }
        }

        public void Set(IVibeTable table, float stack = 1)
        {
            if (!StacksValid(stack))
            {
                Remove(table);
                return;
            }

            if (!TryNew(table, stack))
                tableData[table] = stack;
        }

        public void Add(IVibeTable table, float stack = 1)
        {
            if (tableData.TryGetValue(table, out float existing_stack))
            {
                existing_stack += stack;
                if (StacksValid(existing_stack))
                    tableData[table] = existing_stack;
                else
                    Remove(table);
            }
            else if (StacksValid(stack))
            {
                tableData.Add(table, stack);
                tableKeys.Add(table);
            }
        }

        bool TryNew(IVibeTable table, float stack)
        {
            if (tableData.ContainsKey(table))
                return false;

            tableData.Add(table, stack);
            tableKeys.Add(table);
            return true;
        }

        public bool Remove(IVibeTable table)
        {
            if (tableData.Remove(table))
            {
                tableKeys.Remove(table);
                return true;
            }

            return false;
        }

        public void Clear()
        {
            tableData.Clear();
            tableKeys.Clear();
        }

        public float Get(IVibeKey vibe)
        {
            float sum = 0;
            int count = tableKeys.Count;
            for (int i = 0; i < count; i++)
            {
                sum += tableKeys[i].Get(vibe, tableData[tableKeys[i]]);
            }
            return sum;
        }

        ///<summary>Collects all keys in all pooled tables and calculates their total value. Expensive!</summary>
        public int GetAllPoolData(ref List<KeyValuePair<IVibeKey, float>> dataOut)
        {
            dataOut.Clear();
            int dataOutCount = 0;

            int poolSize = tableKeys.Count;
            for (int i = 0; i < poolSize; i++)
            {
                IVibeTable table = tableKeys[i];
                float stacks = GetStacks(table);

                var data = table.GetTableData();
                foreach (var keyData in data)
                {
                    IVibeKey key = keyData.Key;
                    float updatedValue = table.Get(key, stacks);
                    int existingIndex = dataOut.FindIndex(p => p.Key.Equals(key));
                    if (existingIndex < 0)
                    {
                        //*Add a new key
                        dataOutCount++;
                        dataOut.Add(new KeyValuePair<IVibeKey, float>(key, updatedValue));
                    }
                    else
                    {
                        //*Add value to existing key
                        var existing = dataOut[existingIndex];
                        dataOut[existingIndex] = new KeyValuePair<IVibeKey, float>(key, existing.Value + updatedValue);
                    }
                }
            }

            return dataOutCount;
        }

        public float GetStacks(IVibeTable tableKey)
        {
            if (tableData.TryGetValue(tableKey, out float stacks))
                return stacks;

            return 0;
        }
    }
}