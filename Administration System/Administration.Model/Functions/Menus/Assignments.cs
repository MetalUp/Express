namespace Model.Functions.Menus
{
    public static class Assignments
    {
        [PageSize(20)]
        [TableView(false, nameof(Assignment.DueBy), nameof(Assignment.Status), nameof(Assignment.Project))]
        public static IQueryable<Assignment> MyAssignments(IContext context) =>
            AssignmentsTo(Users.Me(context), context);

        internal static IQueryable<Assignment> AssignmentsTo(User user, IContext context)
        {
            var id = user.Id;
            return context.Instances<Assignment>().Where(a => a.AssignedToId == id).OrderByDescending(a => a.DueBy);
        }


        [PageSize(20)]
        public static IQueryable<Assignment> AssignmentsCreatedByMe(IContext context)
        {
            var meId = Users.Me(context).Id;
            return context.Instances<Assignment>().Where(s => s.AssignedById == meId).OrderByDescending(a => a.DueBy);
        }

        public static IQueryable<Assignment> AllAssignments(IContext context) => context.Instances<Assignment>();


        public static IContext NewAssignmentToIndividual(User assignedTo, Project project, [ValueRange(0, 30)] DateTime dueBy, IContext context)
        {
            var me = Users.Me(context);
            var a = new Assignment()
            {
                AssignedToId = assignedTo.Id,
                AssignedTo = assignedTo,
                AssignedById = me.Id,
                AssignedBy = me,
                ProjectId = project.Id,
                Project = project,
                DueBy = dueBy,
                Status = AssignmentStatus.PendingStart
            };
            return context.WithNew(a);
        }

        public static IContext NewAssignmentToGroup(Group group, Project project, [ValueRange(0, 30)] DateTime dueBy, IContext context) =>
            group.Students.Aggregate(context, (c, s) => NewAssignmentToIndividual(s, project, dueBy, c));

        public static List<Group> Choices0NewAssignmentToGroup(Group group, Task task, [ValueRange(0, 30)] DateTime dueBy, IContext context) =>
            Groups.AllOurGroups(context).ToList();

        public static IContext MarkTasksNotCompleted(this IQueryable<Assignment> assignments, string teacherNote, IContext context) =>
          assignments.Aggregate(context, (c, a) => a.MarkNotCompleted(teacherNote, c));


        internal static Assignment GetAssignmentForCurrentUser(int taskId, IContext context)
        {
            int pId = Tasks.GetTask(taskId, context).ProjectId.Value;
            int uId = Users.Me(context).Id;
            return context.Instances<Assignment>().Where(a => a.ProjectId == pId && a.AssignedToId == uId).OrderByDescending(a => a.DueBy).FirstOrDefault();
        }
    }
}
