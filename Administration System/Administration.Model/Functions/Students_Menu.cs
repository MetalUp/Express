

namespace Model.Functions
{
    [Named("Student Actions")]
    public static class Students_Menu
    {
        public static Student Me(IContext context)
        {
            var userId = Users_Menu.Me(context).Id;
            return context.Instances<Student>().Single(t => t.UserId == userId);
        }

        [PageSize(10)]
        [TableView(false, nameof(Assignment.DueBy), nameof(Assignment.Status), nameof(Assignment.Task))]       
        public static IQueryable<Assignment> MyAssignments(IContext context)
        {
            var studentId = Me(context).Id;
            return context.Instances<Assignment>().Where(a => a.AssignedToId == studentId).OrderByDescending(a => a.DueBy);
        }
    }
}
