namespace Model.Types
{
    [ViewModel(typeof(UserViewModel_Functions))]
    public class UserViewModel
    {
        public UserViewModel() { }
        public UserViewModel(int userId, string displayName)
        {
            UserId = userId;
            DisplayName = displayName;
        }

        internal int UserId { get; init; }

        public string DisplayName { get; init; }
    }
}
