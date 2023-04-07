namespace Model.Functions.Menus
{
    public static class Activities
    {
        public static IQueryable<Activity> AllActivities(IContext context) =>
            context.Instances<Activity>().OrderByDescending(a => a.TimeStamp);


        #region Methods to record an Activity involving code
        internal static IContext SubmitCodeFail(int taskId, string code, string message, IContext context)
        {
            var context2 = RecordActivity(taskId, ActivityType.SubmitCodeFail, code, message, 0, context);
            return IfAssignedMarkAsStarted(taskId, context2);
        }

        internal static IContext SubmitCodeSuccess(int taskId, string code, IContext context)
        {

            var context2 = RecordActivity(taskId, ActivityType.SubmitCodeSuccess, code, null, 0, context);
            return IfAssignedMarkAsStarted(taskId, context2);
        }

        private static IContext IfAssignedMarkAsStarted(int taskId, IContext context)
        {
            var a = Assignments.GetAssignmentForCurrentUser(taskId, context);
            if (a == null) return context;
            return a.Status == AssignmentStatus.PendingStart ? context.WithUpdated(a, new Assignment(a) { Status = AssignmentStatus.Started }) : context;
        }

        internal static IContext RunTestsFail(int taskId, string message, string code, IContext context) =>
            RecordActivity(taskId, ActivityType.RunTestsFail, code, message, 0, context);

        internal static IContext RunTestsSuccess(int taskId, string code, IContext context)
        {
            var context2 = RecordActivity(taskId, ActivityType.RunTestsSuccess, code, null, 0, context);
            var task = context.Instances<Task>().Single(t => t.Id == taskId);
            var next = task.NextTaskId == null ? null : context.Instances<Task>().Single(t => t.Id == taskId);
            if (next == null || !next.HasTests())
            {
                var a = Assignments.GetAssignmentForCurrentUser(taskId, context);
                return context2.WithUpdated(a, new Assignment(a) { Status = AssignmentStatus.Completed });
            }
            else
            {
                return context2;
            }
        }

        internal static IContext RecordActivity(int taskId, ActivityType type, string code, string message, int hintUsed, IContext context)
        {
            Assignment assign = Assignments.GetAssignmentForCurrentUser(taskId, context);
            int? aId = assign?.Id;
            int uId = Users.Me(context).Id;
            var act = new Activity(uId, aId, taskId, type, hintUsed, code, message, context);
            return context.WithNew(act);
        }
        #endregion


        #region other internal methods

        internal static IQueryable<Activity> ActivitiesOfCurrentUser(IContext context)
        {
            int uId = Users.Me(context).Id;
            return AllActivities(context).Where(a => a.UserId == uId);
        }

        internal static IQueryable<Activity> ActivitiesOfCurrentUser(int taskId, IContext context) =>
            ActivitiesOfCurrentUser(context).Where(a => a.TaskId == taskId);


        internal static Activity MostRecentActivityOfType(ActivityType type, Task task, IContext context) =>
            ActivitiesOfCurrentUser(task.Id, context).Where(a => a.ActivityType == type).FirstOrDefault();
        #endregion
    }
}
