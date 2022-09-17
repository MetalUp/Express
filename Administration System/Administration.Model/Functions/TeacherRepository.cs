

namespace Model.Functions
{
    public static class TeacherRepository
    {

        public static IContext SendInvitation(
            string name, 
            [DefaultValue((int) Role.Student)] Role role, 
            [Optionally] Group group, 
            IContext context)
        {

            throw new NotImplementedException();
            //Create studentUser in status Pending copying in name, Role, & teacher's organisation
            //Create deferred method to send email to Teacher's email address, as invitation to be forwarded
            // to the specified name.
            //return context.WithNew(studentUser).WithDeferred(sendEmail)
        }

        public static List<Group> Choices2SendInvitation(IContext context) => 
            throw new NotImplementedException();
            // Get MyGroups for user.

    }
}
