using System;
using System.Collections.Generic;

namespace Vibes.Core
{
    [Serializable]
    public class VibeTable : IVibeTable, IGetVibes, ISetVibes
    {
        protected virtual IDictionary<IVibeKey, Data> Storage { get; } = new Dictionary<IVibeKey, Data>();
        public IEnumerable<KeyValuePair<IVibeKey, Data>> GetTableData() => Storage;
        public int Count => Storage.Count;

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
                Data existing_data = Storage[vibe];
                existing_data.value = baseValue;
                Storage[vibe] = existing_data;
            }
        }

        public void Set(IVibeKey vibe, float baseValue, ScalingAlgorithms.Operation operation, float scale) => Set(vibe, new Data(baseValue, operation, scale));
        public void Set(string vibe, float baseValue) => Set(new VibeKey(vibe), baseValue);
        public void Set(string vibe, float baseValue, ScalingAlgorithms.Operation operation, float scale) => Set(new VibeKey(vibe), baseValue, operation, scale);
        public void Set(string vibe, Data data) => Set(new VibeKey(vibe), data);
        public void Set(IVibeKey vibe, Data data)
        {
            if (!vibe.IsValid())
                return;

            if (!TryNew(vibe, data))
                Storage[vibe] = data;
        }
        public void Set(params KeyValuePair<IVibeKey, float>[] vibes)
        {
            int len = vibes.Length;
            for (int i = 0; i < len; i++)
                Set(vibes[i].Key, vibes[i].Value);
        }
        public void Set(params KeyValuePair<string, float>[] vibes)
        {
            int len = vibes.Length;
            for (int i = 0; i < len; i++)
                Set(vibes[i].Key, vibes[i].Value);
        }

        /// <summary>
        /// Sets dictionary value without checking if vibe is valid. Should only be used in very specific cases if you know what you're doing.
        /// </summary>
        protected void SetUnsafe(IVibeKey vibe, Data data)
        {
            Storage[vibe] = data;
        }

        public void Add(IVibeKey vibe, float valueIncrement)
        {
            if (!vibe.IsValid())
                return;

            if (!TryNew(vibe, new Data(valueIncrement)))
            {
                Data existing_data = Storage[vibe];
                existing_data.value += valueIncrement;
                Storage[vibe] = existing_data;
            }
        }
        public void Add(string vibe, float valueIncrement) => Add(new VibeKey(vibe), valueIncrement);

        bool TryNew(IVibeKey vibe, Data data)
        {
            if (ContainsKey(vibe))
                return false;

            Storage.Add(vibe, data);
            // keys.Add(vibe);
            return true;
        }

        public bool ContainsKey(IVibeKey vibe)
        {
            return Storage.ContainsKey(vibe);
        }
        public bool ContainsKey(string key) => ContainsKey(new VibeKey(key));

        public bool Remove(IVibeKey vibe)
        {
            return Storage.Remove(vibe);
        }
        public bool Remove(string vibe) => Remove(new VibeKey(vibe));

        public float Get(IVibeKey vibe) => Get(vibe, 1);
        public float Get(string vibe) => Get(new VibeKey(vibe), 1);
        public float Get(IVibeKey vibe, float stack)
        {
            if (Storage.TryGetValue(vibe, out var data))
                return data.GetValue(stack);

            return 0;
        }
        public float Get(string vibe, float stack) => Get(new VibeKey(vibe), stack);

        public Data GetData(IVibeKey vibe)
        {
            if (Storage.TryGetValue(vibe, out var data))
                return data;

            return null;
        }
        public Data GetData(string vibe) => GetData(new VibeKey(vibe));

        public void Clear()
        {
            Storage.Clear();
        }

        public override int GetHashCode()
        {
            return Storage.GetHashCode();
        }

        public bool Equals(IVibeTable other)
        {
            if (ReferenceEquals(other, null)) return false;

            return GetHashCode() == other.GetHashCode();
        }

        public static bool operator ==(VibeTable v1, IVibeTable v2)
        {
            if (ReferenceEquals(v1, null)) return ReferenceEquals(v2, null);

            return v1.Equals(v2);
        }

        public static bool operator !=(VibeTable p1, IVibeTable p2)
        {
            return !(p1 == p2);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IVibeTable);
        }

        [Serializable]
        public class Data
        {
            public Data(float value) : this(value, ScalingAlgorithms.Operation.linear, 1) { }
            public Data(float value, ScalingAlgorithms.Operation operation = ScalingAlgorithms.Operation.linear, float scale = 1)
            {
                this.value = value;
                this.scale = scale;
                this.operation = operation;
            }

            public float value = 1;
            public float scale = 1;
            public ScalingAlgorithms.Operation operation = ScalingAlgorithms.Operation.linear;

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