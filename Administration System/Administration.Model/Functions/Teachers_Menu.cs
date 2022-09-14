namespace Model.Functions
{
    [Named("Teacher Actions")]
    public static class Teachers_Menu
    {
        public static Teacher Me(IContext context)
        {
            var userId = Users.Me(context).Id;
            return context.Instances<Teacher>().SingleOrDefault(t => t.UserId == userId);
        }

        public static Organisation MyOrganisation(IContext context) =>
            Me(context).Organisation;

        #region Invitations
        public static IContext InviteStudentToJoin(
            string name,
            string emailAddress,
            IContext context) =>  //TODO also send email
            context.WithNew(new Student()
            {
                Name = name,
                EmailAddress = emailAddress,
                Organisation = Me(context).Organisation,
                Status = MemberStatus.Pending
            });

        public static IContext InviteTeacherToJoin(
            string name,
            string emailAddress,
            IContext context) =>  //TODO also send email
            context.WithNew(new Teacher()
            {
                Name = name,
                EmailAddress = emailAddress,
                Organisation = Me(context).Organisation,
                Status = MemberStatus.Pending
            });


        public static IQueryable<Student> PendingStudentInvitations(IContext context) =>
            context.Instances<Student>().Where(i => i.Status == MemberStatus.Pending);

        public static IQueryable<Teacher> PendingTeacherInvitations(IContext context) =>
            context.Instances<Teacher>().Where(i => i.Status == MemberStatus.Pending);

        public static IContext SendReminderToAllPendingInvitations(IContext context) => throw new NotImplementedException();
        #endregion

        #region Students
        public static IQueryable<Student> OurStudents(IContext context)
        {
            int myOrgId = Me(context).OrganisationId;
            return context.Instances<Student>().Where(s => s.OrganisationId == myOrgId);
        }

        public static Student FindStudent(Student name, IContext context) => name;

        [PageSize(10)]
        public static IQueryable<Student> AutoComplete0FindStudent(string name, IContext context) =>
            OurStudents(context).Where( s => s.Name.ToUpper().Contains(name.ToUpper()));

        #endregion

        #region Colleagues
        public static IQueryable<Teacher> MyColleagues(IContext context)
        {
            var me = Me(context);
            int myOrgId = me.OrganisationId;
            int myId = me.Id;
            return context.Instances<Teacher>().Where(t => t.OrganisationId == myOrgId && t.Id != myId);
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
