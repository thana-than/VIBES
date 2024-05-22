namespace Vibes
{
    public static class VibesUtility
    {
        public static int NameToHash(string name)
        {
            return name.GetHashCode();
        }
    }
}