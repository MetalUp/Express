using NakedFramework.Value;
using System.Text;

namespace Model.Functions
{


    public static class Task_Functions
    {
        #region AssignTo
        [MemberOrder(10)]
        public static IContext AssignTo(this Task task, [Optionally] Group inGroup, bool allInGroup, [Optionally] User singleUser, DateTime dueBy, IContext context) =>
           allInGroup ? throw new NotImplementedException("All In Group assignment not yet implemented") :
            Assignments.NewAssignmentToIndividual(singleUser, task, dueBy, context);


        public static string ValidateAssignTo(this Task task, Group inGroup, bool allInGroup, User singleUser, DateTime dueBy, IContext context) =>
            allInGroup && inGroup is null ? "Must specify a Group" :
               !allInGroup && singleUser is null ? "Must specify a Single User" : null;

        public static List<Group> Choices1AssignTo(this Task task, [Optionally] Group inGroup, bool allInGroup, [Optionally] User singleUser, DateTime dueBy, IContext context) =>
            Groups.AllOurGroups(context).ToList();

        public static List<User> Choices3AssignTo(this Task task, [Optionally] Group inGroup, bool allInGroup, [Optionally] User singleUser, DateTime dueBy, IContext context) =>
            inGroup is null ? 
                Users.Students(context).ToList() : 
                inGroup.Students.ToList();


        #endregion

        #region internal functions
        internal static bool IsPublic(this Task task) => task.Status == TaskStatus.Public;

        internal static bool IsAssignable(this Task task) => task.Status != TaskStatus.UnderDevelopment;

        internal static bool IsAssignedToCurrentUser(this Task task, IContext context)
        {
            var myId = Users.Me(context).Id;
            var taskId = task.Id;
            return context.Instances<Assignment>().Any(a => a.AssignedToId == myId && a.TaskId == taskId);
        }

        internal static string LanguageAsFileExtension(this Task task) =>
            task.Language switch
            {
                ProgrammingLanguage.Python => ".py",
                ProgrammingLanguage.CSharp => ".cs",
                ProgrammingLanguage.VB => ".vb",
                ProgrammingLanguage.Java => ".java",
                _ => ".txt"
            };
        #endregion

        #region Edit (via a VM & for authors only)
        public static TaskAuthorView Edit(this Task task) => new TaskAuthorView(task);

        #endregion
    }
}
