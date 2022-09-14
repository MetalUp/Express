using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Functions
{
    public static class TeacherRepository
    {

        public static (Teacher, IContext) AcceptInvitation(this Teacher teacher, User user, IContext context)
        {
            var s = new Teacher(teacher) { User = user, UserId = user.Id };
            var u = new User(user) { Role = Role.Teacher };
            return (s, context.WithNew(s).WithUpdated(user, u));
        }

        public static (Teacher, IContext) UpgradeRoleToAdministrator(this Teacher teacher, IContext context)
        {
            var u = new User(teacher.User) { Role = Role.Manager };
            return (teacher, context.WithUpdated(teacher.User, u));
        }
    }
}
