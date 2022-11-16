
namespace Model.Types
{
    [ViewModel(typeof(HintUserView_Functions))]
    public class HintUserView
    {
        public HintUserView(int taskId, int hintNo, string title, string contents, int previousHintNo, int nextHintNo, int costOfNextHint)
        {
            TaskId = taskId;
            HintNo = hintNo;
            Title = title;
            Contents = contents;
            PreviousHintNo = previousHintNo;
            NextHintNo = nextHintNo;
            CostOfNextHint = costOfNextHint;
        }

        public int TaskId { get; init; }

        public int HintNo { get; init; }

        public string Title { get; init; }

        public string Contents { get; init; }

        public int PreviousHintNo { get; init; } //0 indicates there is no previous hint

        public int NextHintNo { get; init; } //0 indicates there is no next hint

        public int CostOfNextHint { get; init; }  //If zero means that the user can just navigate to it (because they have seen it before)
    }
}
