namespace Model.Types
{
    public class Activity
    {
        public Activity() { }


        [Hidden]
        public  int Id { get; init; }

        [Hidden]
        public int AssigmentId { get; init; }
        public virtual Assignment Assignment { get; init; }

        public DateTime TimeStamp { get; init; }

        public ActivityType Type { get; init; }

        public string Details { get; init; }    

        public override string ToString() => Type.ToString();
    }

    public enum ActivityType
    {
        Assigned, Started, Submit_Fail, UsedHint, Submit_Success, Test_Fail, Test_Success, Completed, NotCompleted
    } 
}
