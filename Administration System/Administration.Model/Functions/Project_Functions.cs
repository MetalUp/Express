using NakedFramework.Value;
using System.Text;

namespace Model.Functions
{


    public static class Project_Functions
    {
        #region Display

        public static bool HideLink(this Project project) => !project.Tasks.Any();

        #endregion

        #region Edits
        [Edit]
        public static IContext EditTitle(
             this Project proj,
             string title,
             IContext context) =>
                 context.WithUpdated(proj, new(proj) { Title = title });

        [Edit]
        public static IContext EditStatus(
             this Project proj,
             ProjectStatus status,
             IContext context) =>
                 context.WithUpdated(proj, new(proj) { Status = status });

        [Edit]
        public static IContext EditLanguage(
             this Project proj,
             Language language,
             IContext context) =>
                 context.WithUpdated(proj, new(proj) { Language = language });

        [Edit]
        public static IContext EditPasteExpression(
    this Project proj,
    bool pasteExpression,
    IContext context) =>
        context.WithUpdated(proj, new(proj) { PasteExpression = pasteExpression });

        [Edit]
        public static IContext EditPasteCode(
            this Project proj,
            bool pasteCode,
            IContext context) =>
                context.WithUpdated(proj, new(proj) { PasteCode = pasteCode });

        [Edit]
        public static IContext EditDescription(
            this Project proj,
            [MultiLine(10)] string description,
            IContext context) =>
             context.WithUpdated(proj, new(proj) { Description = description });

        [Edit]
        public static IContext EditCommonHiddenCodeFile(
            this Project project,
            File commonHiddenCodeFile,
            IContext context) =>
                context.WithUpdated(project, new Project(project) { 
                    CommonHiddenCodeFileId = commonHiddenCodeFile.Id,
                    CommonHiddenCodeFile = commonHiddenCodeFile
                });

        [Edit]
        public static IContext EditCommonTestsFile(
            this Project project,
            File commonTestsFile,
            IContext context) =>
                context.WithUpdated(project, new Project(project)
                {
                    CommonTestsFileId = commonTestsFile.Id,
                    CommonTestsFile = commonTestsFile
                });

        #endregion

        #region AssignTo
        [MemberOrder(10)]
        public static IContext AssignToIndividual(this Project project, User singleUser, DateTime dueBy, IContext context) =>
            Assignments.NewAssignmentToIndividual(singleUser, project, dueBy, context);

        [MemberOrder(20)]
        public static IContext AssignToGroup(this Project project, Group group, DateTime dueBy, IContext context) =>
         Assignments.NewAssignmentToGroup(group, project, dueBy, context);


        public static string ValidateAssignTo(this Project project, Group inGroup, bool allInGroup, User singleUser, DateTime dueBy, IContext context) =>
            allInGroup && inGroup is null ? "Must specify a Group" :
               !allInGroup && singleUser is null ? "Must specify a Single User" : null;

        public static List<Group> Choices1AssignTo(this Project project, [Optionally] Group inGroup, bool allInGroup, [Optionally] User singleUser, DateTime dueBy, IContext context) =>
            Groups.AllOurGroups(context).ToList();
        #endregion

        #region internal functions
        internal static bool IsAssignable(this Project project) => project.Status == ProjectStatus.Assignable;

        internal static bool IsAssignedToCurrentUser(this Project project, IContext context)
        {
            var myId = Users.Me(context).Id;
            var pid = project.Id;
            return context.Instances<Assignment>().Any(a => a.AssignedToId == myId && a.ProjectId == pid);
        }
        #endregion

        #region Copying
        [MemberOrder(30)]
        public static (Project, IContext) CopyProjectForNewLanguage(
            this Project project,
            Language newLanguage,
            IContext context
            )
        {
            var author = Users.Me(context);
            var p = new Project(project)
            {
                Id = 0, //Because it is a new object
                Language = newLanguage,
                Tasks = new List<Task>()
            };
            return (p, context.WithNew(p));
        }

        [MemberOrder(40)]
        public static IContext CopyNextTaskFromAnotherProject(
            this Project project,
            [Optionally][Named("Previous Task (if relevant)")] Task previousTask,
            Task copyFrom,
            IContext context
            )
        {
            var t = new Task(copyFrom)
            {
                Id = 0, //Because it is a new object
                PreviousTaskId = previousTask is null ? null : previousTask.Id,
                PreviousTask = previousTask,
                NextTaskId = null,
                NextTask = null,
                HiddenCodeFileId = project.CommonHiddenCodeFileId,
                HiddenCodeFile = project.CommonHiddenCodeFile,
                TestsFileId = project.CommonTestsFileId,
                TestsFile = project.CommonTestsFile,
                Hints = new List<Hint>(copyFrom.Hints)
            };
            var p2 = new Project(project)
            {
                Tasks = project.Tasks.Append(t).ToList()
            };
            var context2 = context.WithNew(t).WithUpdated(project, p2);
            var context3 = previousTask is null ? context2 : context2.WithUpdated(previousTask,
                new Task(previousTask) {NextTask = t });
            return context3;
        }

        public static Task Default1CopyNextTaskFromAnotherProject(this Project project) =>
            project.Tasks.LastOrDefault();

        #endregion

        #region Create Task
        [MemberOrder(50)]
        public static IContext CreateTask(
            this Project project,
            string title,
            [Optionally] Task previousTask,
            IContext context)
        {
            var t = new Task
            {
                ProjectId = project.Id,
                Project = project,
                Name = title,
                PreviousTaskId = previousTask is null ? null : previousTask.Id,
                PreviousTask = previousTask,
                HiddenCodeFileId = previousTask is null ? null : previousTask.HiddenCodeFileId,
                HiddenCodeFile = previousTask is null ? null : previousTask.HiddenCodeFile,
                TestsFileId = previousTask is null ? null : previousTask.TestsFileId,
                TestsFile = previousTask is null ? null : previousTask.TestsFile,
            };
            var updatedPrevious = previousTask is null ? null :
                new Task(previousTask) { NextTask = t };
            var context2 = updatedPrevious is null ? context : context.WithUpdated(previousTask, updatedPrevious);
            return context2.WithNew(t);
        }

        public static Task Default2CreateTask(
            this Project project) =>
            project.Tasks.LastOrDefault();
        #endregion
    }
}
