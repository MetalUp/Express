namespace Model.Types
{
    [ViewModel(typeof(UserViewModel_Functions))]
    public class UserViewModel
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public UserViewModel() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
