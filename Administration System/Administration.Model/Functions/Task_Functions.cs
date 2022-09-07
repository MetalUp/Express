
namespace Model.Functions
{


    public static class Task_Functions
    {
        #region Editing
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
        public static IContext EditDescription(
            this Task task,
            string description,
            IContext context) =>
                context.WithUpdated(task, new(task) { Description = description });

        [Edit]
        public static IContext EditMaxMarks(
            this Task task,
            int maxMarks,
            IContext context) =>
                context.WithUpdated(task, new(task) { MaxMarks = maxMarks });

        [Edit]
        public static IContext EditHints(
            this Task task,
            [RegEx(@"[A-Za-z0-9_]+.[A-Za-z0-9_]+(\s*,\s*[A-Za-z0-9_]+.[A-Za-z0-9_]+)*\s*")] string hints,
            IContext context) =>
                context.WithUpdated(task, new(task) { Hints = hints });

        [Edit]
        public static IContext EditHintCosts(
            this Task task,
            [RegEx(@"[0-9]+(\s*,\s*[0-9]+)*\s*")] string hintCosts,
            IContext context) =>
                context.WithUpdated(task, new(task) { HintCosts = hintCosts });

        private const string filePath = "";

        [Edit]
        public static IContext EditReadyMadeFunctions(
            this Task task,
            [RegEx(filePath)] string readyMadeFunctions,
            IContext context) =>
                context.WithUpdated(task, new(task) { ReadyMadeFunctions = readyMadeFunctions });

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
        public static IContext EditTests(
            this Task task,
            [RegEx(filePath)] string tests,
            IContext context) =>
                context.WithUpdated(task, new(task) { Tests = tests });

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
        public static IContext EditNextTaskDoesNotClearFunctions(
          this Task task,
          bool nextTaskDoesNotClearFunctions,
          IContext context) =>
            context.WithUpdated(task, new(task) { NextTaskClearsFunctions = nextTaskDoesNotClearFunctions });

        #endregion

        #region Assigning
        //public static IContext AssignTaskToGroup(this Task task, Group group, DateTime dueBy, IContext context) =>
        // group.Students.Aggregate(context, (c, s) => c.WithNew(NewAssignment(task, s, dueBy, User_MenuFunctions.Me(context))));

        //public static IList<Group> Choices1AssignTaskToGroup(IContext context) =>
        //    Group_MenuFunctions.MyGroups(context);


        //Need autocomplete for group & default for assignedBy

        //public static IContext AssignTaskToStudent(this Task task, Student student, DateTime dueBy, IContext context) =>
        //        context.WithNew(NewAssignment(task, student, dueBy, User_MenuFunctions.Me(context)));

        ////Need autocomplete for student & default for assignedBy
        //private static Assignment NewAssignment(Task task, User student, DateTime dueBy, User assignedBy)
        //{
        //    return new Assignment()
        //    {
        //        Task = task,
        //        AssignedTo = student,
        //        AssignedById = assignedBy.Id,
        //        DueBy = dueBy,
        //        Status = ActivityType.Assigned
        //    };
        //}
        #endregion

    }
}
