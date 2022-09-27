namespace Model.Authorization
{
    internal static class Helpers
    {
        internal static bool MatchesOneOf(string memberName, params string[] names) =>
    names.Contains(memberName);

        internal static bool IsProperty<T>(string memberName) => 
            typeof(T).GetProperties().Any(x => x.Name == memberName);

        internal static bool IsAction<T>(string memberName) =>
            typeof(T).GetMethods().Any(x => x.Name == memberName);
    }
}
