namespace Model.Functions.Services
{
    public class ActivityService
    {
        public IContext SubmitCodeFail(int taskId, string code, string errorMessage, IContext context) =>
            RecordActivity(taskId, ActivityType.SubmitCodeFail, code, errorMessage, null, context);

        public IContext SubmitCodeSuccess(int taskId, string code, IContext context) =>
            RecordActivity(taskId, ActivityType.SubmitCodeSuccess, code, null, null, context);

        public IContext RunTestsFail(int taskId, string errorMessage, IContext context) =>
            RecordActivity(taskId, ActivityType.RunTestsFail, null, errorMessage, null, context);

        public IContext RunTestsSuccess(int taskId, IContext context)
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

        public IContext HintUsed(int taskId, int hintNo, IContext context) =>
            RecordActivity(taskId, ActivityType.HintUsed, null, null, null, context);

        public Activity GetLastCodeSubmittedSuccess(int taskId, IContext context) =>
            GetActivitiesOnTask(taskId, context).LastOrDefault(a => a.ActivityType == ActivityType.SubmitCodeSuccess);

        public Activity GetLastHintUsed(int taskId, IContext context) =>
            GetActivitiesOnTask(taskId, context).LastOrDefault(a => a.ActivityType == ActivityType.HintUsed);

        public Activity GetLastRunTestsSuccess(int taskId, IContext context) =>
            GetActivitiesOnTask(taskId, context).LastOrDefault( a => a.ActivityType == ActivityType.RunTestsSuccess);

        #region private helpers
        private IContext RecordActivity(int taskId, ActivityType type, string code, string errorMessage, int? hintUsed, IContext context)
        {
            var aId = Assignments.GetAssignmentForCurrentUser(taskId, context).Id;
            var act = new Activity()
            {
                AssignmentId = aId,
                TaskId = taskId,
                ActivityType = type,
                ErrorMessage = errorMessage,
                TimeStamp = context.Now()
            };
            return context.WithNew(act);
        }


        private IQueryable<Activity> GetActivitiesOnTask(int taskId, IContext context)
        {
            var aId = Assignments.GetAssignmentForCurrentUser(taskId, context).Id;
            return context.Instances<Activity>().Where(act => act.AssignmentId == aId && act.TaskId == taskId);
        }


        #endregion {

    }
}
