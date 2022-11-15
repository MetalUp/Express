﻿namespace Model.Types
{
    public class Activity
    {
        public Activity() { }
        public Activity(Activity cloneFrom) 
        { 
            Id = cloneFrom.Id;
            AssignmentId = cloneFrom.AssignmentId;
            Assignment = cloneFrom.Assignment;
            TaskId = cloneFrom.TaskId;
            Task = cloneFrom.Task;
            ActivityType = cloneFrom.ActivityType;
            TimeStamp = cloneFrom.TimeStamp;
            HintUsed = cloneFrom.HintUsed;
            CodeSubmitted = cloneFrom.CodeSubmitted;
            Message = cloneFrom.Message;
        }

        public Activity(int assignmentId, int taskId, ActivityType activityType, int? hintUsed, string codeSubmitted, string message, IContext context)
        {
            AssignmentId = assignmentId;
            TaskId = taskId;
            ActivityType = activityType;
            TimeStamp = context.Now();
            HintUsed = hintUsed??hintUsed.Value;
            CodeSubmitted = codeSubmitted;
            Message = message;
        }

        [Hidden]
        public  int Id { get; init; }

        [Hidden]
        public int AssignmentId { get; init; }
        [MemberOrder(2)]
        public virtual Assignment Assignment { get; init; }

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
        public string CodeSubmitted { get; init; }

        [MemberOrder(9)]
        public string Message { get; init; }

        public override string ToString() => $"{TimeStamp} {ActivityType}";
    }


}
