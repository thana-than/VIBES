using System;

namespace Vibes
{
    [Serializable]
    public readonly struct Vibe : IEquatable<Vibe>
    {
        public Vibe(string name)
        {
            this.name = name;
            hash = name.GetHashCode();
            isValid = true;
        }

        public readonly string name;
        public readonly int hash;
        public readonly bool isValid;

        public static implicit operator int(Vibe id) => id.hash;
        public static implicit operator string(Vibe id) => id.name;

        public bool Equals(Vibe other)
        {
            return hash == other.hash;
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