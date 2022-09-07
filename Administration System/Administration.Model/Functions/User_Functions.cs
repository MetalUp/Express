
namespace Model.Functions
{
    public static class User_Functions
    {

        #region Editing
        [Edit]
        public static IContext EditRole(
            this User user,
            Role role,
            IContext context) =>
            context.WithUpdated(user, new(user) { Role = role });
        #endregion

        internal static bool HasRoleAtLeast(this User user, Role role) => (int) user.Role >= (int)role;

        internal static bool HasRole(this User user, Role role) => user.Role == role;
    }
}
