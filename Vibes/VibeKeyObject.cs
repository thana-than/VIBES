using System;

namespace Vibes.Core
{
    /// <summary>
    /// Mutable class that holds the name and generated hash key of a vibe. Good for dynamic referencing / serialization.
    /// </summary>
    [System.Serializable]
    public class VibeKeyObject : IVibeKey
    {
        public VibeKeyObject(string name)
        {
            Name = name;
        }

        public VibeKeyObject(IVibeKey key)
        {
            name = key.Name;
            hash = key.Hash;
        }

        string name;
        int hash = VibeKey.INVALID_HASH;

        public void RegenerateHash()
        {
            hash = VibesUtility.NameToHash(Name);

            if (Hash == VibeKey.INVALID_KEY_STRING_TEST_HASH)
                hash = VibeKey.INVALID_HASH;
        }

        public string Name { get { return name; } set { name = value; RegenerateHash(); } }
        public int Hash => hash;

        public bool IsValid() => hash != VibeKey.INVALID_HASH;

        public static implicit operator int(VibeKeyObject id) => id.Hash;
        public static implicit operator VibeKey(VibeKeyObject key) => new VibeKey(key);
        public static implicit operator VibeKeyObject(VibeKey key) => new VibeKeyObject(key);

        public bool Equals(IVibeKey other)
        {
            if (ReferenceEquals(other, null)) return false;

            return Hash == other.Hash;
        }

        public static bool operator ==(VibeKeyObject v1, IVibeKey v2)
        {
            if (ReferenceEquals(v1, null)) return ReferenceEquals(v2, null);

            return v1.Equals(v2);
        }

        public static bool operator !=(VibeKeyObject v1, IVibeKey v2)
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