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

        public static IQueryable<Assignment> Assignments(this Student student, IContext context)
        {
            var studentId = student.Id;
            return context.Instances<Assignment>().Where(a => a.AssignedToId == studentId).OrderByDescending(a => a.DueBy);
        }

        public static (Assignment, IContext) AssignTask(this Student student, Task task, DateTime dueBy, IContext context)
        {
            var a = new Assignment()
            {
                AssignedToId = student.Id,
                AssignedById = Teachers_Menu.Me(context).Id,
                TaskId = task.Id,
                DueBy = dueBy,
                Marks = 0,
                Status = AssignmentStatus.PendingStart
            };
            return (a, context.WithNew(a));
        }

    }
}
