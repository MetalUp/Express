using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Functions
{
    public static class Student_Functions
    {
        #region Edits
        [Edit]
        public static IContext EditFullName(
            this Student student,
            [RegEx(@"[A-Za-z]+\s[A-Za-z\w]+")] string name,
            IContext context) =>
            context.WithUpdated(student, new(student) {Name = name });

        [Edit]
        public static IContext EditOrganisation(
            this Student user,
            Organisation organisation,
            IContext context) =>
            context.WithUpdated(user, new(user) { Organisation = organisation, OrganisationId = organisation.Id });
        #endregion

         public static (Student, IContext) AcceptInvitation(this Student student, User user, IContext context)
        {
            var s = new Student(student) { User = user, UserId = user.Id };
            var u = new User(user) { Role = Role.Student };
            return (s, context.WithNew(s).WithUpdated(user, u));
        }
    }
}
