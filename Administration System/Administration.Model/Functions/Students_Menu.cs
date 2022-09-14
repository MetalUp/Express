

namespace Model.Functions
{
    [Named("Student Actions")]
    public static class Students_Menu
    {

        [PageSize(10)]
        [TableView(false, nameof(Assignment.DueBy), nameof(Assignment.Status), nameof(Assignment.Task))]
        public static IQueryable<Assignment> MyAssignments(IContext context) =>
          Student_Functions.Assignments(Students.MeAsStudent(context), context);
    }
}
