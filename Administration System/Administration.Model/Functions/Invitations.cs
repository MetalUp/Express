namespace Model.Functions
{

    [Named("Invitations")]
    public static class Invitation_MenuFunctions
    {
        public static (Invitation, IContext) CreateNewInvitation(IContext context)
        {
            //context.CurrentUser
            throw new NotImplementedException();
            var t = new Invitation {};
            return (t, context.WithNew(t));
        }

        public static IQueryable<Invitation> AllInvitations(IContext context) => context.Instances<Invitation>();

        //View outstanding invitations
    }

    public static class Invitations_Functions
    {
        //Invite Student - made by user in a Teacher role on behalf of their organisation
        //Send reminder (to one or many)
        //Rescind outstanding (one or many)
        //Accept invitation - made by invitee, changes User role and adds to organisation
    }
}