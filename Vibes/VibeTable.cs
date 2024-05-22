using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Vibes.Core
{
    [Serializable]
    public class VibeTable : IVibeTable, IGetVibes, ISetVibes, IStoreReadonlyKeys<IVibeKey>
    {
        readonly Dictionary<IVibeKey, Data> stored = new Dictionary<IVibeKey, Data>();
        readonly List<IVibeKey> keys = new List<IVibeKey>();
        public ReadOnlyCollection<IVibeKey> StoredKeys => keys.AsReadOnly();

        public VibeTable() { }
        public VibeTable(params KeyValuePair<IVibeKey, float>[] vibes)
        {
            int len = vibes.Length;
            for (int i = 0; i < len; i++)
                Add(vibes[i].Key, vibes[i].Value);
        }

        public VibeTable(params KeyValuePair<string, float>[] vibes)
        {
            int len = vibes.Length;
            for (int i = 0; i < len; i++)
                Add(vibes[i].Key, vibes[i].Value);
        }

        public void Set(IVibeKey vibe, float baseValue)
        {
            if (!vibe.IsValid())
                return;

            if (!TryNew(vibe, new Data(baseValue)))
            {
                Data existing_data = stored[vibe];
                existing_data.value = baseValue;
                stored[vibe] = existing_data;
            }
        }
        public void Set(string vibe, float baseValue) => Set(new VibeKey(vibe), baseValue);

        public void Set(IVibeKey vibe, float baseValue, ScalingAlgorithms.Operation operation, float scale)
        {
            if (!vibe.IsValid())
                return;

            Data data = new Data(baseValue, operation, scale);
            if (!TryNew(vibe, data))
                stored[vibe] = data;
        }
        public void Set(string vibe, float baseValue, ScalingAlgorithms.Operation operation, float scale) => Set(new VibeKey(vibe), baseValue, operation, scale);

        public void Add(IVibeKey vibe, float valueIncrement)
        {
            if (!vibe.IsValid())
                return;

            if (!TryNew(vibe, new Data(valueIncrement)))
            {
                Data existing_data = stored[vibe];
                existing_data.value += valueIncrement;
                stored[vibe] = existing_data;
            }
        }
        public void Add(string vibe, float valueIncrement) => Add(new VibeKey(vibe), valueIncrement);

        bool TryNew(IVibeKey vibe, Data data)
        {
            if (stored.ContainsKey(vibe))
                return false;

            stored.Add(vibe, data);
            keys.Add(vibe);
            return true;
        }

        public bool Remove(VibeKey vibe)
        {
            if (stored.Remove(vibe))
            {
                keys.Remove(vibe);
                return true;
            }

            return false;
        }

        public float Get(IVibeKey vibe) => Get(vibe, 1);
        public float Get(string vibe) => Get(new VibeKey(vibe), 1);
        public float Get(IVibeKey vibe, float stack)
        {
            if (stored.TryGetValue(vibe, out var data))
                return data.GetValue(stack);

            return 0;
        }
        public float Get(string vibe, float stack) => Get(new VibeKey(vibe), stack);

        public void Clear()
        {
            stored.Clear();
            keys.Clear();
        }

        struct Data
        {
            public Data(float value) : this(value, ScalingAlgorithms.Operation.linear, 1) { }
            public Data(float value, ScalingAlgorithms.Operation operation = ScalingAlgorithms.Operation.linear, float scale = 1)
            {
                this.value = value;
                this.scale = scale;
                this.operation = operation;
            }

            public float value;
            public float scale;
            public ScalingAlgorithms.Operation operation;

            public float GetValue(float stack) => ScalingAlgorithms.Perform(operation, stack, value, scale);
        }

        public static class ScalingAlgorithms
        {
            //* See https://riskofrain2.fandom.com/wiki/Item_Stacking for descriptions of these operations
            //* See https://www.desmos.com/calculator/8yyearx0ja for visualizations of these operations
            public enum Operation { linear, exponential, hyperbolic, }
            static readonly OperationFunc[] operations = new OperationFunc[] { Linear, Exponential, Hyperbolic };

            delegate float OperationFunc(float stack, float value, float scale);
            public static float Linear(float stack, float value, float scale) => value + (stack - 1) * (value * scale);
            public static float Exponential(float stack, float value, float power) => value + (float)Math.Pow((stack - 1) * value, power);
            public static float Hyperbolic(float stack, float value, float scale) => value + (1 - 1 / (1 + value * (stack - 1))) * scale;

            public static float Perform(Operation operation, float stack, float value, float scale)
            {
                if (stack <= 0)
                    return 0;

                return operations[(int)operation](stack, value, scale);
            }
        }
    }
}