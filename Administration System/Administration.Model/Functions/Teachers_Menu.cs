using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Functions
{
    public static class Teachers_Menu
    {

        public static IContext InviteUser(
            string userName, 
            Role asRole,
            [ValueRange(1,30)][DefaultValue(7)][Named("Valid (no. of days)")] int valid, 
            IContext context) =>
            context.WithNew(new Invitation()
            {
                From = Users_Menu.Me(context),
                ToUserName = userName,
                ToJoin = Users_Menu.Me(context).Organisation,
                AsRole = Role.Student,
                IssuedOn = context.Today(),
                Valid = valid,
                Status = InvitationStatus.Pending
            });

        public static Role Default1InviteUser() => Role.Student;

        public static string Validate1InviteUser(Role asRole) => asRole == Role.Student || asRole == Role.Teacher ? "": "Can only invite user as Student or Teacher";

        public static bool HideInviteUserToBeStudent(IContext context) => UserIsTeacher(context);


        public static IQueryable<Invitation> ViewPendingInvitations(IContext context)
        {
            int myId = Users_Menu.Me(context).Id;
            return context.Instances<Invitation>().Where(i => i.FromUserId == myId && i.Status == InvitationStatus.Pending);
        }

        public static IContext SendReminderToAllPendingInvitations(IContext context) => throw new NotImplementedException();

        public static IQueryable<User> OurStudents(this User user, IContext context)
        {
            int myOrgId = Users_Menu.Me(context).OrganisationId.Value;
            return context.Instances<User>().Where(u => u.OrganisationId == myOrgId && u.Role == Role.Student);
        }

        public static bool HideOurStudents(IContext context) => UserIsTeacher(context);

        private static bool UserIsTeacher(IContext context) => !Users_Menu.Me(context).HasRoleAtLeast(Role.Teacher);

        public static IQueryable<User> MyColleagues(IContext context)
        {
            User me = Users_Menu.Me(context);
            int myOrgId = me.OrganisationId.Value;
            int myId = me.Id;
            return context.Instances<User>().Where(u => u.OrganisationId == myOrgId && u.Role == Role.Teacher && u.Id != myId);
        }

        public static bool HideMyColleagues(this User user, IContext context) => UserIsTeacher(context);

    }
}
