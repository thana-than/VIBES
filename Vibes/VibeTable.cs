using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Vibes.Core
{
    [Serializable]
    [JsonConverter(typeof(Json.IVibeTableConverter))]
    public class VibeTable : IVibeTable, IGetVibes, ISetVibes
    {
        protected virtual IDictionary<IVibeKey, Data> Storage { get; } = new Dictionary<IVibeKey, Data>();
        public IEnumerable<KeyValuePair<IVibeKey, Data>> GetTableData() => Storage;
        public int Count => Storage.Count;

        public VibeTable() { }
        public VibeTable(params KeyValuePair<IVibeKey, float>[] vibes) : this()
        {
            int len = vibes.Length;
            for (int i = 0; i < len; i++)
                Add(vibes[i].Key, vibes[i].Value);
        }

        public VibeTable(params KeyValuePair<string, float>[] vibes) : this()
        {
            int len = vibes.Length;
            for (int i = 0; i < len; i++)
                Add(vibes[i].Key, vibes[i].Value);
        }

        public VibeTable(params KeyValuePair<IVibeKey, Data>[] vibes) : this()
        {
            foreach (var v in vibes)
                Set(v.Key, v.Value);
        }

        public VibeTable(IEnumerable<KeyValuePair<IVibeKey, Data>> vibes) : this()
        {
            foreach (var v in vibes)
                Set(v.Key, v.Value);
        }

        public VibeTable(IEnumerable<KeyValuePair<IVibeKey, Data>> vibes, Guid guid) : this()
        {
            foreach (var v in vibes)
                Set(v.Key, v.Value);
        }

        public VibeTable(IVibeTable table) : this(table.GetTableData()) { }

        public void Set(IVibeKey vibe, float baseValue)
        {
            if (!vibe.IsValid())
                return;

            SetUnsafe(vibe, baseValue);
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
                SetUnsafe(vibe, data);
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

        public void Add(IVibeKey vibe, float valueIncrement)
        {
            if (!vibe.IsValid())
                return;

            AddUnsafe(vibe, valueIncrement);
        }
        public void Add(string vibe, float valueIncrement) => Add(new VibeKey(vibe), valueIncrement);

        /// <summary>
        /// Sets dictionary value without checking if vibe is valid. Should only be used in very specific cases if you know what you're doing.
        /// </summary>
        protected void SetUnsafe(IVibeKey vibe, Data data)
        {
            Storage.Remove(vibe); //*Remove the previous key in case we need THIS instance of the key (in situations where this key could change)
            Storage[vibe] = data;
        }

        /// <summary>
        /// Sets dictionary value without checking if vibe is valid. Should only be used in very specific cases if you know what you're doing.
        /// </summary>
        void SetUnsafe(IVibeKey vibe, float baseValue)
        {
            if (!TryNew(vibe, new Data(baseValue)))
            {
                Data existing_data = Storage[vibe];
                existing_data.value = baseValue;
                SetUnsafe(vibe, existing_data);
            }
        }

        /// <summary>
        /// Adds dictionary value without checking if vibe is valid. Should only be used in very specific cases if you know what you're doing.
        /// </summary>
        protected void AddUnsafe(IVibeKey vibe, float valueIncrement)
        {
            if (!TryNew(vibe, new Data(valueIncrement)))
            {
                Data existing_data = Storage[vibe];
                existing_data.value += valueIncrement;
                Storage[vibe] = existing_data;
            }
        }

        bool TryNew(IVibeKey vibe, Data data)
        {
            if (ContainsKey(vibe))
                return false;

            Storage.Add(vibe, data);
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
            if (!vibe.IsValid())
                return 0;

            return GetUnsafe(vibe, stack);
        }
        public float Get(string vibe, float stack) => Get(new VibeKey(vibe), stack);

        /// <summary>
        /// Retrieves the value stored here from a vibe key even if the key is invalid.
        /// </summary>
        public float GetUnsafe(IVibeKey vibe, float stack)
        {
            if (Storage.TryGetValue(vibe, out var data))
                return data.GetValue(stack);

            return 0;
        }

        public Data GetData(IVibeKey vibe)
        {
            if (!vibe.IsValid())
                return null;

            return GetDataUnsafe(vibe);
        }
        public Data GetData(string vibe) => GetData(new VibeKey(vibe));

        /// <summary>
        /// Retrieves data stored here from a vibe key even if the key is invalid.
        /// </summary>
        public Data GetDataUnsafe(IVibeKey vibe)
        {
            if (Storage.TryGetValue(vibe, out var data))
                return data;

            return null;
        }

        public void Clear()
        {
            Storage.Clear();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual bool Equals(IVibeTable other)
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
        public class Data : IEquatable<Data>
        {
            public Data() : this(0) { }
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

            public bool Equals(Data other)
            {
                return value == other.value && scale == other.scale && operation == other.operation;
            }
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