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

        public readonly string name;
        public readonly int hash;
        public readonly bool isValid;

        public bool IsValid() => isValid;

        public static implicit operator int(VibeKey id) => id.hash;

        public bool Equals(IVibeKey other)
        {
            return hash == other.GetHashCode();
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