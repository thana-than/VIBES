using System.Collections.ObjectModel;
using Vibes.Core;

namespace Vibes
{
    public interface IGetVibes
    {
        float Get(Vibe vibe);
    }
    public interface IStoreReadonlyKeys<T>
    {
        ReadOnlyCollection<T> StoredKeys { get; }
    }
    public interface ISetVibes
    {
        void Set(Vibe vibe, float baseValue);
    }
    public interface IVibeTable
    {
        float Get(Vibe vibe, float stacks);
    }
}