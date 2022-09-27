using NakedFunctions.Security;

namespace Model.Authorization
{
    public class MainMenuAuthorizer : IMainMenuAuthorizer
    {
        private const string qualifier = "Model.Functions.Menus.";
        private string MenuNameOnly(string target) => target.Remove(0, qualifier.Length);

        public bool IsVisible(string target, string memberName, IContext context) =>
            MenuNameOnly(target) switch
            {
                nameof(Users) => UsersAuth(memberName, context),
                nameof(Organisations) => OrganisationsAuth(memberName, context),
                nameof(Groups) => GroupsAuth(memberName, context),
                nameof(Invitations) => InvitationsAuth(memberName, context),
                nameof(Tasks) => TasksAuth(memberName, context),
                nameof(Assignments) =>AssignmentsAuth(memberName, context),
                nameof(Activities) => ActivitiesAuth(memberName, context),
                _ => false
            };

        private bool ActivitiesAuth(string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                _ => false
            };

        private bool AssignmentsAuth(string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                >= Role.Teacher => Helpers.MatchesOneOf(memberName,
                    nameof(Assignments.MyAssignments), 
                    nameof(Assignments.AssignmentsCreatedByMe), 
                    nameof(Assignments.NewAssignmentToIndividual),
                    nameof(Assignments.NewAssignmentToAllInGroup)),
                Role.Student => Helpers.MatchesOneOf(memberName,
                    nameof(Assignments.MyAssignments)),
                _ => false
            };

        private bool TasksAuth(string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                Role.Author => true,
                Role.Teacher => Helpers.MatchesOneOf(memberName,
                    nameof(Tasks.AllAssignableTasks),
                    nameof(Tasks.PublicTasks),
                    nameof(Tasks.FindTasks)),
                Role.Student => Helpers.MatchesOneOf(memberName,
                    nameof(Tasks.PublicTasks)),
                _ => Helpers.MatchesOneOf(memberName, nameof(Tasks.PublicTasks))
            };

        private bool InvitationsAuth(string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                >= Role.Teacher => Helpers.MatchesOneOf(memberName,
                    nameof(Invitations.InviteNewTeacher),
                    nameof(Invitations.InviteNewStudent)),
                Role.Student => false,
                _ => Helpers.MatchesOneOf(memberName, nameof(Invitations.AcceptInvitation))
            };

        private bool GroupsAuth(string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                >= Role.Teacher => Helpers.MatchesOneOf(memberName, 
                    nameof(Groups.AllOurGroups),
                    nameof(Groups.CreateNewGroup)),
                Role.Student => false,
                _ => false
            };

        private bool OrganisationsAuth(string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                >= Role.Teacher => Helpers.MatchesOneOf(memberName, nameof(Organisations.MyOrganisation)),
                Role.Student => false,
                _ => false
            };

        private bool UsersAuth(string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                >= Role.Teacher => Helpers.MatchesOneOf(memberName,
                    nameof(Users.Me),
                    nameof(Users.Students), 
                    nameof(Users.StudentsPendingAcceptance),
                    nameof(Users.FindStudentByLoginId),
                    nameof(Users.FindStudentByName),
                    nameof(Users.MyColleagues)),
                Role.Student => false,
                _ => false
            };


    }
}
