namespace Model.Functions;

public static class HintUserView_Functions
{
    public static string[] DeriveKeys(this HintUserView target) =>
        new[] { target.TaskId.ToString(), target.HintNo.ToString() };

    public static HintUserView CreateFromKeys(string[] keys, IContext context) =>
       TaskAccess.GetHint(Convert.ToInt32(keys[0]), Convert.ToInt32(keys[1]), context).Item1; //Safe to ignore returned IContext,
           //because this method only ever called if the view has been previously accessed
       

}