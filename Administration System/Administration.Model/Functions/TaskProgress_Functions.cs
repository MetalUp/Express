

namespace Model.Functions;

public static class TaskProgress_Functions
{
    public static string[] DeriveKeys(this TaskProgress tp) => 
        new[] {tp.AssignmentId.ToString(), tp.TaskId.ToString(), tp.TaskNoOrSummary, tp.Status  };

    public static TaskProgress CreateFromKeys(string[] keys) => 
        new TaskProgress(Convert.ToInt32(keys[0]), Convert.ToInt32(keys[1]), keys[2], keys[3]);

}