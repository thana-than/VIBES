using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Vibes
{
    [Serializable]
    public class VibePool : IGetVibes, IStoreReadonlyKeys<IVibeTable>
    {
        readonly Dictionary<IVibeTable, float> tableData = new Dictionary<IVibeTable, float>();
        readonly List<IVibeTable> tableKeys = new List<IVibeTable>();
        public ReadOnlyCollection<IVibeTable> StoredKeys => tableKeys.AsReadOnly();
        bool StacksValid(float stacks) => stacks > 0;

        public VibePool(params IVibeTable[] tables)
        {
            //*Sum all the stacks provided, since duplicate tables may be provided
            //* Since the stack value has not been provided as a parameter, we assume each provided table is 1 stack. 
            foreach (var table in tables)
            {
                if (!tableData.TryAdd(table, 1))
                    tableData[table]++;
                else
                    tableKeys.Add(table);
            }
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

        public VibePool(params KeyValuePair<IVibeTable, float>[] tableStacks)
        {
            //*Sum all the stacks provided, since duplicate tables may be provided
            int len = tableStacks.Length;
            for (int i = 0; i < len; i++)
            {
                var key = tableStacks[i].Key;
                float stacks = tableStacks[i].Value;
                if (!tableData.TryAdd(key, stacks))
                    tableData[key] += stacks;
                else
                    tableKeys.Add(key);
            }

            //*Validate to ensure there are no invalid tables after the stacks have been summed
            ValidateTables();
        }

        public void Set(IVibeTable table, float stack = 1)
        {
            if (!StacksValid(stack))
            {
                Remove(table);
                return;
            }

            if (!tableData.TryAdd(table, stack))
                tableData[table] = stack;
            else
                tableKeys.Add(table);
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

        public float Get(Vibe vibe)
        {
            float sum = 0;
            int count = tableKeys.Count;
            for (int i = 0; i < count; i++)
            {
                sum += tableKeys[i].Get(vibe, tableData[tableKeys[i]]);
            }
            return sum;
        }
    }
}