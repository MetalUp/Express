
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
        public static IContext AssignTaskToGroup(this Task task, Group group, DateTime dueBy, User assignedBy, IContext context) =>
         group.Students.Aggregate(context, (c, s) => c.WithNew(NewAssignment(task, s, dueBy, assignedBy)));

        //Need autocomplete for group & default for assignedBy

        public static IContext AssignTaskToStudent(this Task task, User student, DateTime dueBy, User assignedBy, IContext context) =>
                context.WithNew(NewAssignment(task, student, dueBy, assignedBy));

        //Need autocomplete for student & default for assignedBy
        private static Assignment NewAssignment(Task task, User student, DateTime dueBy, User assignedBy)
        {
            return new Assignment()
            {
                Task = task,
                AssignedTo = student,
                Group = null,
                AssignedById = assignedBy.Id,
                DueBy = dueBy,
                Status = Activity.Assigned
            };
        }

        //ListActivity(Task task)
    }
}
