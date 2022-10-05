namespace Model.Types
{
    public enum Role
    {
        Guest, Student, Teacher, Author, Root
    }

    public enum ProgrammingLanguage
    {
        Python, CSharp, VB, Java
    }

    public enum ActivityType
    {
        AccessedTask, SubmitFail, SubmitSuccess, TestFail, TestSuccess, HintUsed, TeacherNote
    }

    public enum UserStatus
    {
        PendingAcceptance, Active, NotAccepted, Inactive
    }

    public enum AssignmentStatus
    {
        PendingStart, Started, Completed, NotCompleted
    }

    public enum ProjectStatus
    {
        UnderDevelopment, Assignable
    }
}
