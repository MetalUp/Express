using System.Reflection;

namespace Model.Authorization
{
    internal static class Helpers
    {
        internal static bool MemberIsProperty<T>(T target, string memberName) =>
            typeof(T).GetProperties().Any(x => x.IsPublic() && x.Name == memberName);

        internal static bool MemberIsAction<T>(T target, string memberName) =>
            typeof(T).GetMethods().Any(x => x.IsPublic() && x.Name == memberName);
    }
}
