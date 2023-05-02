
namespace Model.Types
{
    [ViewModel(typeof(CodeUserView_Functions))]
    public class CodeUserView
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public CodeUserView() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public CodeUserView(int taskId, string code, int version, bool hasPreviousVersion)
        {
            TaskId = taskId;
            Code = code;
            Version = version;
            HasPreviousVersion = hasPreviousVersion;
        }

        public int TaskId { get; init; }

        public string Code { get; init; }

        public int Version { get; init; }

        public bool HasPreviousVersion { get; init; }
    }
}
