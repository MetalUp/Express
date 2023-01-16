namespace Model.Functions.Menus
{
    public static class Activities
    {
        public static IQueryable<Activity> AllActivities(IContext context) => 
            context.Instances<Activity>().OrderByDescending(a => a.TimeStamp);


        #region Methods to record an Activity involving code
        internal static IContext SubmitCodeFail(int taskId, string code, string message, IContext context)
        {
            var context2 = RecordActivity(taskId, ActivityType.SubmitCodeFail, code, message, null, context);
            return EnsureAssignmentIsStarted(taskId, context2);
        }

        internal static IContext SubmitCodeSuccess(int taskId, string code, IContext context)
        {

            var context2 = RecordActivity(taskId, ActivityType.SubmitCodeSuccess, code, null, null, context);
            return EnsureAssignmentIsStarted(taskId, context2);
        }

        private static IContext EnsureAssignmentIsStarted(int taskId, IContext context)
        {
            var a = Assignments.GetAssignmentForCurrentUser(taskId, context);
            return a.Status == AssignmentStatus.PendingStart ? context.WithUpdated(a, new Assignment(a) { Status = AssignmentStatus.Started }) : context;
        }

        internal static IContext RunTestsFail(int taskId, string message, string code, IContext context) =>
            RecordActivity(taskId, ActivityType.RunTestsFail, code, message, null, context);

        internal static IContext RunTestsSuccess(int taskId, string code, IContext context)
        {
            var context2 = RecordActivity(taskId, ActivityType.RunTestsSuccess, code, null, null, context);
            var task = context.Instances<Task>().Single(t => t.Id == taskId);
            var next = task.NextTaskId == null ? null : context.Instances<Task>().Single(t => t.Id == taskId);
            if (next == null || !next.HasTests() )
            {
                var a = Assignments.GetAssignmentForCurrentUser(taskId, context);
                return context2.WithUpdated(a, new Assignment(a) { Status = AssignmentStatus.Completed });
            }
            else
            {
                return context2;
            }
        }

        internal static IContext RecordActivity(int taskId, ActivityType type, string code, string message, int? hintUsed, IContext context)
        {
            var aId = Assignments.GetAssignmentForCurrentUser(taskId, context).Id;
            var act = new Activity()
            {
                AssignmentId = aId,
                TaskId = taskId,
                ActivityType = type,
                CodeSubmitted = code,
                Message = message,
                TimeStamp = context.Now()
            };
            return context.WithNew(act);
        }
        #endregion


        #region other internal methods

        internal static IQueryable<Activity> ActivitiesOfCurrentUser(int taskId, IContext context) =>
            Assignments.GetAssignmentForCurrentUser(taskId, context).ListActivity(taskId, context);
        #endregion
    }
}
