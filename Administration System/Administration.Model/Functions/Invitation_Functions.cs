namespace Model.Functions
{
    public static class Invitation_Functions
    {
        //Send invitation (to one or many)
        //Send reminder (to one or many)
        //Rescind outstanding (one or many)
        //Accept invitation - made by invitee, changes User role and adds to organisation

        public static IContext SendReminder(this Invitation invitation, IContext context) => throw new NotImplementedException();

        public static IContext Cancel(this Invitation invitation, IContext context) => throw new NotImplementedException();

        public static IContext Accept(this Invitation invitation, IContext context)
        {
            var user = Users_Menu.FindByUserName(invitation.ToUserName, context);
            return context.WithUpdated(user, new User(user) {
                OrganisationId = invitation.From.OrganisationId, 
                Role = invitation.AsRole});
        }
   
         
    }
}