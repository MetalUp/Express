using Model.Functions;

namespace Model.Functions.Menus
{
    public static class Teachers
    {
        public static User Me(IContext context) => UserRepository.Me(context);

        public static Organisation MyOrganisation(IContext context) =>
            Me(context).Organisation;

        #region Invitations

        public static (Invitation, IContext) AddNewStudent(
            string name,
            [Optionally] Group group,
            IContext context)
        {
            (var student, var invite, var context2) = CreateNewUser(name, Role.Student, context);
            if (group != null)
            {
                var sg = new StudentGroup() { Student = student, Group = group };
                return (invite, context2.WithNew(sg));

            }
            else
            {
                return (invite, context);
            }
        }


        public static (Invitation, IContext) AddNewTeacher(string name, IContext context) {
            (var teacher, var invite, var context2) = CreateNewUser(name, Role.Teacher, context);
            return (invite, context2);
                }

        private static (User, Invitation, IContext) CreateNewUser(
            string name,
            Role asRole,
            IContext context)
        {
            var me = Me(context);
            var user = new User()
            {
                Name = name,
                Role = asRole,
                Status = UserStatus.PendingAcceptance,
                OrganisationId = me.OrganisationId,
                Organisation = me.Organisation
            };
            var invite = new Invitation()
            {
                Id = context.NewGuid(),
                Invitee = user,
                SenderId = me.Id,
                Sender = me,
                Sent = context.Now()
            };
            return (user, invite, context.WithNew(user).WithNew(invite));
        }




        public static IQueryable<User> PendingStudentInvitations(IContext context) =>
            PendingInvitations(context).Where(i => i.Role == Role.Student);

        public static IQueryable<User> PendingTeacherInvitations(IContext context) =>
            PendingInvitations(context).Where(i => i.Role == Role.Teacher);

        private static IQueryable<User> PendingInvitations(IContext context) =>
        context.Instances<User>().Where(i => i.Status == UserStatus.PendingAcceptance);

        public static IContext SendReminderToAllPendingInvitations(IContext context) => throw new NotImplementedException();
        #endregion

        #region Students
        public static IQueryable<User> OurStudents(IContext context)
        {
            int myOrgId = Me(context).OrganisationId;
            return context.Instances<User>().Where(s => s.OrganisationId == myOrgId);
        }

        public static User FindStudent(User name, IContext context) => name;

        [PageSize(10)]
        public static IQueryable<User> AutoComplete0FindStudent(string name, IContext context) =>
            OurStudents(context).Where(s => s.Name.ToUpper().Contains(name.ToUpper()));

        #endregion

        #region Colleagues
        public static IQueryable<User> MyColleagues(IContext context)
        {
            var me = Me(context);
            int myOrgId = me.OrganisationId;
            int myId = me.Id;
            return context.Instances<User>().Where(t => t.OrganisationId == myOrgId && t.Id != myId);
        }

        #endregion

        #region Assigments
        [PageSize(20)]
        public static IQueryable<Assignment> AssignmentsMadeByMe(IContext context)
        {
            var meId = Me(context).Id;
            return context.Instances<Assignment>().Where(s => s.AssignedById == meId).OrderByDescending(a => a.DueBy);
        }

        #endregion
    }
}
