using Model.Functions.Services;

namespace Model.Functions;

public static class TaskUserView_Functions
{
    public static string[] DeriveKeys(this TaskUserView target) => 
        new[] { target.Task.Id.ToString(), target.CurrentHintNo.ToString()};

    public static TaskUserView CreateFromKeys(string[] keys, IContext context) => 
        TaskAccess.GetTask(Convert.ToInt32(keys[0]), Convert.ToInt32(keys[1]), context);
}