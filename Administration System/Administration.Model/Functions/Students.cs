

namespace Model.Functions
{
    public static class Students
    {
        public static Student MeAsStudent(IContext context)
        {
            var userId = Users.Me(context).Id;
            return context.Instances<Student>().SingleOrDefault(t => t.UserId == userId);
        }

        [PageSize(10)]
        [TableView(false, nameof(Assignment.DueBy), nameof(Assignment.Status), nameof(Assignment.Task))]
        public static IQueryable<Assignment> MyAssignments(IContext context) =>
          Student_Functions.Assignments(MeAsStudent(context), context);
    }
}
