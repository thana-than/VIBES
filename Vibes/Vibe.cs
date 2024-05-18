namespace Vibes
{
    public readonly struct Vibe : System.IEquatable<Vibe>
    {
        public Vibe(string name)
        {
            this.name = name;
            hash = name.GetHashCode();
        }

        public readonly string name;
        public readonly int hash;

        public static implicit operator int(Vibe id) => id.hash;
        public static implicit operator string(Vibe id) => id.name;

        public readonly bool Equals(Vibe other)
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