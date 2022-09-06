namespace Model.Functions
{

    [Named("Invitations")]
    public static class Invitations_Menu
    {
        public static IQueryable<Invitation> AllInvitations(IContext context) => context.Instances<Invitation>();

        //View outstanding invitations
    }

}