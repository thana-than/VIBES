using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Vibes
{
    [Serializable]
    public class VibePool : IGetVibes, IStoreReadonlyKeys<IVibeTable>
    {
        readonly List<IVibeTable> tableKeys = new List<IVibeTable>();
        readonly Dictionary<IVibeTable, float> tableData = new Dictionary<IVibeTable, float>();
        public ReadOnlyCollection<IVibeTable> StoredKeys => throw new NotImplementedException();

        public VibePool(params IVibeTable[] tables)
        {
            foreach (var table in tables)
            {
                if (tableData.TryAdd(table, 1))
                    tableKeys.Add(table);
            }
        }

        public VibePool(params KeyValuePair<IVibeTable, float>[] tableStacks)
        {
            foreach (var table in tableStacks)
            {
                if (table.Value <= 0) //TODO this should not skip and instead we should run a second loop after this one to remove anything zero or less (though do we even need to do that idk)
                    continue;

                var key = table.Key;
                float stacks = table.Value;
                if (tableData.TryAdd(key, stacks))
                    tableKeys.Add(key);
            }
        }

        public void Set(IVibeTable table, float stack = 1)
        {
            if (stack <= 0)
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
            //! Maybe we don't want to remove if stack is less than zero? maybe equations just check first?
            if (tableData.TryGetValue(table, out float existing_stack))
            {
                existing_stack += stack;
                if (existing_stack > 0)
                    tableData[table] = existing_stack;
                else
                    Remove(table);
            }
            else if (stack > 0)
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