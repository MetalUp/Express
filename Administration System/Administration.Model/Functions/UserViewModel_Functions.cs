using Model.Functions.Services;

namespace Model.Functions;

public static class UserViewModel_Functions
{
    public static string[] DeriveKeys(this UserViewModel target) => 
        new[] { target.UserId.ToString()};

    public static UserViewModel CreateFromKeys(string[] keys, IContext context)
    {
        var user = Users.FindById(int.Parse(keys[0]), context);
        return new UserViewModel(user.Id, user.Name);
    }
}