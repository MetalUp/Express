namespace Model.Functions
{
    [Named("Assignments")]
    public static class Assignment_MenuFunctions
    {
        public static IQueryable<Assignment> AllAssignments(IContext context) =>
            context.Instances<Assignment>();

        public static IQueryable<Assignment> CurrentAssigments(IContext context)
        {
            throw new NotImplementedException();

        }

        public static IQueryable<Assignment> AssigmentsNowDue(IContext context)
        {
            throw new NotImplementedException();

        }

        #region Assignments for Student
        public static IQueryable<Assignment> AssignmentsForStudent(this User student, [Optionally] DateTime fromDate, [Optionally] DateTime toDate, IContext context)
        {
            throw new NotImplementedException();
        }

        //public static bool HideAssignmentsForStudent(this User student) //? Can we do this based on state of contributee?

        #endregion

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



    //ShowActiveAssignments(Group group, [Optionally] DateTime dueBy)
    //ShowActiveAssignments(Student student, [Optionally] DateTime dueBy)
    //ShowAssignments(Group group, DateTime fromDate, [Optionally], DateTime toDate, [Optionally] byTeacher)

    //ShowActiveTasks(Student student)
    //ListAssignments(Student student, 
 
    //MarkAsNotCompleted(Assignment assignment) //Only available to teachers - just to take off active list
}
