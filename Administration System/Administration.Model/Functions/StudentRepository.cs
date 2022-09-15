

namespace Model.Functions
{
    public static class StudentRepository
    {
        public static User Me(IContext context) => Users.Me(context);

        [PageSize(10)]
        [TableView(false, nameof(Assignment.DueBy), nameof(Assignment.Status), nameof(Assignment.Task))]
        public static IQueryable<Assignment> MyAssignments(IContext context) =>
          Student_Functions.Assignments(Me(context), context);
    }
}
