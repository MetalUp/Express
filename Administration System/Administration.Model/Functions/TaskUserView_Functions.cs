

namespace Model.Functions;

public static class TaskUserView_Functions
{
    public static string[] DeriveKeys(this TaskUserView target) => 
        new[] { target.Task  == null ? "0" :target.Task.Id.ToString()};

    public static TaskUserView CreateFromKeys(string[] keys, IContext context) => 
        TaskAccess.GetTask(Convert.ToInt32(keys[0]), context);
}