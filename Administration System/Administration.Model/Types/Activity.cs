namespace Model.Types
{
    public class Activity
    {
        public Activity() { }
        public Activity(Activity cloneFrom) 
        { 
            Id = cloneFrom.Id;
            UserId = cloneFrom.UserId;  
            AssignmentId = cloneFrom.AssignmentId;
            TaskId = cloneFrom.TaskId;
            Task = cloneFrom.Task;
            ActivityType = cloneFrom.ActivityType;
            TimeStamp = cloneFrom.TimeStamp;
            HintUsed = cloneFrom.HintUsed;
            CodeSubmitted = cloneFrom.CodeSubmitted;
            Message = cloneFrom.Message;
        }

        public Activity( int userId, int? assignmentId, int taskId, ActivityType activityType, int hintUsed, string codeSubmitted, string message, IContext context)
        {
            UserId = userId;    
            AssignmentId = assignmentId;
            TaskId = taskId;
            ActivityType = activityType;
            TimeStamp = context.Now();
            HintUsed = hintUsed;
            CodeSubmitted = codeSubmitted;
            Message = message;
        }

        [Hidden]
        public  int Id { get; init; }

        [Hidden]
        public int? UserId { get; init; }

        [Hidden]
        public int? AssignmentId { get; init; }

        [Hidden]
        public int TaskId { get; init; }
        [MemberOrder(3)]
        public virtual Task Task { get; init; }

        [MemberOrder(4)]
        public ActivityType ActivityType { get; init; }

        [MemberOrder(6)]
        public DateTime TimeStamp { get; init; }

        [MemberOrder(7)]
        public int HintUsed { get; init; }

        [MemberOrder(8)]
        public FileAttachment SubmittedCode => new FileAttachment(Encoding.ASCII.GetBytes(CodeSubmitted), "Click here to view", "text/plain");

        [Hidden]
        public string CodeSubmitted { get; init; }

        [MemberOrder(9)]
        public string Message { get; init; }

        private string hintNumber() => ActivityType == ActivityType.HintUsed ? HintUsed.ToString() : "";

        public override string ToString() => $"{TimeStamp} {ActivityType} {hintNumber()}";
    }


}
