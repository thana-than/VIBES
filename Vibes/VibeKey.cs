using Newtonsoft.Json;

namespace Vibes
{
    /// <summary>
    /// Immutable struct that holds the name and generated hash key of a vibe. Good for fast caching/referencing via code.
    /// </summary>
    [JsonConverter(typeof(Json.IVibeKeyConverter))]
    public readonly struct VibeKey : IVibeKey
    {
        public VibeKey(string name)
        {
            Name = name;
            Hash = VibesUtility.NameToHash(name);
            isValid = true;

            if (Hash == INVALID_KEY_STRING_TEST_HASH)
            {
                Hash = INVALID_HASH;
                isValid = false;
            }
        }

        public VibeKey(IVibeKey key)
        {
            Name = key.Name;
            Hash = key.Hash;
            isValid = key.IsValid();
        }

        public const string INVALID_KEY_NAME = "_INVALID_";
        public static readonly int INVALID_HASH = VibesUtility.NameToHash("");
        public static readonly int INVALID_KEY_STRING_TEST_HASH = VibesUtility.NameToHash(INVALID_KEY_NAME);

        public static readonly VibeKey INVALID_KEY = new VibeKey(INVALID_KEY_NAME);

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