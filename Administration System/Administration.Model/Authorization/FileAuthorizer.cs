using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class FileAuthorizer : ITypeAuthorizer<File>
    {
        public bool IsVisible(File file, string memberName, IContext context) =>
         Users.UserRole(context) switch
                    {
                        Role.Root => true,
                        Role.Author => IsProperty<File>(memberName)  || UserIsAuthor(file, context),
                        _ => false
                    };

        private bool UserIsAuthor(File file, IContext context) =>
            file.AuthorId == Users.Me(context).Id;
    }
}
