
namespace Model.Functions
{
    [Named("Tasks")]
    public static class Task_MenuFunctions
    {
        public static (Task, IContext) CreateNewTask(string title, IContext context)
        {
            var t = new Task { Title = title };
            return (t, context.WithNew(t));
        }       

        public static IQueryable<Task> AllTasks(IContext context) =>context.Instances<Task>();
    }

    public static class Task_Functions
    {
        #region AssignTaskToGroup
        public static IContext AssignTaskToGroup(this Task task, Group group, DateTime dueBy, IContext context) =>
         group.Students.Aggregate(context, (c, s) => c.WithNew(NewAssignment(task, s, dueBy, User_MenuFunctions.Me(context))));

        public static IList<Group> Choices1AssignTaskToGroup(IContext context) =>
            Group_MenuFunctions.MyGroups(context);


        #endregion

        //Need autocomplete for group & default for assignedBy

        public static IContext AssignTaskToStudent(this Task task, User student, DateTime dueBy, IContext context) =>
                context.WithNew(NewAssignment(task, student, dueBy, User_MenuFunctions.Me(context)));

        //Need autocomplete for student & default for assignedBy
        private static Assignment NewAssignment(Task task, User student, DateTime dueBy, User assignedBy)
        {
            return new Assignment()
            {
                Task = task,
                AssignedTo = student,
                AssignedById = assignedBy.Id,
                DueBy = dueBy,
                Status = Activity.Assigned
            };
        }

        //ListActivity(Task task)
    }
}
