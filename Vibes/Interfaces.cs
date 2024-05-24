using System;
using System.Collections.ObjectModel;
using Vibes.Core;

namespace Vibes
{
    public interface IVibeKey : IEquatable<IVibeKey>
    {
        int Hash { get; }
        string Name { get; }
        bool IsValid();
    }
    public interface IGetVibes
    {
        float Get(IVibeKey vibe);
    }
    public interface IStoreReadonlyKeys<T>
    {
        ReadOnlyCollection<T> StoredKeys { get; }
    }
    public interface ISetVibes
    {
        void Set(IVibeKey vibe, float baseValue);
    }
    public interface IVibeTable
    {
        float Get(IVibeKey vibe, float stacks);
    }
}