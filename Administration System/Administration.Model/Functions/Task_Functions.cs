using NakedFramework.Value;
using System.Text;

namespace Model.Functions
{
    public static class Task_Functions
    {
        internal static bool IsDefault(this Task task) => task.Project.Status == ProjectStatus.Public;

        #region Editing Task properties
        [Edit]
        public static IContext EditTitle(
            this Task task,
            string title,
            IContext context) =>
                context.WithUpdated(task, new(task) { Title = title });

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

        #endregion

        #region FileAttachments
        #region Description
        [MemberOrder(20)]
        public static IContext LoadDescriptionFromFile(
            this Task task,
            FileAttachment file,
            IContext context) =>
                SaveDescription(task, file.GetResourceAsByteArray(), file.Name, context);

        [MemberOrder(21)]
        public static IContext EnterDescriptionAsString(
            this Task task,
            [MultiLine(20)] string description,
            IContext context) =>
             SaveDescription(task, Encoding.ASCII.GetBytes(description),
                 $"Description.html",
                 context);

        internal static IContext SaveDescription(
            this Task task,
            byte[] bytes,
            string name,
            IContext context) =>
                context.WithUpdated(task,
                    new(task)
                    {
                        DescContent = bytes,
                        DescName = name,
                    });


        [MemberOrder(22)]
        public static IContext ClearDescription(
            this Task task,
            bool confirm,
            IContext context) =>
            context.WithUpdated(task,
                new(task)
                {
                    DescContent = null,
                    DescName = null,
                });

        public static string ValidateClearDescription(this Task task, bool confirm) =>
            confirm ? null : "Confirm must be selected";
        #endregion

        #region Hidden Functions
        //[MemberOrder(30)]
        //public static IContext LoadHiddenFunctionsFromFile(
        //    this Task task,
        //    FileAttachment file,
        //    IContext context) =>
        //    SaveHiddenFunctions(task, file.GetResourceAsByteArray(), file.Name, file.MimeType, context);

        [MemberOrder(31)]
        public static IContext EditHiddenFunctionsAsString(
            this Task task,
            [MultiLine(20)] string hiddenFunctionsCode,
            IContext context) =>
                 SaveHiddenFunctions(task, Encoding.ASCII.GetBytes(hiddenFunctionsCode),
                     $"HiddenFunctions{task.Project.LanguageAsFileExtension()}",
                     "text/plain",
                     context);

        public static string Default1EditHiddenFunctionsAsString(this Task task) =>
            task.RMFContent is null ? null : Encoding.Default.GetString(task.RMFContent);

        internal static IContext SaveHiddenFunctions(
            this Task task,
            byte[] bytes,
            string name,
            string mimeType,
            IContext context) =>
                context.WithUpdated(task, new(task) { RMFContent = bytes });


        [MemberOrder(32)]
        public static IContext ClearHiddenFunctions(
            this Task task,
            bool confirm,
            IContext context) =>
            context.WithUpdated(task, new(task) { RMFContent = null });

        public static string ValidateClearReadyMadeFunctions(this Task task, bool confirm) =>
            confirm ? null : "Confirm must be selected";
        #endregion

        #region Tests
        //[MemberOrder(40)]
        //public static IContext LoadTestsFromFile(
        //    this Task task,
        //    FileAttachment file,
        //    IContext context) =>
        //    SaveTests(task, file.GetResourceAsByteArray(), file.Name, file.MimeType, context);

        [MemberOrder(41)]
        public static IContext EditTestsAsString(
            this Task task,
            [MultiLine(20)] string testsCode,
            IContext context) =>
                 SaveTests(task, Encoding.ASCII.GetBytes(testsCode),
                     $"Tests{task.Project.LanguageAsFileExtension()}",
                     "text/plain",
                     context);

        public static string Default1EditTestsAsString(this Task task) =>
            task.TestsContent is null ? null : Encoding.Default.GetString(task.TestsContent);


        internal static IContext SaveTests(
            this Task task,
            byte[] bytes,
            string name,
            string mimeType,
            IContext context) =>
                context.WithUpdated(task, new(task) { TestsContent = bytes });

        [MemberOrder(42)]
        public static IContext ClearTests(
            this Task task,
            bool confirm,
            IContext context) =>
                context.WithUpdated(task, new(task) { TestsContent = null });

        public static string ValidateClearTests(this Task task, bool confirm) =>
            confirm ? null : "Confirm must be selected";
        #endregion

        #endregion

        #region Hints 
        [MemberOrder(50)]
        public static IContext AddNewHint(
            this Task task,
            int number,
            string title,
            [DefaultValue(1)] int costInMarks,
            FileAttachment file,
            IContext context)
        {
            var hint = new Hint
            {
                Number = number,
                Title = title,
                CostInMarks = costInMarks,
                FileContent = file == null ? null : file.GetResourceAsByteArray(),
            };        
            return context.WithNew(hint).WithUpdated(task,new Task(task) { Hints = task.Hints.Append(hint).ToList() });
        }

        public static int Default1AddNewHint(this Task task) =>
            task.Hints.Count + 1;

        [MemberOrder(50)]
        public static IContext UseExistingHintsFrom(
            this Task thisTask,
            Task otherTask,
            IContext context) =>
            context.WithUpdated(thisTask,new Task(thisTask) { Hints = new List<Hint>(otherTask.Hints) });

        #endregion
    }
}
