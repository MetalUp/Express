namespace Model.Types
{
    public class Activity
    {
        public Activity() { }

        public Activity(Activity cloneFrom) 
        { 
            Id = cloneFrom.Id;
            TaskId = cloneFrom.TaskId;
            Task = cloneFrom.Task; 
            TimeStamp = cloneFrom.TimeStamp;
            Type = cloneFrom.Type;
            Code = cloneFrom.Code;
        }

        [Hidden]
        public  int Id { get; init; }

        [Hidden]
        public int TaskId { get; init; }

        public virtual Task Task { get; init; }

        public DateTime TimeStamp { get; init; }

        public ActivityType Type { get; init; }

        public string Code { get; init; }    

        public override string ToString() => Type.ToString();
    }


}
