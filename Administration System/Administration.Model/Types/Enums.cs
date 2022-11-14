namespace Model.Types
{
    public enum Role
    {
        Guest, Student, Teacher, Author, Root
    }

    public enum ActivityType
    {
        SubmitCodeFail, SubmitCodeSuccess, RunTestsFail, RunTestsSuccess, HintUsed
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
        UnderDevelopment, Assignable, Retired
    }

    public enum ContentType
    {
        Description, HiddenCode, Tests, Wrapper, Helpers, Rules
    }
}
