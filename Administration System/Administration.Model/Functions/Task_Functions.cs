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
            IContext context)
        {
            var author = Users.Me(context);
            var f = new File() { Name = file.Name, Content = file.GetResourceAsByteArray(), Mime = "text/html", AuthorId = author.Id, Author = author };
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

        [MemberOrder(21)]
        public static IContext ClearDesciption(
    this Task task,
    IContext context) =>
    context.WithUpdated(task, new Task(task) { DescriptionFileId = null, DescriptionFile = null });
        #endregion

        #region Hidden Functions
        [MemberOrder(30)]
        public static IContext LoadHiddenFunctionsFromFile(
            this Task task,
            FileAttachment file,
            IContext context) =>
            SaveHiddenFunctionsAsFile(task, file.Name, file.GetResourceAsByteArray(), context);

        public static IContext LoadHiddenFunctionsAsString(
    this Task task,
    [MultiLine(10)] string content,
    IContext context) =>
    SaveHiddenFunctionsAsFile(task, $"HiddenFunctions{task.Project.LanguageAsFileExtension()}", Encoding.ASCII.GetBytes(content), context);


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
        #endregion

        #region Tests
        [MemberOrder(40)]
        public static IContext LoadTestsFromFile(
            this Task task,
            FileAttachment file,
            IContext context) =>
                SaveTestsAsFile(task, file.Name, file.GetResourceAsByteArray(), context);

        public static IContext LoadTestsAsString(
            this Task task,
            [MultiLine(10)] string content,
            IContext context) =>
                SaveTestsAsFile(task, $"Tests{task.Project.LanguageAsFileExtension()}", Encoding.ASCII.GetBytes(content), context);


        private static IContext SaveTestsAsFile(
            this Task task,
            string name,
            byte[] content,
            IContext context)
        {
            var author = Users.Me(context);
            var f = new File() { Name = name, Content = content, Mime ="text/plain", AuthorId = author.Id, Author = author };
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


        [MemberOrder(41)]
        public static IContext ClearTests(
            this Task task,
            IContext context) =>
            context.WithUpdated(task, new Task(task) { TestsFileId = null, TestsFile = null });
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
