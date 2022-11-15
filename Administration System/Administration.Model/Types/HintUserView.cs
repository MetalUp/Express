
namespace Model.Types
{
    [ViewModel(typeof(HintUserView_Functions))]
    public class HintUserView
    {
        public HintUserView(string title, string contents, int? previousHintNo, int? nextHintNo, int costOfNextHint)
        {

            Title = title;
            Contents = contents;
            PreviousHintNo = previousHintNo;
            NextHintNo = nextHintNo;
            CostOfNextHint = costOfNextHint;
        }

        string Title { get; init; }

        string Contents { get; init; }

        int? PreviousHintNo { get; init; } //Null indicates there is no previous hint

        int? NextHintNo { get; init; } //Null indicates there is no next hint

        int CostOfNextHint { get; init; }  //If zero means that the user can just navigate to it (because they have seen it before)
    }
}
