namespace Model.Functions.Menus
{
    public static class Activities
    {
        public static IQueryable<Activity> AllActivities(IContext context) => 
            context.Instances<Activity>().OrderByDescending(a => a.TimeStamp);


        #region Methods to record an Activity
        internal static IContext SubmitCodeFail(int taskId, string code, string errorMessage, IContext context) =>
     RecordActivity(taskId, ActivityType.SubmitCodeFail, code, errorMessage, null, context);

        internal static IContext SubmitCodeSuccess(int taskId, string code, IContext context) =>
            RecordActivity(taskId, ActivityType.SubmitCodeSuccess, code, null, null, context);

        internal static IContext RunTestsFail(int taskId, string errorMessage, IContext context) =>
            RecordActivity(taskId, ActivityType.RunTestsFail, null, errorMessage, null, context);

        internal static IContext RunTestsSuccess(int taskId, IContext context)
        {
            var context2 = RecordActivity(taskId, ActivityType.RunTestsSuccess, null, null, null, context);
            var task = context.Instances<Task>().Single(t => t.Id == taskId);
            if (task.NextTaskId != null)
            {
                return context2;
            }
            else
            {
                var a = Assignments.GetAssignmentForCurrentUser(taskId, context);
                return context2.WithUpdated(a, new Assignment(a) { Status = AssignmentStatus.Completed });
            }
        }

        internal static IContext HintUsed(int taskId, int hintNo, IContext context) =>
            RecordActivity(taskId, ActivityType.HintUsed, null, null, null, context);
        #endregion


        internal static Activity GetLastCodeSubmittedSuccess(int taskId, IContext context) =>
            GetActivitiesOnTask(taskId, context).LastOrDefault(a => a.ActivityType == ActivityType.SubmitCodeSuccess);

        internal static Activity GetLastHintUsed(int taskId, IContext context) =>
            GetActivitiesOnTask(taskId, context).LastOrDefault(a => a.ActivityType == ActivityType.HintUsed);

        internal static Activity GetLastRunTestsSuccess(int taskId, IContext context) =>
            GetActivitiesOnTask(taskId, context).LastOrDefault(a => a.ActivityType == ActivityType.RunTestsSuccess);

        #region private helpers
        internal static IContext RecordActivity(int taskId, ActivityType type, string code, string message, int? hintUsed, IContext context)
        {
            var aId = Assignments.GetAssignmentForCurrentUser(taskId, context).Id;
            var act = new Activity()
            {
                AssignmentId = aId,
                TaskId = taskId,
                ActivityType = type,
                Message = message,
                TimeStamp = context.Now()
            };
            return context.WithNew(act);
        }

        internal static IQueryable<Activity> GetActivitiesOnTask(int taskId, IContext context)
        {
            var aId = Assignments.GetAssignmentForCurrentUser(taskId, context).Id;
            return context.Instances<Activity>().Where(act => act.AssignmentId == aId && act.TaskId == taskId);
        }
        #endregion 
    }
}
