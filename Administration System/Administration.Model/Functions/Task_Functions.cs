﻿namespace Model.Functions
{
    public static class Task_Functions
    {
        #region Display
        public static bool HideHiddenCodeFile(this Task task) => task.HiddenCodeFileId == null;

        public static bool HideTestsFile(this Task task) => task.TestsFileId == null;

        public static bool HideTestsRunOnClient(this Task task) => !task.TestsRunOnClient;

        public static bool HideWrapperFile(this Task task) => task.WrapperFileId == null;

        public static bool HideRegExRulesFile(this Task task) => task.RegExRulesFileId == null;

        #endregion

        #region Editing Task properties
        [Edit]
        public static IContext EditNumber(
            this Task task,
            int number,
            IContext context) =>
                context.WithUpdated(task, new(task) { Number = number });

        [Edit]
        public static IContext EditSummary(
    this Task task,
    string summary,
    IContext context) =>
        context.WithUpdated(task, new(task) { Summary = summary });

        [Edit]
        public static IContext EditMaxMarks(
            this Task task,
            int maxMarks,
            IContext context) =>
                context.WithUpdated(task, new(task) { MaxMarks = maxMarks });

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

        #region Description
        [Edit]
        public static IContext EditDescriptionFile(
            this Task task,
            [Optionally] File file,
            IContext context) =>
                    context.WithUpdated(task,
                    new(task)
                    {
                        DescriptionFileId = file.Id,
                        DescriptionFile = file,
                    });

        public static string ValidateEditDescriptionFile(
              this Task task,
              File file) =>
                    file == null ? null : file.ValidateContentType(ContentType.Description);

        #endregion


        #endregion

        #region Hints 
        [MemberOrder(10)]
        public static IContext AddNewHint(
            this Task task,
            int number,
            [DefaultValue(1)] int costInMarks,
            IContext context)
        {
            var name = $"{task.Project.Title} {task.Number}-{number}";
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
                Number = number,
                CostInMarks = costInMarks,
                File = file
            };
            return context.WithNew(file).WithNew(hint).WithUpdated(task, new Task(task) { Hints = task.Hints.Append(hint).ToList() });
        }

        public static int Default1AddNewHint(this Task task) =>
            task.Hints.Count + 1;

        [MemberOrder(15)]
        public static IContext UseExistingHintsFrom(
            this Task thisTask,
            Task otherTask,
            IContext context) =>
            context.WithUpdated(thisTask, new Task(thisTask) { Hints = new List<Hint>(otherTask.Hints) });

        internal static Hint GetHintNo(this Task task, int hintNo) => task.Hints.Single(h => h.Number == hintNo);

        #endregion

        #region Associated files

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

        public static bool HideOverrideProjectHiddenCode(this Task task) => task.HiddenCodeFileId != null;

        [MemberOrder(31)]
        public static IContext ClearTaskSpecificHiddenCode(
             this Task task,
             IContext context) =>
                 context.WithUpdated(task, new Task(task) { HiddenCodeFileId = null, HiddenCodeFile = null });

        public static bool HideClearTaskSpecificHiddenCode(this Task task) => task.HiddenCodeFileId == null;

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

        public static bool HideAddTaskSpecificTests(this Task task) => task.TestsFileId != null;

        [MemberOrder(41)]
        public static IContext ClearTaskSpecificTests(
            this Task task,
            IContext context) =>
             context.WithUpdated(task, new Task(task) { TestsFileId = null, TestsFile = null });


        public static bool HideClearTaskSpecificTests(this Task task) => task.TestsFileId == null;

        #endregion

        #region Task Specific Wrapper Code
        [MemberOrder(50)]
        public static IContext AddTaskSpecificWrapperCode(
            this Task task,
            File file,
            IContext context) =>
                context.WithUpdated(task,
                            new(task)
                            {
                                WrapperFileId = file.Id,
                                WrapperFile = file,
                            });

        public static string ValidateAddTaskSpecificWrapperCode(
    this Task task,
        File file) =>
        file.ValidateContentType(ContentType.WrapperCode);

        public static bool HideAddTaskSpecificWrapperCode(this Task task) => task.WrapperFileId != null;

        [MemberOrder(51)]
        public static IContext ClearTaskSpecificWrapperCode(
            this Task task,
            IContext context) =>
             context.WithUpdated(task, new Task(task) { WrapperFileId = null, WrapperFile = null });


        public static bool HideClearTaskSpecificWrapperCode(this Task task) => task.WrapperFileId == null;

        #endregion

        #region Task Specific RegEx Rules
        [MemberOrder(60)]
        public static IContext AddTaskSpecificRegExRules(
            this Task task,
            File file,
            IContext context) =>
                context.WithUpdated(task,
                            new(task)
                            {
                                RegExRulesFileId = file.Id,
                                RegExRulesFile = file,
                            });

        public static IEnumerable<File> Choices1AddTaskSpecificRegExRuless(this Task task, IContext context)
        {
            string langId = task.Project.LanguageId;
            return context.Instances<File>().Where(f => f.ContentType == ContentType.RegExRules && f.LanguageId == langId);
        }

        public static bool HideAddTaskSpecificRegExRules(this Task task) => task.RegExRulesFileId != null;

        [MemberOrder(61)]
        public static IContext ClearTaskSpecificRegExRules(
            this Task task,
            IContext context) =>
             context.WithUpdated(task, new Task(task) { RegExRulesFileId = null, RegExRulesFile = null });


        public static bool HideClearTaskSpecificRegExRules(this Task task) => task.RegExRulesFileId == null;

        #endregion
        #endregion

        #region Helpers
        public static bool CodeCarriedForwardToNextTask(this Task task) => !task.NextTaskClearsFunctions;

        public static bool HasTests(this Task task) => task.Tests is not null;
        #endregion

        #region TestsRunOnClient
        public static IContext SpecifyThatTestsRunOnClient(this Task task, IContext context) =>
            context.WithUpdated(task, new(task) { TestsRunOnClient = true });

        public static bool HideSpecifyThatTestsRunOnClient(this Task task) => task.TestsRunOnClient;

        public static IContext ClearTestsRunOnClient(this Task task, IContext context) =>
    context.WithUpdated(task, new(task) { TestsRunOnClient = false });

        public static bool HideClearTestsRunOnClient(this Task task) => !task.TestsRunOnClient;
        #endregion
    }
}
