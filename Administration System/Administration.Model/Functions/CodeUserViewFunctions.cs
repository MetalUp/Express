namespace Model.Functions;

public static class CodeUserView_Functions
{
    public static string[] DeriveKeys(this CodeUserView target) =>
        new[] { target.TaskId.ToString(), target.Version.ToString()};

    public static CodeUserView CreateFromKeys(string[] keys, IContext context) =>
       TaskAccess.GetCodeVersion(Convert.ToInt32(keys[0]), Convert.ToInt32(keys[1]), context).Item1; //Can safely throw away context because this method is only called when view is refreshed
}