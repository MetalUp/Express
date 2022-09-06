using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Functions
{
    public static class User_Functions
    {

        #region Editing
        [Edit]
        public static IContext EditFullName(
            this User user,
            [RegEx(@"[A-Za-z]+\s[A-Za-z\w]+")] string fullName,
            IContext context) =>
            context.WithUpdated(user, new(user) { FullName = fullName });


        [Edit]
        public static IContext EditPreferredLanguage(
            this User user,
            ProgrammingLanguage preferredLanguage,
            IContext context) =>
            context.WithUpdated(user, new(user) { PreferredLanguage = preferredLanguage });

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
