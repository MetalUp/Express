

namespace Model.Functions;

public static class UserViewModel_Functions
{
    public static string[] DeriveKeys(this UserViewModel target) => 
        new[] { target.UserId.ToString(), target.ActiveTaskId.ToString()};

    public static UserViewModel CreateFromKeys(string[] keys, IContext context)
    {
        var user = Users.FindById(int.Parse(keys[0]), context);
        var taskId = int.Parse(keys[1]);
        return new UserViewModel(user.Id, user.Name, taskId);
    }
}