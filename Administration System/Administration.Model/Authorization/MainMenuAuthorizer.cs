using Model.Functions.Services;
using NakedFunctions.Security;

namespace Model.Authorization
{
    public class MainMenuAuthorizer : IMainMenuAuthorizer
    {
        private const string qualifier = "Model.Functions.Menus.";
        private string MenuNameOnly(string target) => target.Split('.').Last();

        public bool IsVisible(string target, string memberName, IContext context) =>
            MenuNameOnly(target) switch
            {
                nameof(Users) => UsersAuth(memberName, context),
                nameof(Organisations) => OrganisationsAuth(memberName, context),
                nameof(Groups) => GroupsAuth(memberName, context),
                nameof(Invitations) => InvitationsAuth(memberName, context),
                nameof(Projects) => ProjectsAuth(memberName, context),
                nameof(Languages) => LanguagesAuth(memberName, context),
                nameof(Files) => FilesAuth(memberName, context),
                nameof(Assignments) => AssignmentsAuth(memberName, context),
                nameof(Activities) => ActivitiesAuth(memberName, context),
                nameof(Compile) => CompileAuth(memberName, context),
                nameof(TaskAccess) => TaskAccessAuth(memberName, context),
                nameof(InvitationAcceptance) => InvitationAcceptanceAuth(memberName,context),
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
                    nameof(Assignments.NewAssignmentToGroup)),
                Role.Student => Helpers.MatchesOneOf(memberName,
                    nameof(Assignments.MyAssignments)),
                _ => false
            };

        private bool ProjectsAuth(string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                Role.Author => true,
                Role.Teacher => Helpers.MatchesOneOf(memberName,
                    nameof(Projects.AllAssignableProjects),
                    nameof(Projects.FindProjects)),
                _ => false
            };

        private bool FilesAuth(string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                Role.Author => true,
                _ => false
            };

        private bool InvitationsAuth(string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                >= Role.Teacher => Helpers.MatchesOneOf(memberName,
                    nameof(Invitations.InviteNewTeacher),
                    nameof(Invitations.InviteNewStudent)),
                _ => false
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
                    nameof(Users.OurStudents),
                    nameof(Users.StudentsPendingAcceptance),
                    nameof(Users.FindStudentByName),
                    nameof(Users.MyColleagues)),
                Role.Student => false,
                _ => false
            };

        private bool CompileAuth(string memberName, IContext context) => true;

        private bool TaskAccessAuth(string memberName, IContext context) => true;

        private bool InvitationAcceptanceAuth(string memberName, IContext context) => true;

        private bool LanguagesAuth(string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                _ => false
            };
    }
}
