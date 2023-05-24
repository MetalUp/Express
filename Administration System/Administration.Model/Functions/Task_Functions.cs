namespace Model.Functions
{
    public static class Task_Functions
    {
        #region Display
        public static bool HideTestsRunOnClient(this Task task) => !task.TestsRunOnClient;

        public static bool HideWrapperFile(this Task task) => task.WrapperFileId == null;

        public static bool HideRegExRulesFile(this Task task) => task.RegExRulesFileId == null;

        #endregion

        #region Editing Task properties (for File properties - see below)
        [Edit]
        public static IContext EditNumber(
            this Task task,
            int? number,
            IContext context) =>
                context.WithUpdated(task, new(task) { Number = number });

        [Edit]
        public static IContext EditTitle(
this Task task,
string title,
IContext context) =>
context.WithUpdated(task, new(task) { Title = title });

        [Edit]
        public static IContext EditPreviousTask(
            this Task task,
            [Optionally] Task previousTask,
            IContext context) =>
                context.WithUpdated(task, new(task) { PreviousTask = previousTask });

        public static string ValidateEditPreviousTask(
            this Task task,
            Task previousTask,
            IContext context) =>
                previousTask == null ? null :
                previousTask.Id == task.Id ? "Cannot specify a task as its own Previous Task" :
                previousTask.Language == task.Language ? "" : "Previous Task must specify the same Language";

        [Edit]
        public static IContext EditNextTask(
           this Task task,
           [Optionally] Task nextTask,
           IContext context) =>
            context.WithUpdated(task, new(task) { NextTask = nextTask });

        public static string ValidateEditNextTask(
            this Task task,
            Task nextTask,
            IContext context) =>
                nextTask == null ? null :
                nextTask.Id == task.Id ? "Cannot specify a task as its own Next Task" :
                nextTask.Language == task.Language ? "" : "Next Task must specify the same Language";

        [Edit]
        public static IContext EditNextTaskClearsFunctions(
          this Task task,
          bool nextTaskClearsFunctions,
          IContext context) =>
            context.WithUpdated(task, new(task) { NextTaskClearsFunctions = nextTaskClearsFunctions });


        [Edit]
        public static IContext EditTestsRunOnClient(
        this Task task,
        bool testsRunOnClient,
        IContext context) =>
          context.WithUpdated(task, new(task) { TestsRunOnClient = testsRunOnClient });

        #endregion

        #region Hints 
        #region Add New Hint
        [MemberOrder(10)]
        public static IContext AddNewHint(
            this Task task,
            int hintNumber,
            IContext context)
        {
            var name = $"{task.Project.Title} {task.Number}-{hintNumber}";
            var file = new File()
            {
                Name = name,
                ContentType = ContentType.Hint,
                AuthorId = task.Project.AuthorId,
                LanguageId = null,
                Content = new byte[0]
            };
            var hint = new Hint
            {
                Number = hintNumber,
                File = file
            };
            return context.WithNew(file).WithNew(hint).WithUpdated(task, new Task(task) { Hints = task.Hints.Append(hint).ToList() });
        }

        public static int Default1AddNewHint(this Task task) =>
            task.Hints.Count + 1;
        #endregion

        #region Edit Hint
        [MemberOrder(12)]
        [UrlLink]   //Opens file in Edit Viewer
        public static string EditHint(this Task task, Hint hint) => $"/dashboard/editor/{hint.FileId}";

        public static ICollection<Hint> Choices1EditHint(this Task task) => task.Hints;

        public static Hint Default1EditHint(this Task task) => task.Hints.FirstOrDefault();

        public static bool HideEditHint(this Task task) => task.Hints.Count == 0;
        #endregion

        [MemberOrder(15)]
        public static IContext UseExistingHintsFrom(
            this Task thisTask,
            Task otherTask,
            IContext context) =>
            context.WithUpdated(thisTask, new Task(thisTask) { Hints = new List<Hint>(otherTask.Hints) });

        internal static Hint GetHintNo(this Task task, int hintNo) => task.Hints.Single(h => h.Number == hintNo);
        #endregion

        #region Associated files

        #region Description
        [Edit] //To change the file reference
        public static IContext EditDescriptionFile(
            this Task task,
            [Optionally] File descriptionFile,
            IContext context) =>
                    context.WithUpdated(task,
                    new(task)
                    {
                        DescriptionFile = descriptionFile,
                    });

        public static string ValidateEditDescriptionFile(
              this Task task,
              File file) =>
                    file == null ? null : file.ValidateContentType(ContentType.TaskDescription);

        [MemberOrder(30)]
        [UrlLink]   //Opens file in Edit Viewer
        public static string EditDescription(this Task task) =>
            $"/dashboard/editor/{task.DescriptionFileId}";

        public static bool HideEditDescription(this Task task) =>
            task.DescriptionFileId == null;


        [MemberOrder(32)]
        public static IContext CreateNewDescriptionFile(
            this Task task,
            IContext context)
        {
            var (file, context2) = Files.CreateNewFileAsString(task.Title, ContentType.TaskDescription, null, "TODO", context);
            return context2.WithUpdated(task, new(task) { DescriptionFile = file });
        }

        public static bool HideCreateNewDescriptionFile(this Task task) => task.DescriptionFileId != null;

        #endregion

        #region Task Specific Hidden Code
        [Edit] //To change the file reference
        public static IContext EditHiddenCodeFile(
            this Task task,
            [Optionally] File hiddenCodeFile,
            IContext context) =>
                    context.WithUpdated(task,
                    new(task)
                    {
                        HiddenCodeFile = hiddenCodeFile,
                    });

        public static string ValidateEditHiddenCodeFile(
              this Task task,
              File file) =>
                    file == null ? null : file.ValidateContentType(ContentType.HiddenCode);

        [MemberOrder(40)]
        [UrlLink]   //Opens file in Edit Viewer
        public static string EditHiddenCode(this Task task) => $"/dashboard/editor/{task.HiddenCodeFileId}";

        public static bool HideEditHiddenCode(this Task task) => task.HiddenCodeFileId == null;

        [MemberOrder(42)]
        public static IContext CreateNewHiddenCodeFile(
            this Task task,
            IContext context)
        {
            var (file, context2) = Files.CreateNewFileAsString(task.Title, ContentType.HiddenCode, task.Project.Language, "TODO", context);
            return context2.WithUpdated(task, new(task) { HiddenCodeFile = file });
        }

        public static bool HideCreateNewHiddenCodeFile(this Task task) => task.HiddenCodeFileId != null;
        #endregion

        #region Task Specific Tests
        [Edit]  //To change the file reference
        public static IContext EditTestsFile(
           this Task task,
           [Optionally] File testsFile,
           IContext context) =>
                   context.WithUpdated(task,
                   new(task)
                   {
                       TestsFile = testsFile,
                   });

        public static string ValidateEditTestsFile(
              this Task task,
              File file) =>
                    file == null ? null : file.ValidateContentType(ContentType.Tests);


        [MemberOrder(50)]
        [UrlLink]   //Opens file in Edit Viewer
        public static string EditTests(this Task task) =>
             $"/dashboard/editor/{task.TestsFileId}";

        public static bool HideEditTests(this Task task) =>
            task.TestsFileId == null;


        [MemberOrder(52)]
        public static IContext CreateNewTestsFile(
            this Task task,
            IContext context)
        {
            var (file, context2) = Files.CreateNewFileAsString(task.Title, ContentType.Tests, task.Project.Language, "TODO", context);
            return context2.WithUpdated(task, new(task) { TestsFile = file });
        }

        public static bool HideCreateNewTestsFile(this Task task) => task.TestsFileId != null;

        #endregion
        #endregion

        #region Helpers
        public static bool CodeCarriedForwardToNextTask(this Task task) => !task.NextTaskClearsFunctions;

        public static bool HasTests(this Task task) => task.Tests is not null;
        #endregion
    }
}
