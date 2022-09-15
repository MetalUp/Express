using Model.Functions.Menus;

namespace Model.Functions
{
    public static class Student_Functions
    {
        #region Edits
        [Edit]
        public static IContext EditFullName(
            this User student,
            [RegEx(@"[A-Za-z]+\s[A-Za-z\w]+")] string name,
            IContext context) =>
            context.WithUpdated(student, new(student) {Name = name });

        [Edit]
        public static IContext EditOrganisation(
            this User user,
            Organisation organisation,
            IContext context) =>
            context.WithUpdated(user, new(user) { Organisation = organisation, OrganisationId = organisation.Id });
        #endregion


        public static IQueryable<Assignment> Assignments(this User student, IContext context)
        {
            var studentId = student.Id;
            return context.Instances<Assignment>().Where(a => a.AssignedToId == studentId).OrderByDescending(a => a.DueBy);
        }

        public static (Assignment, IContext) AssignTask(this User student, Task task, DateTime dueBy, IContext context)
        {
            if (task.Status == TaskStatus.UnderDevelopment) throw new Exception("Tasks under development cannot be assigned - this should have been prevented");
            var a = new Assignment()
            {
                AssignedToId = student.Id,
                AssignedById = Teachers.Me(context).Id,
                TaskId = task.Id,
                DueBy = dueBy,
                Marks = 0,
                Status = AssignmentStatus.PendingStart
            };
            return (a, context.WithNew(a));
        }

    }
}
