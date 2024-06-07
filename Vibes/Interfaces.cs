using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Vibes.Core;

namespace Vibes
{
    public interface IVibeKey : IEquatable<IVibeKey>
    {
        int Hash { get; }
        string Name { get; }
        bool IsValid();
        int GetHashCode();
        bool Equals(object obj);
    }
    public interface IGetVibes
    {
        float Get(IVibeKey vibe);
    }

    public interface IGetStacks
    {
        float Get(IVibeKey vibe, float stacks);
    }

    public interface IModifyStacks
    {
        void Set(IVibeTable vibe, float stacks);
        void Add(IVibeTable vibe, float stacks);
        bool Remove(IVibeTable vibe);
    }

    public interface IStoreReadonlyKeys<T>
    {
        ReadOnlyCollection<T> StoredKeys { get; }
    }
    public interface ISetVibes
    {
        void Set(IVibeKey vibe, float baseValue);
    }

    public interface IVibeTable : IGetStacks, IEquatable<IVibeTable>
    {
        IEnumerable<KeyValuePair<IVibeKey, VibeTable.Data>> GetTableData();
        int GetHashCode();
        bool Equals(object obj);
    }

    public interface IVibePool : IGetVibes, IStoreReadonlyKeys<IVibeTable>, IModifyStacks
    {
        int GetAllPoolData(ref List<KeyValuePair<IVibeKey, float>> dataOut);
        float GetStacks(IVibeTable tableKey);
    }
}