namespace Model.Types
{
    public class AssignmentActivity
    {
        [Hidden]
        public  int Id { get; init; }

        public virtual Assignment Assignment { get; init; }

        public DateTime TimeStamp { get; init; }

        public Activity Activity { get; init; }

        public string Details { get; init; }    

        public override string ToString() => Activity.ToString();
    }

    public enum Activity
    {
        Assigned, Started, Unsuccessful_Code_Submission, Successful_Code_Submission, Completed, NotCompleted
    }
}
