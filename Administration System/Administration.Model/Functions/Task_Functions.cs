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
                context.WithUpdated(task, new(task) { Name = title });

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
        public static IContext AddDescriptionFromFile(
            this Task task,
            FileAttachment file,
            IContext context) =>
                SaveDescriptionAsFile(task, file.Name, file.GetResourceAsByteArray(), context);

        public static string DisableAddDescriptionFromFile(this Task task) =>
            task.HiddenFunctionsFileId is null ? null : "Either go to Description file and reload/edit it, or Clear Description to create a new file here.";

        [MemberOrder(21)]
        public static IContext AddDescriptionAsString(
            this Task task,
            [MultiLine(10)] string content,
            IContext context) =>
                SaveDescriptionAsFile(task, $"Description{task.Project.LanguageAsFileExtension()}", Encoding.ASCII.GetBytes(content), context);

        public static string DisableAddDescriptionAsString(this Task task) => DisableAddDescriptionFromFile(task);


        private static IContext SaveDescriptionAsFile(
            this Task task,
            string name,
            byte[] content,
            IContext context)
                {
                    var author = Users.Me(context);
                    var f = new File() { Name = name, Content = content, Mime = "text/html", AuthorId = author.Id, Author = author };
                    return context
                        .WithNew(f)
                        .WithUpdated(task,
                            new(task)
                            {
                                DescriptionFileId = f.Id,
                                DescriptionFile = f,
                            });
                }

        [Edit]
        public static IContext EditDesciption(
            this Task task,
            File descriptionFile,
            IContext context) =>
                context.WithUpdated(task, new Task(task) { DescriptionFileId = descriptionFile.Id, DescriptionFile = descriptionFile });

        [MemberOrder(22)]
        public static IContext ClearDesciption(
            this Task task,
            IContext context) =>
                context.WithUpdated(task, new Task(task) { DescriptionFileId = null, DescriptionFile = null });


        public static string DisableClearDescription(this Task task) => task.TestsFileId is null ? "No Description specified" : null;
        #endregion

        #region Hidden Functions
        [MemberOrder(30)]
        public static IContext AddHiddenFunctionsFromFile(
            this Task task,
            FileAttachment file,
            IContext context) =>
                SaveHiddenFunctionsAsFile(task, file.Name, file.GetResourceAsByteArray(), context);

        public static string DisableAddHiddenFunctionsFromFile(this Task task) =>
            task.HiddenFunctionsFileId is null ? null : "Either go to Hidden Functions file and reload/edit it, or Clear Hidden Functions to create a new file here.";

        [MemberOrder(31)]
        public static IContext AddHiddenFunctionsAsString(
            this Task task,
            [MultiLine(10)] string content,
            IContext context) =>
                SaveHiddenFunctionsAsFile(task, $"HiddenFunctions{task.Project.LanguageAsFileExtension()}", Encoding.ASCII.GetBytes(content), context);

        public static string DisableAddHiddenFunctionsAsString(this Task task) => DisableAddHiddenFunctionsFromFile(task);

        private static IContext SaveHiddenFunctionsAsFile(
            this Task task,
            string name,
            byte[] content,
            IContext context)
                {
                    var author = Users.Me(context);
                    var f = new File() { Name = name, Content = content, Mime = "text/plain", AuthorId = author.Id, Author = author };
                    return context
                        .WithNew(f)
                        .WithUpdated(task,
                            new(task)
                            {
                                HiddenFunctionsFileId = f.Id,
                                HiddenFunctionsFile = f,
                            });
                }

        [Edit]
        public static IContext EditHiddenFunctions(
            this Task task,
            File hiddenFunctionsfile,
            IContext context) =>
                context.WithUpdated(task, new Task(task) { HiddenFunctionsFileId = hiddenFunctionsfile.Id, HiddenFunctionsFile = hiddenFunctionsfile });

        [MemberOrder(32)]
        public static IContext ClearHiddenFunctions(
            this Task task,
            IContext context) =>
                context.WithUpdated(task, new Task(task) { HiddenFunctionsFileId = null, HiddenFunctionsFile = null });

        public static string DisableClearHiddenFunctions(this Task task) => task.TestsFileId is null ? "No Hidden Functions specified" : null;
        #endregion

        #region Tests
        [MemberOrder(40)]
        public static IContext AddTestsFromFile(
            this Task task,
            FileAttachment file,
            IContext context) =>
                SaveTestsAsFile(task, file.Name, file.GetResourceAsByteArray(), context);

        public static string DisableAddTestsFromFile(this Task task) =>
            task.TestsFileId is null ? null : "Either go to Tests file and reload/edit it, or Clear Tests to create a new file here.";

        [MemberOrder(41)]
        public static IContext AddTestsAsString(
            this Task task,
            [MultiLine(10)] string content,
            IContext context) =>
                SaveTestsAsFile(task, $"Tests{task.Project.LanguageAsFileExtension()}", Encoding.ASCII.GetBytes(content), context);

        public static string DisableAddTestsAsString(this Task task) => DisableAddTestsFromFile(task);

        private static IContext SaveTestsAsFile(
            this Task task,
            string name,
            byte[] content,
            IContext context)
                {
                    var author = Users.Me(context);
                    var f = new File() { Name = name, Content = content, Mime = "text/plain", AuthorId = author.Id, Author = author };
                    return context
                        .WithNew(f)
                        .WithUpdated(task,
                            new(task)
                            {
                                TestsFileId = f.Id,
                                TestsFile = f,
                            });
                }

        [Edit]
        public static IContext EditTests(
            this Task task,
            File testsFile,
            IContext context) =>
                context.WithUpdated(task, new Task(task) { TestsFileId = testsFile.Id, TestsFile = testsFile });


        [MemberOrder(42)]
        public static IContext ClearTests(
            this Task task,
            IContext context) =>
                context.WithUpdated(task, new Task(task) { TestsFileId = null, TestsFile = null });

        public static string DisableClearTests(this Task task) => task.TestsFileId is null ? "No Tests specified" : null;
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
                Name = title,
                CostInMarks = costInMarks,
                Content = file == null ? null : file.GetResourceAsByteArray(),
            };
            return context.WithNew(hint).WithUpdated(task, new Task(task) { Hints = task.Hints.Append(hint).ToList() });
        }

        public static int Default1AddNewHint(this Task task) =>
            task.Hints.Count + 1;

        [MemberOrder(50)]
        public static IContext UseExistingHintsFrom(
            this Task thisTask,
            Task otherTask,
            IContext context) =>
            context.WithUpdated(thisTask, new Task(thisTask) { Hints = new List<Hint>(otherTask.Hints) });

        #endregion
    }
}
