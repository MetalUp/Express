namespace Model.Types
{
    public enum Role
    {
        Guest, Student, Teacher, Author, Admin, Root
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
}
