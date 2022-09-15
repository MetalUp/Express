using Model.Functions;

namespace Model.Functions.Menus
{
    public static class Students
    {

        [PageSize(10)]
        [TableView(false, nameof(Assignment.DueBy), nameof(Assignment.Status), nameof(Assignment.Task))]
        public static IQueryable<Assignment> MyAssignments(IContext context) =>
          StudentRepository.Me(context).Assignments(context);
    }
}
