namespace Model.Types
{
    [ViewModel(typeof(UserViewModel_Functions))]
    public class UserViewModel
    {
        public UserViewModel() { }
        public UserViewModel(int userId, string displayName, int activeTaskId)
        {
            UserId = userId;
            DisplayName = displayName;
            ActiveTaskId = activeTaskId;
        }

        internal int UserId { get; init; }

        public string DisplayName { get; init; }

        //0 indicates 'no active task'
        public int ActiveTaskId { get; init; }
    }
}
