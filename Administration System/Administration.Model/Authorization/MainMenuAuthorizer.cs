using NakedFunctions.Security;
using Model.Functions.Menus;

namespace Model.Authorization
{
    public class MainMenuAuthorizer : IMainMenuAuthorizer
    {
        private const string qualifier = "Model.Functions.Menus.";
        private string NameOnly(string target) => target.Remove(0, qualifier.Length);

        public bool IsVisible(string target, string memberName, IContext context) =>
            NameOnly(target) switch
            {
                nameof(Students) => Users.UserHasRoleAtLeast(Role.Student, context),
                nameof(Teachers) => Users.UserHasRoleAtLeast(Role.Teacher, context),    
                nameof(Maintenance) => Users.UserHasSpecificRole(Role.Root, context),
                _ => false
            };
    }
}
