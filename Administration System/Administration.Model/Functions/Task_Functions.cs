using NakedFramework.Value;
using System.Text;

namespace Model.Functions
{
    public static class Task_Functions
    {
        #region Editing Task properties
        [Edit]
        public static IContext EditNumber(
            this Task task,
            int number,
            IContext context) =>
                context.WithUpdated(task, new(task) { Number = number });

        [Edit]
        public static IContext EditMaxMarks(
            this Task task,
            int maxMarks,
            IContext context) =>
                context.WithUpdated(task, new(task) { MaxMarks = maxMarks });

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

        #region Associated files
        #region Description
        [MemberOrder(20)]
        public static IContext AddDescription(
            this Task task,
            File file,
            IContext context) =>
                    context.WithUpdated(task,
                    new(task)
                    {
                        DescriptionFileId = file.Id,
                        DescriptionFile = file,
                    });

        public static string ValidateAddDescription(
          this Task task,
          File file) =>
            file.ValidateContentType(ContentType.Description);
            
        public static string DisableAddDescription(this Task task) =>
            task.DescriptionFileId is null ? null : "Either go to Description file and reload/edit it, or Clear Description to create a new file here.";

        [MemberOrder(22)]
        public static IContext ClearDesciption(
            this Task task,
            IContext context) =>
                context.WithUpdated(task, new Task(task) { DescriptionFileId = null, DescriptionFile = null });


        public static string DisableClearDescription(this Task task) => task.DescriptionFileId is null ? "No Description specified" : null;
        #endregion

        #region Task Specific Hidden Code
        [MemberOrder(30)]
        public static IContext AddTaskSpecificHiddenCode(
            this Task task,
            File file,
            IContext context) =>
                context.WithUpdated(task,
                            new(task)
                            {
                                HiddenCodeFileId = file.Id,
                                HiddenCodeFile = file,
                            });

        public static string ValidateAddTaskSpecificHiddenCode(
            this Task task,
            File file) =>
            file.ValidateContentType(ContentType.HiddenCode);

        public static string DisableOverrideProjectHiddenCode(this Task task) =>
            task.HiddenCodeFileId is null ? null : "Either go to Hidden Code file and reload/edit it, or Clear Hidden Code to create a new file here.";

        [MemberOrder(32)]
        public static IContext ClearTaskSpecificHiddenCode(
     this Task task,
     IContext context) =>
         context.WithUpdated(task, new Task(task) { HiddenCodeFileId = null, HiddenCodeFile = null });


        public static bool HideClearTaskSpecificHiddenCode(this Task task) => task.HiddenCodeFileId is null;

        #endregion

        #region Task Specific Tests
        [MemberOrder(40)]
        public static IContext AddTaskSpecificTests(
            this Task task,
            File file,
            IContext context) =>
                context.WithUpdated(task,
                            new(task)
                            {
                                TestsFileId = file.Id,
                                TestsFile = file,
                            });

        public static string ValidateAddTaskSpecificTests(
            this Task task,
            File file) =>
                file.ValidateContentType(ContentType.Tests);

        public static bool HideAddTaskSpecificTests(this Task task) =>
            task.TestsFileId == null;

        [MemberOrder(42)]
        public static IContext ClearTaskSpecificTests(
this Task task,
IContext context) =>
 context.WithUpdated(task, new Task(task) { HiddenCodeFileId = null, HiddenCodeFile = null });


        public static bool HideClearTaskSpecificTests(this Task task) => task.HiddenCodeFileId is null;

        #endregion

        #endregion

        #region Hints 
        [MemberOrder(50)]
        public static IContext AddNewHint(
            this Task task,
            int number,
            [DefaultValue(1)] int costInMarks,
            IContext context)
        {
            var name = $"{task} Hint {number}";
            var file = new File()
            {
                Name = name + ".html",
                ContentType = ContentType.Hint,
                AuthorId = task.Project.AuthorId,
                Mime = "text/html",
                LanguageId = null,
                Content = new byte[0]
            };
            var hint = new Hint
            {
                Number = number,
                Name = name,
                CostInMarks = costInMarks,
                File = file
            };
            return context.WithNew(file).WithNew(hint).WithUpdated(task, new Task(task) { Hints = task.Hints.Append(hint).ToList() });
        }

        public static int Default1AddNewHint(this Task task) =>
            task.Hints.Count + 1;

        [MemberOrder(50)]
        public static IContext UseExistingHintsFrom(
            this Task thisTask,
            Task otherTask,
            IContext context) =>
            context.WithUpdated(thisTask, new Task(thisTask) { Hints = new List<Hint>(otherTask.Hints) });

        internal static Hint GetHintNo(this Task task, int hintNo) => task.Hints.Single(h => h.Number == hintNo);

        #endregion

        #region Helpers
        public static bool CodeCarriedForwardToNextTask(this Task task) => !task.NextTaskClearsFunctions;

        public static bool HasTests(this Task task) => task.Tests is not null;
        #endregion
    }
}
