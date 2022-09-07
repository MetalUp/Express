using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Functions
{
    [Named("Teacher Actions")]
    public static class Teachers_Menu
    {
        public static Teacher Me(IContext context)
        {
            var userId = Users_Menu.Me(context).Id;
            return context.Instances<Teacher>().Single(t => t.UserId == userId);
        }

        public static Organisation MyOrganisation(IContext context) =>
            Me(context).Organisation;

        #region Invitations
        public static IContext CreateInvitation(
            string toEmailAddress,
            Role asRole,
            [ValueRange(1, 30)][DefaultValue(7)][Named("Valid (no. of days)")] int valid,
            IContext context) =>
            context.WithNew(new Invitation()
            {
                From = Me(context),
                ToEmailAddress = toEmailAddress,
                ToJoin = Me(context).Organisation,
                AsRole = asRole,
                IssuedOn = context.Today(),
                Valid = valid,
                Status = InvitationStatus.Pending
            });

        public static Role Default1InviteUser() => Role.Student;

        public static string Validate1InviteUser(Role asRole) => asRole == Role.Student || asRole == Role.Teacher || asRole == Role.Administrator ? "": "Can only invite as Role: Student, Teacher, or Administrator";

        public static IQueryable<Invitation> MyPendingInvitations(IContext context)
        {
            var myId = Me(context).Id;
            return AllPendingInvitations(context).Where(i => i.FromId == myId);
        }

        public static IQueryable<Invitation> AllPendingInvitations(IContext context) =>
            context.Instances<Invitation>().Where(i => i.Status == InvitationStatus.Pending);


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
            OurStudents(context).Where( s => s.RealName.ToUpper().Contains(name.ToUpper()));

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
    }
}
