namespace Model.Types
{
    public class Activity
    {
        public Activity() { }
        public Activity(Activity cloneFrom) 
        { 
            Id = cloneFrom.Id;
            AssignmentId = cloneFrom.AssignmentId;
            Assignment = cloneFrom.Assignment;
            Type = cloneFrom.Type;
            TimeStamp = cloneFrom.TimeStamp;
            SubmittedCode = cloneFrom.SubmittedCode;
        }

        [Hidden]
        public  int Id { get; init; }

        [Hidden]
        public int AssignmentId { get; init; }
        [MemberOrder(2)]
        public virtual Assignment Assignment { get; init; }

        [MemberOrder(4)]
        public ActivityType Type { get; init; }

        [MemberOrder(6)]
        public DateTime TimeStamp { get; init; }

        [MemberOrder(8)]
        public string SubmittedCode { get; init; }    

        public override string ToString() => Type.ToString();
    }


}
