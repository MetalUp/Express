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
        PendingStart, Started, Completed, Terminated
    }

    public enum ProjectStatus
    {
        UnderDevelopment, Assignable, Retired
    }

    public enum ContentType
    {
        Description, HiddenCode, Tests, WrapperCode, Helpers, RexExRules, Hint
    }

    public enum CompilerOutcome //Duplication of Outcome enum in CompileServer project
    {
        CompilationError = 11,
        RunTimeError = 12,
        TimeLimitExceeded = 13,
        Ok = 15,
        MemoryLimitExceeded = 17,
        IllegalSystemCall = 19,
        InternalError = 20,
        ServerOverload = 21
    }
}
