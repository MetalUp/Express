namespace Model.Functions.Menus
{
    public static class Assignments
    {



        [MemberOrder(10)]
        [RenderEagerly]
        [TableView(false, nameof(Assignment.Project), nameof(Assignment.AssignedTo), nameof(Assignment.DueBy), nameof(Assignment.Status))]
        public static IQueryable<Assignment> AllAssignments(IContext context) => context.Instances<Assignment>();

        [MemberOrder(20)]
        [PageSize(20)]
        [RenderEagerly]
        [TableView(false, nameof(Assignment.DueBy), nameof(Assignment.Status), nameof(Assignment.Project))]
        public static IQueryable<Assignment> MyCurrentAssignments(IContext context) =>
            AssignmentsTo(Users.Me(context), context).Where(a => a.Status == AssignmentStatus.PendingStart || a.Status == AssignmentStatus.Started).OrderBy(a => a.DueBy);

        [MemberOrder(25)]
        [PageSize(20)]
        [RenderEagerly]
        [TableView(false, nameof(Assignment.DueBy), nameof(Assignment.Status), nameof(Assignment.Project))]
        public static IQueryable<Assignment> MyPastAssignments(IContext context) =>
    AssignmentsTo(Users.Me(context), context).Where(a => a.Status == AssignmentStatus.Completed || a.Status == AssignmentStatus.Terminated).OrderBy(a => a.DueBy);

        internal static IQueryable<Assignment> AssignmentsTo(User user, IContext context)
        {
            var id = user.Id;
            return context.Instances<Assignment>().Where(a => a.AssignedToId == id).OrderByDescending(a => a.DueBy);
        }


        [MemberOrder(30)]
        [RenderEagerly]
        [TableView(false,  nameof(Assignment.DueBy), nameof(Assignment.Status), nameof(Assignment.AssignedTo), nameof(Assignment.Project))]
        public static IQueryable<Assignment> AssignmentsSetByMe( IContext context)
        {
            var meId = Users.Me(context).Id;
            return context.Instances<Assignment>().Where(s => s.AssignedById == meId).OrderByDescending(a => a.DueBy).ThenBy(a => a.Status);
        }


        [MemberOrder(40)]
        [RenderEagerly]
        [TableView(false, nameof(Assignment.AssignedTo), nameof(Assignment.Project), nameof(Assignment.DueBy), nameof(Assignment.Status))]
        public static IQueryable<Assignment> OverdueAssignmentsSetByMe(IContext context)
        {
            var today = context.Today();
            return AssignmentsSetByMe(context).Where(a => a.DueBy < today && a.Status != AssignmentStatus.Completed);
        }


        [MemberOrder(50)]
        [PageSize(20)]
        [RenderEagerly]
        [TableView(false, nameof(Assignment.Project), nameof(Assignment.AssignedTo), nameof(Assignment.DueBy), nameof(Assignment.Status))]
        public static IQueryable<Assignment> FindAssignmentsSetByMe(
             AssignmentStatus status,
            IContext context) =>
                 AssignmentsSetByMe(context)
                .Where(s => s.Status == status)
                .OrderByDescending(a => a.DueBy);


        [MemberOrder(60)]
        internal static (Assignment, IContext) NewAssignmentToIndividual(User assignedTo, Project project, [ValueRange(0, 30)] DateTime dueBy, IContext context)
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
            return (a, context.WithNew(a));
        }

        internal static IContext NewAssignmentToGroup(Group group, Project project, [ValueRange(0, 30)] DateTime dueBy, IContext context) =>
            group.Students.Aggregate(context, (c, s) => NewAssignmentToIndividual(s, project, dueBy, c).Item2);


        public static IContext MarkTasksNotCompleted(this IQueryable<Assignment> assignments, string teacherNote, IContext context) =>
          assignments.Aggregate(context, (c, a) => a.MarkAsTerminated(teacherNote, c));


        internal static Assignment GetAssignmentForCurrentUser(int taskId, IContext context)
        {
            var task = Tasks.GetTask(taskId, context);
            if (task == null) return null;
            int projectId =  task.ProjectId.Value;
            return AssignmentsForCurrentUser(projectId, context).OrderByDescending(a => a.DueBy).FirstOrDefault();
        }

        internal static IQueryable<Assignment> AssignmentsForCurrentUser(int projectId, IContext context)
        {
            int uId = Users.Me(context).Id;
            return context.Instances<Assignment>().Where(a =>
                    a.ProjectId == projectId
                    && a.AssignedToId == uId
                    );
        }

    }
}
