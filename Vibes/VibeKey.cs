namespace Vibes
{
    /// <summary>
    /// Immutable struct that holds the name and generated hash key of a vibe. Good for fast caching/referencing via code.
    /// </summary>
    public readonly struct VibeKey : IVibeKey
    {
        public VibeKey(string name)
        {
            this.name = name;
            hash = name.GetHashCode();
            isValid = true;
        }
        public string Name => name;
        public int Hash => hash;

        public readonly string name;
        public readonly int hash;
        public readonly bool isValid;

        public bool IsValid() => isValid;

        public static implicit operator int(VibeKey id) => id.hash;

        public bool Equals(IVibeKey other)
        {
            if (ReferenceEquals(other, null)) return false;

            return hash == other.GetHashCode();
        }

        public static bool operator ==(VibeKey v1, IVibeKey v2)
        {
            if (ReferenceEquals(v1, null)) return ReferenceEquals(v2, null);

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
            return hash;
        }
        public override string ToString()
        {
            return name;
        }
    }
}