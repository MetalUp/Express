using NakedFramework.Value;
using System.Text;

namespace Model.Functions
{


    public static class Task_Functions
    {
        #region Assigning
        [MemberOrder(10)]
        public static IContext AssignToIndividual(this Task task, User user, DateTime dueBy, IContext context) =>
            Assignments.NewAssignmentToIndividual(user, task, dueBy, context);

        [MemberOrder(10)]
        public static IContext AssignToAlInGroup(this Task task, Group group, DateTime dueBy, IContext context) =>
            Assignments.NewAssignmentToAllInGroup(group, task, dueBy, context);

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
        #endregion

        #region Authoring
        #region Editing Task properties
        [Edit]
        public static IContext EditTitle(
            this Task task,
            string title,
            IContext context) =>
                context.WithUpdated(task, new(task) { Title = title });

        [Edit]
        public static IContext EditLanguage(
            this Task task,
            ProgrammingLanguage language,
            IContext context) =>
                context.WithUpdated(task, new(task) { Language = language });

        [Edit]
        public static IContext EditMaxMarks(
            this Task task,
            int maxMarks,
            IContext context) =>
                context.WithUpdated(task, new(task) { MaxMarks = maxMarks });

        [Edit]
        public static IContext EditPasteExpression(
            this Task task,
            bool pasteExpression,
            IContext context) =>
                context.WithUpdated(task, new(task) { PasteExpression = pasteExpression });

        [Edit]
        public static IContext EditPasteFunctions(
            this Task task,
            bool pasteFunctions,
            IContext context) =>
                context.WithUpdated(task, new(task) { PasteFunctions = pasteFunctions });

        [Edit]
        public static IContext EditPreviousTask(
            this Task task,
            Task previousTask,
            IContext context) =>
                context.WithUpdated(task, new(task) { PreviousTask = previousTask });

        public static string ValidateEditPreviousTask(
            this Task task,
            Task previousTask,
            IContext context) =>
                previousTask.Id == task.Id ? "Cannot specify a task as its own Previous Task" :
                previousTask.Language == task.Language ? "" : "Previous Task must specify the same Language";

        [Edit]
        public static IContext EditNextTask(
           this Task task,
           Task nextTask,
           IContext context) =>
            context.WithUpdated(task, new(task) { NextTask = nextTask });

        public static string ValidateEditNextTask(
            this Task task,
            Task nextTask,
            IContext context) =>
                nextTask.Id == task.Id ? "Cannot specify a task as its own Next Task" :
                nextTask.Language == task.Language ? "" : "Next Task must specify the same Language";

        [Edit]
        public static IContext EditNextTaskClearsFunctions(
          this Task task,
          bool nextTaskClearsFunctions,
          IContext context) =>
            context.WithUpdated(task, new(task) { NextTaskClearsFunctions = nextTaskClearsFunctions });


        [Edit]
        public static IContext EditStatus(
          this Task task,
          TaskStatus status,
          IContext context) =>
            context.WithUpdated(task, new(task) { Status = status });

        #endregion

        #region FileAttachments

        [MemberOrder(20)]
        public static IContext LoadDescriptionFromFile(
            this Task task,
            FileAttachment file,
            IContext context) =>
                SaveDescription(task, file.GetResourceAsByteArray(), file.Name, file.MimeType, context);

        [MemberOrder(21)]
        public static IContext EnterDescriptionAsString(
            this Task task,
            [MultiLine(20)] string description,
            IContext context) =>
                 SaveDescription(task, Encoding.ASCII.GetBytes(description),
                     $"Desscription.html", 
                     "txt/html", 
                     context);

        public static IContext SaveDescription(
            this Task task,
            byte[] bytes,
            string name,
            string mimeType,
            IContext context) =>
                context.WithUpdated(task,
                    new Task(task)
                    {
                        DescContent = bytes,
                        DescName = name,
                        DescMime = mimeType,
                    });


        [MemberOrder(22)]
        public static IContext ClearDescription(
            this Task task,
            bool confirm,
            IContext context) =>
            context.WithUpdated(task,
                new Task(task)
                {
                    DescContent = null,
                    DescName = null,
                    DescMime = null,
                });

        public static string ValidateClearDescription(this Task task, bool confirm) =>
            confirm ? null : "Confirm must be selected";

        [MemberOrder(30)]
        public static IContext SpecifyHiddenFunctions(
            this Task task,
            FileAttachment file,
            IContext context) =>
                context.WithUpdated(task,
                    new Task(task)
                    {
                        RMFContent = file.GetResourceAsByteArray(),
                        RMFName = file.Name,
                        RMFMime = file.MimeType,
                    });

        [MemberOrder(32)]
        public static IContext ClearHiddenFunctions(
            this Task task,
            bool confirm,
            IContext context) =>
            context.WithUpdated(task,
                new Task(task)
                {
                    RMFContent = null,
                    RMFName = null,
                    RMFMime = null,
                });

        public static string ValidateClearReadyMadeFunctions(this Task task, bool confirm) =>
            confirm ? null : "Confirm must be selected";

        [MemberOrder(40)]
        public static IContext SpecifyTests(
            this Task task,
            FileAttachment tests,
            IContext context) =>
                context.WithUpdated(task,
                    new Task(task)
                    {
                        TestsContent = tests.GetResourceAsByteArray(),
                        TestsName = tests.Name,
                        TestsMime = tests.MimeType,
                    });

        [MemberOrder(42)]
        public static IContext ClearTests(
            this Task task,
            bool confirm,
            IContext context) =>
                context.WithUpdated(task,
                    new Task(task)
                    {
                        TestsContent = null,
                        TestsName = null,
                        TestsMime = null,
                    });

        public static string ValidateClearTests(this Task task, bool confirm) =>
            confirm ? null : "Confirm must be selected";

        #endregion

        #region Hints 
        [MemberOrder(50)]
        public static IContext AddHint(
            this Task task,
            int number,
            string title,
            [DefaultValue(1)] int costInMarks,
            [Optionally] FileAttachment file,
            IContext context)
        {
            var hint = new Hint
            {
                Number = number,
                Title = title,
                CostInMarks = costInMarks,
                TaskId = task.Id,
                Task = task,
                FileContent = file == null ? null : file.GetResourceAsByteArray(),
                FileName = file == null ? null : file.Name,
                FileMime = file == null ? null : file.MimeType
            };
            return context.WithNew(hint);
        }

        public static int Default1AddHint(this Task task) =>
            task.Hints.Count + 1;

        [MemberOrder(55)]
        public static IContext RemoveHint(this Task task, Hint hint, IContext context) =>
            context.WithDeleted(hint);

        public static List<Hint> Choices1RemoveHint(this Task task, Hint hint, IContext context) =>
            task.Hints.ToList();
        #endregion

        #region Copy
        public static (Task, IContext) DuplicateTaskForNewLanguage(this Task task,
            ProgrammingLanguage newLanguage,
            IContext context
            )
        {
            var newHints = task.Hints.Select(h => new Hint(h) { Id = 0}).ToList();
            var context2 = newHints.Aggregate(context, (c, h) => c.WithNew(h));
            var task2 = new Task(task)
            {
                Id = 0, //Because its a new object
                Language = newLanguage,
                RMFContent = null,
                TestsContent = null,
                NextTask = null,
                PreviousTask = null,
                Hints = newHints
            };
            return (task2, context2.WithNew(task2));
        }
        #endregion

        #endregion
    }
}
