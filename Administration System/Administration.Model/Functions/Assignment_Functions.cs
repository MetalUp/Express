﻿namespace Model.Functions
{
    public static class Assignment_Functions
    {
        #region Display
        public static bool HideLink(this Assignment assignment, IContext context) =>
            assignment.Status == AssignmentStatus.Completed || assignment.Status == AssignmentStatus.Terminated;
        #endregion
        #region Activity
        [RenderEagerly]
        [TableView(false, nameof(Activity.Task), nameof(Activity.ActivityType), nameof(Activity.HintUsed),nameof(Activity.TimeStamp))]
        public static IQueryable<Activity> ListActivity(this Assignment assignment, IContext context)
        {
            int assigId = assignment.Id;
            return context.Instances<Activity>().Where(a => a.AssignmentId == assigId).OrderByDescending(a => a.TimeStamp);
        }

        [RenderEagerly]
        [TableView(false, nameof(Activity.ActivityType), nameof(Activity.HintUsed), nameof(Activity.TimeStamp))]
        public static IQueryable<Activity> ListActivityForTask(this Assignment assignment, Task task, IContext context)
        {
            int taskId = task.Id;
            return ListActivity(assignment, context).Where(a => a.TaskId == taskId);
        }

        public static ICollection<Task> Choices1ListActivityForTask(this Assignment assignment) =>
            assignment.Project.Tasks;

        #endregion

        [Edit]
        public static IContext EditTeacherNotes(this Assignment assignment, string teacherNotes, IContext context) =>
            context.WithUpdated(assignment, new Assignment(assignment) { TeacherNotes = teacherNotes });
        
        public static string Default1EditTeacherNotes(this Assignment a) => a.TeacherNotes;

        public static IContext MarkAsTerminated(this Assignment a, string notes, IContext context) =>
            context.WithUpdated(a, new Assignment(a) { Status = AssignmentStatus.Terminated, TeacherNotes = notes });

        public static string Default1MarkAsTerminated(this Assignment a) => a.TeacherNotes;


        #region Internal
        internal static bool IsAssignedTo(this Assignment assgn, User user) => assgn.AssignedToId == user.Id;

        internal static bool IsAssignedByOrToAnyoneInOrganisation(this Assignment assgn, Organisation org) =>
            assgn.AssignedBy.OrganisationId == org.Id || assgn.AssignedTo.OrganisationId == org.Id;

        internal static bool IsCurrent(this Assignment assgn) => assgn.Status == AssignmentStatus.PendingStart || assgn.Status == AssignmentStatus.Started;
        #endregion
    }
}
