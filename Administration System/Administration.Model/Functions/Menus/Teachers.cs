﻿using Model.Functions;

namespace Model.Functions.Menus
{
    public static class Teachers
    {
        public static User Me(IContext context) => Users.Me(context);

        public static Organisation MyOrganisation(IContext context) =>
            Me(context).Organisation;

        #region Invitations
        public static IContext InviteStudentToJoin(
            string name,
            string emailAddress,
            IContext context) =>  //TODO also send email
            context.WithNew(new User()
            {
                Name = name,
                EmailAddress = emailAddress,
                Role = Role.Student,
                Organisation = Me(context).Organisation,
                Status = MemberStatus.PendingAcceptance
            });

        public static IContext InviteTeacherToJoin(
            string name,
            string emailAddress,
            IContext context) =>  //TODO also send email
            context.WithNew(new User()
            {
                Name = name,
                EmailAddress = emailAddress,
                Role= Role.Teacher,
                Organisation = Me(context).Organisation,
                Status = MemberStatus.PendingAcceptance
            });


        public static IQueryable<User> PendingStudentInvitations(IContext context) =>
            PendingInvitations(context).Where(i => i.Role == Role.Student);

        public static IQueryable<User> PendingTeacherInvitations(IContext context) =>
            PendingInvitations(context).Where(i => i.Role == Role.Teacher);

        private static IQueryable<User> PendingInvitations(IContext context) =>
    context.Instances<User>().Where(i => i.Status == MemberStatus.PendingAcceptance);

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
