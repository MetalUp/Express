using NakedFramework.Value;
using System.Text;

namespace Model.Functions
{


    public static class Project_Functions
    {
        #region AssignTo
        [MemberOrder(10)]
        public static IContext AssignToIndividual(this Project project, User singleUser, DateTime dueBy, IContext context) =>
            Assignments.NewAssignmentToIndividual(singleUser, project, dueBy, context);

        [MemberOrder(20)]
        public static IContext AssignToGroup(this Project project, Group group, DateTime dueBy, IContext context) =>
         Assignments.NewAssignmentToGroup(group, project, dueBy, context);


        public static string ValidateAssignTo(this Project project, Group inGroup, bool allInGroup, User singleUser, DateTime dueBy, IContext context) =>
            allInGroup && inGroup is null ? "Must specify a Group" :
               !allInGroup && singleUser is null ? "Must specify a Single User" : null;

        public static List<Group> Choices1AssignTo(this Project project, [Optionally] Group inGroup, bool allInGroup, [Optionally] User singleUser, DateTime dueBy, IContext context) =>
            Groups.AllOurGroups(context).ToList();
        #endregion

        #region internal functions
        internal static bool IsAssignable(this Project project) => project.Status == ProjectStatus.Assignable;

        internal static bool IsAssignedToCurrentUser(this Project project, IContext context)
        {
            var myId = Users.Me(context).Id;
            var pid = project.Id;
            return context.Instances<Assignment>().Any(a => a.AssignedToId == myId && a.ProjectId == pid);
        }

        internal static string LanguageAsFileExtension(this Project project) =>
            project.Language switch
            {
                ProgrammingLanguage.Python => "_py.txt",
                ProgrammingLanguage.CSharp => "_cs.txt",
                ProgrammingLanguage.VB => "_vb.txt",
                ProgrammingLanguage.Java => "_java.txt",
                _ => ".txt"
            };
        #endregion

        #region Edit Task (author action)
        public static TaskAuthorView EditTask(this Project project, Task task ) => new TaskAuthorView(task);

        public static ICollection<Task> Choices1EditTask(this Project project) => project.Tasks;
        #endregion

        #region Create Task (author action)
        public static IContext CreateTask(
            this Project project, 
            string title,
            [Optionally] Task previousTask,
            [Optionally] Task nextTask,
            IContext context)
        {
            var t = new Task {
                ProjectId = project.Id, 
                Project = project, 
                Title = title, 
                PreviousTaskId  = previousTask is null? null : previousTask.Id,
                PreviousTask = previousTask,
                NextTaskId = nextTask is null ? null : nextTask.Id,
                NextTask = nextTask
            };
            return context.WithNew(t);
        }

        public static ICollection<Task> Choices2CreateTask(
            this Project project) =>
            project.Tasks;

        public static Task Default2CreateTask(
            this Project project) =>
            project.Tasks.LastOrDefault();

        public static ICollection<Task> Choices3CreateTask(
            this Project project) =>
            project.Tasks;

        #endregion
    }
}
