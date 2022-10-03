﻿using NakedFramework.Value;
using System.Text;

namespace Model.Functions
{


    public static class Task_Author_Functions
    {
        #region ViewModel required functions
        public static string[] DeriveKeys(this TaskAuthorView tav) =>  new[] { tav.Task.Id.ToString() };

        public static TaskAuthorView CreateFromKeys(string[] keys, IContext context)
        {
            int customerId = int.Parse(keys[0]);
            return new TaskAuthorView(context.Instances<Task>().Single(t => t.Id == customerId));
        }
        #endregion

        #region Editing Task properties
        [Edit]
        public static IContext EditTitle(
            this TaskAuthorView tav,
            string title,
            IContext context) =>
                context.WithUpdated(tav.Task, new(tav.Task) { Title = title });

        [Edit]
        public static IContext EditLanguage(
            this TaskAuthorView tav,
            ProgrammingLanguage language,
            IContext context) =>
                context.WithUpdated(tav.Task, new(tav.Task) { Language = language });

        [Edit]
        public static IContext EditMaxMarks(
            this TaskAuthorView tav,
            int maxMarks,
            IContext context) =>
                context.WithUpdated(tav.Task, new(tav.Task) { MaxMarks = maxMarks });

        [Edit]
        public static IContext EditPasteExpression(
            this TaskAuthorView tav,
            bool pasteExpression,
            IContext context) =>
                context.WithUpdated(tav.Task, new(tav.Task) { PasteExpression = pasteExpression });

        [Edit]
        public static IContext EditPasteFunctions(
            this TaskAuthorView tav,
            bool pasteFunctions,
            IContext context) =>
                context.WithUpdated(tav.Task, new(tav.Task) { PasteFunctions = pasteFunctions });

        [Edit]
        public static IContext EditPreviousTask(
            this TaskAuthorView tav,
            Task previousTask,
            IContext context) =>
                context.WithUpdated(tav.Task, new(tav.Task) { PreviousTask = previousTask });

        public static string ValidateEditPreviousTask(
            this TaskAuthorView tav,
            Task previousTask,
            IContext context) =>
                previousTask.Id == tav.Task.Id ? "Cannot specify a task as its own Previous Task" :
                previousTask.Language == tav.Task.Language ? "" : "Previous Task must specify the same Language";

        [Edit]
        public static IContext EditNextTask(
           this TaskAuthorView tav,
           Task nextTask,
           IContext context) =>
            context.WithUpdated(tav.Task, new(tav.Task) { NextTask = nextTask });

        public static string ValidateEditNextTask(
            this TaskAuthorView tav,
            Task nextTask,
            IContext context) =>
                nextTask.Id == tav.Task.Id ? "Cannot specify a task as its own Next Task" :
                nextTask.Language == tav.Task.Language ? "" : "Next Task must specify the same Language";

        [Edit]
        public static IContext EditNextTaskClearsFunctions(
          this TaskAuthorView tav,
          bool nextTaskClearsFunctions,
          IContext context) =>
            context.WithUpdated(tav.Task, new(tav.Task) { NextTaskClearsFunctions = nextTaskClearsFunctions });


        [Edit]
        public static IContext EditStatus(
          this TaskAuthorView tav,
          TaskStatus status,
          IContext context) =>
            context.WithUpdated(tav.Task, new(tav.Task) { Status = status });

        #endregion

        #region FileAttachments
        #region Description
        [MemberOrder(21)]
        public static IContext LoadDescriptionFromFile(
            this TaskAuthorView tav,
            FileAttachment file,
            IContext context) =>
                SaveDescription(tav, file.GetResourceAsByteArray(), file.Name, file.MimeType, context);

        internal static IContext SaveDescription(
            this TaskAuthorView tav,
            byte[] bytes,
            string name,
            string mimeType,
            IContext context) =>
                context.WithUpdated(tav.Task,
                    new(tav.Task)
                    {
                        DescContent = bytes,
                        DescName = name,
                        DescMime = mimeType,
                    });


        [MemberOrder(22)]
        public static IContext ClearDescription(
            this TaskAuthorView tav,
            bool confirm,
            IContext context) =>
            context.WithUpdated(tav.Task,
                new(tav.Task)
                {
                    DescContent = null,
                    DescName = null,
                    DescMime = null,
                });

