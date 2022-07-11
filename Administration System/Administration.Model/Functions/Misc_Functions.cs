
namespace Model.Functions
{

    //public static (Student, Group, IContext) AddStudentToGroup(Student student, Group group, IContext context) => throw new Exception();

    //AssignTaskToStudent
    //AssignTaskToGroup
    //ShowActiveAssignments(Group group, [Optionally] DateTime dueBy)
    //ShowActiveAssignments(Student student, [Optionally] DateTime dueBy)
    //ShowAssignments(Group group, DateTime fromDate, [Optionally], DateTime toDate, [Optionally] byTeacher)

    //ShowActiveTasks(Student student)
    //ListTasks(Student student, 
    //ListActivity(Task task)
    //MarkAsNotCompleted(Assignment assignment) //Only available to teachers - just to take off active list

    //Invitation functions
    //Invite Student - made by user in a Teacher role on behalf of their organisation
    //View outstanding invitations
    //Send reminder (to one or many)
    //Rescind outstanding (one or many)
    //Accept invitation - made by invitee, changes User role and adds to organisation



    //Group functions
    //Create New Group (for an organisation)
    //Assign Students (or Teachers) associated from the organisation, or from another group 
    //De-assign students (or teachers) from group
    //Edit group name / description


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
                AssignedBy = assignedBy,
                DueBy = dueBy,
                Status = Activity.Assigned
            };
        }


    }

    [Named("Tasks")]
    public static class Task_MenuFunctions
    {
        //Search for tasks by content
        //Find by Id
        //List Tasks previously assigned to a Group (by me only - defaults to true)
    }

    public static class Assignment_Functions
    {
        public static IContext MarkNotCompleted(this Assignment a, string teacherNote, IContext context) =>
            context.WithNew(new AssignmentActivity() { Assignment = a, TimeStamp = context.Now(), Activity = Activity.NotCompleted, Details = teacherNote })
            .WithUpdated(a, new Assignment(a) { Status = Activity.NotCompleted });

        public static IContext MarkTasksNotCompleted(this IQueryable<Assignment> assignments, string teacherNote, IContext context) =>
          assignments.Aggregate(context, (c, a) => MarkNotCompleted(a, teacherNote, c));

        //Called when the assignee navigates from the assignment to view of the task itself
        public static IContext StartAssigment(this Assignment a, IContext context) =>
            context.WithNew(new AssignmentActivity() { Assignment = a, TimeStamp = context.Now(), Activity = Activity.Started })
            .WithUpdated(a, new Assignment(a) { Status = Activity.Started });
    }

    [Named("Assignments")]
    public static class Assignment_MenuFunctions
    {
        public static IQueryable<Assignment> FindAssigments(Group toGroup, bool nowDue, bool current, IContext context)
        {
            int gId = toGroup.Id;
            return from a in context.Instances<Assignment>()
                   where a.GroupId == gId
                   select a;
        }

        //Find assignments to group, with filtering by status and/or dates, ordered by s
        //Find all assignments due by group, but listed individually
        //Find all tasks overdue, optionally by group
    }

}

