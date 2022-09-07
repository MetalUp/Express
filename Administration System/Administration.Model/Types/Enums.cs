namespace Model.Types
{
    public enum Role
    {
        Guest, Student, Teacher, Author, Administrator, Root
    }

    public enum ProgrammingLanguage
    {
        Python, Csharp, VB, Java
    }

    public enum ActivityType
    {
        AccessedTask, SubmitFail, SubmitSuccess, TestFail, TestSuccess, HintUsed
    }

    public enum InvitationStatus
    {
        Pending, Accepted, Expired, Rescinded
    }

    public enum AssignmentStatus
    {
        PendingStart, Started, Completed, NotCompleted
    }
}
