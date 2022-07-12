namespace Model.Functions
{
    [Named("Assignments")]
    public static class Assignment_MenuFunctions
    {
        public static IQueryable<Assignment> AllAssignments(IContext context) =>
            context.Instances<Assignment>();

        #region FindAssigments
        public static IQueryable<Assignment> FindAssigments(Group toGroup, bool nowDue, bool current, IContext context)
        {
            int gId = toGroup.Id;
            var today = context.Today();
            return from a in context.Instances<Assignment>()
                   where a.GroupId == gId && (!nowDue || a.DueBy < today) && (!current || a.DueBy >= today)
                   select a;
        }

        public static IList<Group> Choices0FindAssignments(IContext context) => Group_MenuFunctions.MyGroups(context);
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
