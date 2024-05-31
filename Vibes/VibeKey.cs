namespace Vibes
{
    /// <summary>
    /// Immutable struct that holds the name and generated hash key of a vibe. Good for fast caching/referencing via code.
    /// </summary>
    public readonly struct VibeKey : IVibeKey
    {
        public VibeKey(string name)
        {
            Name = name;
            Hash = VibesUtility.NameToHash(name);
            isValid = true;
        }

        public VibeKey(IVibeKey key)
        {
            Name = key.Name;
            Hash = key.Hash;
            isValid = key.IsValid();
        }

        public string Name { get; }
        public int Hash { get; }

        public readonly bool isValid;

        public bool IsValid() => isValid;

        public static implicit operator int(VibeKey id) => id.Hash;

        public bool Equals(IVibeKey other)
        {
            return Hash == other.Hash;
        }

        public static bool operator ==(VibeKey v1, IVibeKey v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(VibeKey v1, IVibeKey v2)
        {
            return !(v1 == v2);
        }

        public override bool Equals(object other)
        {
            return Equals(other as IVibeKey);
        }

        public override int GetHashCode()
        {
            return Hash;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}