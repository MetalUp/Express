
namespace Model.Types
{
    [ViewModel(typeof(CodeUserView_Functions))]
    public class CodeUserView
    {
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
