using NakedFramework.Value;

namespace Model.Functions
{


    public static class Task_Functions
    {
        #region Hints
        public static IContext AddNewHint(this Task task, string name, string htmlFile, int costInMarks, IContext context) =>
            context.WithNew(new Hint { Title = name, HtmlFile = htmlFile, CostInMarks = costInMarks, TaskId = task.Id, Task = task });


        #endregion

        #region Assigning
        //public static IContext AssignTaskToGroup(this Task task, Group group, DateTime dueBy, IContext context) =>
        // group.Students.Aggregate(context, (c, s) => c.WithNew(NewAssignment(task, s, dueBy, User_MenuFunctions.Me(context))));

        //public static IList<Group> Choices1AssignTaskToGroup(IContext context) =>
        //    Group_MenuFunctions.MyGroups(context);


        //Need autocomplete for group & default for assignedBy

        //public static IContext AssignTaskToStudent(this Task task, Student student, DateTime dueBy, IContext context) =>
        //        context.WithNew(NewAssignment(task, student, dueBy, User_MenuFunctions.Me(context)));

        public static (Assignment, IContext) AssignToStudent(this Task task, User student, DateTime dueBy, IContext context) =>
            User_Functions.AssignTask(student, task, dueBy, context);


        #endregion

        #region internal functions
        internal static bool IsPublic(this Task task) => task.Status == TaskStatus.Public;

        internal static bool IsAssignable(this Task task) => task.Status != TaskStatus.UnderDevelopment;

        internal static bool IsAssignedToCurrentUser(this Task task, IContext context)
        {
            var myId = UserRepository.Me(context).Id;
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
        public static IContext EditReadyMadeFunctions(
            this Task task,
            string readyMadeFunctions,
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
            string tests,
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
        public static IContext EditNextTaskClearsFunctions(
          this Task task,
          bool nextTaskDoesNotClearFunctions,
          IContext context) =>
            context.WithUpdated(task, new(task) { NextTaskClearsFunctions = nextTaskDoesNotClearFunctions });


        [Edit]
        public static IContext EditStatus(
          this Task task,
          TaskStatus status,
          IContext context) =>
            context.WithUpdated(task, new(task) { Status = status });

        #endregion

        #region FileAttachments

        public static IContext AddOrChangeDescription(
            this Task task, 
            FileAttachment newAttachment, 
            IContext context) =>
                context.WithUpdated(task, 
                    new Task(task) { DescriptionContent = newAttachment.GetResourceAsByteArray()});

        #endregion

        #region Hints 
        public static Hint AddHint(
            this Task task, 
            int number,
            string title, 
            [Optionally] FileAttachment file, 
            [DefaultValue(1)]int costInMarks,
            IContext context)
        {
            throw new NotImplementedException();
        }

        public static int Default1AddHint(this Task task) =>
            task.Hints.Count + 1;

        public static IContext RemoveHint(this Task task, Hint hint, IContext context) =>
            context.WithDeleted(hint);

        public static List<Hint> Choices1RemoveHint(this Task task, Hint hint, IContext context) =>
            task.Hints.ToList();
        #endregion
        #endregion
    }
}
