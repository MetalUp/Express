using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Functions
{
    public static class User_Functions
    {

        public static IContext ChangePassword(
            this User user,
            [Password] string currentPassword,
            [Password] string newPassword,
            [Password] string confirmNewPassword,
            IContext context) =>
            context.WithUpdated(user, new(user) { Password = newPassword });

        public static string ValidateChangePassword(this User user, string currentPassword, string newPassword, string confirmNewPassword) =>
            currentPassword != user.Password ? "Current Password is incorrect" :
                newPassword == currentPassword ? "New Password is the same as Current Password" :
                    newPassword != confirmNewPassword ? "New Password and Confirm New Password do not match" :
                        "";

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
    }
}