        public static string ValidateClearDescription(this TaskAuthorView tav, bool confirm) =>
            confirm ? null : "Confirm must be selected";
        #endregion

        #region Hidden Functions
        [MemberOrder(30)]
        public static IContext LoadHiddenFunctionsFromFile(
            this TaskAuthorView tav,
            FileAttachment file,
            IContext context) =>
            SaveHiddenFunctions(tav, file.GetResourceAsByteArray(), file.Name, file.MimeType, context);

        [MemberOrder(31)]
        public static IContext EnterHiddenFunctionsAsString(
            this TaskAuthorView tav,
            [MultiLine(20)] string description,
            IContext context) =>
                 SaveHiddenFunctions(tav, Encoding.ASCII.GetBytes(description),
                     $"HiddenFunctions{tav.Task.LanguageAsFileExtension()}",
                     "text/plain",
                     context);

        internal static IContext SaveHiddenFunctions(
            this TaskAuthorView tav,
            byte[] bytes,
            string name,
            string mimeType,
            IContext context) =>
                context.WithUpdated(tav.Task,
                    new(tav.Task)
                    {
                        RMFContent = bytes,
                        RMFName = name,
                        RMFMime = mimeType,
                    });
    

        [MemberOrder(32)]
        public static IContext ClearHiddenFunctions(
            this TaskAuthorView tav,
            bool confirm,
            IContext context) =>
            context.WithUpdated(tav.Task,
                new(tav.Task)
                {
                    RMFContent = null,
                    RMFName = null,
                    RMFMime = null,
                });

        public static string ValidateClearReadyMadeFunctions(this Task task, bool confirm) =>
            confirm ? null : "Confirm must be selected";
        #endregion

        #region Tests
        [MemberOrder(40)]
        public static IContext LoadTestsFromFile(
            this TaskAuthorView tav,
            FileAttachment file,
            IContext context) =>
            SaveTests(tav, file.GetResourceAsByteArray(), file.Name, file.MimeType, context);

        [MemberOrder(41)]
        public static IContext EnterTestsAsString(
            this TaskAuthorView tav,
            [MultiLine(20)] string description,
            IContext context) =>
                 SaveTests(tav, Encoding.ASCII.GetBytes(description),
                     $"Tests{tav.Task.LanguageAsFileExtension()}",
                     "text/plain",
                     context);


        internal static IContext SaveTests(
            this TaskAuthorView tav,
            byte[] bytes,
            string name,
            string mimeType,
            IContext context) =>
                context.WithUpdated(tav.Task,
                    new(tav.Task)
                    {
                        TestsContent = bytes,
                        TestsName = name,
                        TestsMime = mimeType,
                    });

        [MemberOrder(42)]
        public static IContext ClearTests(
            this TaskAuthorView tav,
            bool confirm,
            IContext context) =>
                context.WithUpdated(tav.Task,
                    new(tav.Task)
                    {
                        TestsContent = null,
                        TestsName = null,
                        TestsMime = null,
                    });

        public static string ValidateClearTests(this TaskAuthorView tav, bool confirm) =>
            confirm ? null : "Confirm must be selected";
        #endregion

        #endregion

        #region Hints 
        [MemberOrder(50)]
        public static IContext AddHint(
            this TaskAuthorView tav,
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
                FileName = file == null ? null : file.Name,
                FileMime = file == null ? null : file.MimeType,
                TaskId = tav.Task.Id,
                Task = tav.Task
            };
            return context.WithNew(hint);
        }

        public static int Default1AddHint(this TaskAuthorView tav) =>
            tav.Task.Hints.Count + 1;

        [MemberOrder(55)]
        public static IContext RemoveHint(this TaskAuthorView tav, Hint hint, IContext context) =>
            context.WithDeleted(hint);

        public static List<Hint> Choices1RemoveHint(this TaskAuthorView tav, Hint hint, IContext context) =>
            tav.Task.Hints.ToList();
        #endregion

        #region Copy
        public static (Task, IContext) DuplicateTaskForNewLanguage(this TaskAuthorView tav,
            ProgrammingLanguage newLanguage,
            IContext context
            )
        {
            var newHints = tav.Task.Hints.Select(h => new Hint(h) { Id = 0}).ToList();
            var context2 = newHints.Aggregate(context, (c, h) => c.WithNew(h));
            var task2 = new Task(tav.Task)
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
    }
}
