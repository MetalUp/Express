using NakedFunctions.Security;

namespace Model.Authorization
{
    public class InvitationAuthorizer : ITypeAuthorizer<Invitation>
    {
        //Cannot view any properties, or Cancel, unless you are the Inviter (creator)

        //Cannot access Accept if you are logged on as an existing user.
        public bool IsVisible(Invitation inv, string memberName, IContext context) =>
            UserRepository.UserRole(context) switch
            {
               >= Role.Teacher => inv.SenderId == UserRepository.Me(context).Id && !IsAcceptAction(memberName),
               Role.Guest => Helpers.MemberIsProperty(inv, memberName) || IsAcceptAction(memberName),
               _ => false
            };

        private bool IsAcceptAction(string memberName) => memberName == nameof(Invitation_Functions.Accept);
    }
}
