namespace Model.Functions
{
    public static class Assignment_Functions
    {
        #region Activity
        public static IQueryable<Activity> ListActivity(this Assignment assignment, IContext context)
        {
            int assigId = assignment.Id;
            return context.Instances<Activity>().Where(a => a.AssignmentId == assigId).OrderByDescending(a => a.TimeStamp);
        }
        #endregion

        [Edit]
        public static IContext EditTeacherNotes(this Assignment assignment, string notes, IContext context) =>
            context.WithUpdated(assignment, new Assignment(assignment) { TeacherNotes = notes });
        
        public static string Default1EditTeacherNotes(this Assignment a) => a.TeacherNotes;

        public static IContext MarkNotCompleted(this Assignment a, string notes, IContext context) =>
            context.WithUpdated(a, new Assignment(a) { Status = AssignmentStatus.NotCompleted, TeacherNotes = notes });

        public static string Default1MarkNotCompleted(this Assignment a) => a.TeacherNotes;
    }
}
