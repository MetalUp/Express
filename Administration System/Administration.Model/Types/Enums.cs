namespace Model.Types
{
    public enum Role
    {
        Guest, Student, Teacher, Author, Administrator, Root
    }

    public enum ProgrammingLanguage
    {
        Python, CSharp, VB, Java
    }

    public enum ActivityType
    {
        AccessedTask, SubmitFail, SubmitSuccess, TestFail, TestSuccess, HintUsed
    }

    public enum MemberStatus
    {
        Pending, Active, NotAccepted, NoLongerActive
    }

    public enum AssignmentStatus
    {
        PendingStart, Started, Completed, NotCompleted
    }
}
