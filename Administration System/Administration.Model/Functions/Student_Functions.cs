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
            context.WithUpdated(student, new(student) {RealName = name });

        [Edit]
        public static IContext EditOrganisation(
            this Student user,
            Organisation organisation,
            IContext context) =>
            context.WithUpdated(user, new(user) { Organisation = organisation, OrganisationId = organisation.Id });
        #endregion

           

    }
}
