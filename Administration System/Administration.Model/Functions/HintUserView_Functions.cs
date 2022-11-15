using Model.Functions.Services;

namespace Model.Functions;

public static class HintUserView_Functions
{
    public static string[] DeriveKeys(this HintUserView target) =>
        new[] { target.TaskId.ToString(), target.HintNo.ToString() };

    public static HintUserView CreateFromKeys(string[] keys, IContext context) => throw new NotImplementedException();
    //TaskAccess.GetHint(Convert.ToInt32(keys[0]), Convert.ToInt32(keys[1]), context)

}