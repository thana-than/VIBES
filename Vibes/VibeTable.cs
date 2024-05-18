using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Vibes
{
    [Serializable]
    public class VibeTable : IVibeTable, IGetVibes, ISetVibes, IStoreReadonlyKeys<Vibe>
    {
        readonly Dictionary<Vibe, Data> stored = new Dictionary<Vibe, Data>();
        readonly List<Vibe> keys = new List<Vibe>();
        public ReadOnlyCollection<Vibe> StoredKeys => keys.AsReadOnly();

        public VibeTable(params KeyValuePair<Vibe, float>[] vibes)
        {
            int len = vibes.Length;
            for (int i = 0; i < len; i++)
                Add(vibes[i].Key, vibes[i].Value);
        }

        public void Set(Vibe vibe, float baseValue)
        {
            if (stored.TryGetValue(vibe, out var data))
            {
                data.value = baseValue;
                stored[vibe] = data;
            }
            else
                Set(vibe, baseValue, ScalingAlgorithms.Operation.linear, 1);
        }

        public void Set(Vibe vibe, float baseValue, ScalingAlgorithms.Operation operation, float scale)
        {
            Data data = new Data(baseValue, operation, scale);
            if (!stored.TryAdd(vibe, data))
                stored[vibe] = data;
            else
                keys.Add(vibe);
        }

        public void Add(Vibe vibe, float valueIncrement)
        {
            if (stored.TryGetValue(vibe, out var existing_data))
            {
                existing_data.value += valueIncrement;
                stored[vibe] = existing_data;
            }
            else
            {
                stored.Add(vibe, new Data(valueIncrement));
                keys.Add(vibe);
            }
        }

        public bool Remove(Vibe vibe)
        {
            if (stored.Remove(vibe))
            {
                keys.Remove(vibe);
                return true;
            }

            return false;
        }

        public float Get(Vibe vibe) => Get(vibe, 1);
        public float Get(Vibe vibe, float stack)
        {
            if (stored.TryGetValue(vibe, out var data))
                return data.GetValue(stack);

            return 0;
        }

        public void Clear()
        {
            stored.Clear();
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

            public readonly float GetValue(float stack) => ScalingAlgorithms.Perform(operation, stack, value, scale);
        }

        public static class ScalingAlgorithms
        {
            //* See https://riskofrain2.fandom.com/wiki/Item_Stacking for descriptions of these operations
            //* See https://www.desmos.com/calculator/8yyearx0ja for visualizations of these operations
            public enum Operation { linear, exponential, hyperbolic, }
            static readonly OperationFunc[] operations = new OperationFunc[] { Linear, Exponential, Hyperbolic };

            delegate float OperationFunc(float stack, float value, float scale);
            static float Linear(float stack, float value, float scale) => value + (stack - 1) * (value * scale);
            static float Exponential(float stack, float value, float power) => value + (float)Math.Pow((stack - 1) * value, power);
            static float Hyperbolic(float stack, float value, float scale) => value + (1 - 1 / (1 + value * (stack - 1))) * scale;

            public static float Perform(Operation operation, float stack, float value, float scale) => operations[(int)operation](stack, value, scale);
        }
    }
}