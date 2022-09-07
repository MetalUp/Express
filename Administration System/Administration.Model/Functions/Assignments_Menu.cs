namespace Model.Functions
{
    [Named("Assignments")]
    public static class Assignments_Menu
    {


        public static IQueryable<Assignment> CurrentAssigments(IContext context)
        {
            throw new NotImplementedException();

        }

        public static IQueryable<Assignment> AssigmentsNowDue(IContext context)
        {
            throw new NotImplementedException();

        }


        //ShowActiveAssignments(Group group, [Optionally] DateTime dueBy)
        //ShowActiveAssignments(Student student, [Optionally] DateTime dueBy)
        //ShowAssignments(Group group, DateTime fromDate, [Optionally], DateTime toDate, [Optionally] byTeacher)

        //ShowActiveTasks(Student student)
        //ListAssignments(Student student, 

    }
}
