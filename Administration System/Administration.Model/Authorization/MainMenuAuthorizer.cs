using NakedFunctions.Security;
using Model.Functions.Menus;

namespace Model.Authorization
{
    public class MainMenuAuthorizer : IMainMenuAuthorizer
    {
        private const string qualifier = "Model.Functions.Menus.";
        private string NameOnly(string target) => target.Remove(0, qualifier.Length);

        public bool IsVisible(string target, string memberName, IContext context) => true;
            //NameOnly(target) switch
            //{
            //    nameof(Students) => UserRepository.UserHasRoleAtLeast(Role.Student, context),
            //    nameof(Teachers) => UserRepository.UserHasRoleAtLeast(Role.Teacher, context),    
            //    nameof(Root) => UserRepository.UserHasSpecificRole(Role.Root, context),
            //    _ => false
            //};
    }
}
