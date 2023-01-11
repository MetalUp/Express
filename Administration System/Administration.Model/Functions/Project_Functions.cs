using NakedFramework.Value;
using System.Text;

namespace Model.Functions
{


    public static class Project_Functions
    {
        #region Display

        public static bool HideLink(this Project project, IContext context) => !project.IsAssignedToMe(context);

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
        public static IContext EditDescription(
            this Project proj,
            [MultiLine(10)] string description,
            IContext context) =>
             context.WithUpdated(proj, new(proj) { Description = description });


        [Edit]
        public static IContext EditCommonHiddenCodeFile(
            this Project project,
            [Optionally] File commonHiddenCodeFile,
            IContext context) =>
                context.WithUpdated(project, new Project(project)
                {
                    CommonHiddenCodeFileId = commonHiddenCodeFile.Id,
                    CommonHiddenCodeFile = commonHiddenCodeFile
                });


        public static string ValidateEditCommonHiddenCodeFile(this Project project, File commonHiddenCodeFile) =>
            ValidateLanguageAndContentType(project, commonHiddenCodeFile, ContentType.HiddenCode);

        internal static string ValidateLanguageAndContentType(Project project, File commonHiddenCodeFile, ContentType required) =>
    commonHiddenCodeFile.LanguageId == project.LanguageId &&
    commonHiddenCodeFile.ContentType == required
    ? null : $"Either the programming language or content type of the file is incompatible with this property.";

        [Edit]
        public static IContext EditCommonTestsFile(
            this Project project,
            [Optionally] File commonTestsFile,
            IContext context) =>
                context.WithUpdated(project, new Project(project)
                {
                    CommonTestsFileId = commonTestsFile.Id,
                    CommonTestsFile = commonTestsFile
                });

        public static string ValidateEditCommonTestsFile(this Project project, File commonHiddenCodeFile) =>
                ValidateLanguageAndContentType(project, commonHiddenCodeFile, ContentType.Tests);

        #endregion

        #region Common files

        #region Hidden Code
        [MemberOrder(30)]
        public static IContext AddCommonHiddenCodeFromFile(
            this Project proj,
            FileAttachment file,
            IContext context)
        {
            var author = Users.Me(context);
            var f = new File()
            {
                Name = file.Name,
                Content = file.GetResourceAsByteArray(),
                ContentType = ContentType.HiddenCode,
                LanguageId = proj.LanguageId,
                Language = proj.Language,
                Mime = "text/plain",
                AuthorId = author.Id,
                Author = author,
            };
            return context
                .WithNew(f)
                .WithUpdated(proj,
                    new(proj)
                    {
                        CommonHiddenCodeFileId = f.Id,
                        CommonHiddenCodeFile = f,
                    });
        }

        public static string ValidateAddCommonAddHiddenCodeFromFile(
            this Project proj,
            FileAttachment file) =>
            proj.ValidateFileExtension(file);

        internal static string ValidateFileExtension(this Project proj, FileAttachment file) =>
            file.Name.EndsWith(proj.Language.FileExtension) ? null : $"File's name must have extension. {proj.Language.FileExtension}";


        public static string DisableAddCommonHiddenCodeFromFile(this Project proj) =>
    proj.CommonHiddenCodeFileId is null ? null : "Either go to Commoon Hidden Code file and reload/edit it, or clear Common Hidden Code to create a new file here.";
        #endregion

        #region Tests
        [MemberOrder(40)]
        public static IContext AddCommonTestsFromFile(
            this Project proj,
            FileAttachment file,
            IContext context)
        {
            var author = Users.Me(context);
            var f = new File()
            {
                Name = file.Name,
                Content = file.GetResourceAsByteArray(),
                ContentType = ContentType.Tests,
                LanguageId = proj.LanguageId,
                Language = proj.Language,
                Mime = "text/plain",
                AuthorId = author.Id,
                Author = author
            };
            return context
                .WithNew(f)
                .WithUpdated(proj,
                    new(proj)
                    {
                        CommonTestsFileId = f.Id,
                        CommonTestsFile = f,
                    });
        }

        public static string ValidateAddCommonTestsFromFile(
            this Project proj,
            FileAttachment file) =>
                proj.ValidateFileExtension(file);


        public static string DisableAddCommonTestsFromFile(this Project proj) =>
    proj.CommonTestsFileId is null ? null : "Either go to Common Tests file and reload/edit it, or clear Common Tests to create a new file here.";



        #endregion

        #region Custom Wrapper
        [MemberOrder(30)]
        public static IContext AddCustomWrapperCode(
            this Project proj,
            FileAttachment file,
            IContext context)
        {
            var author = Users.Me(context);
            var f = new File()
            {
                Name = file.Name,
                Content = file.GetResourceAsByteArray(),
                ContentType = ContentType.WrapperCode,
                LanguageId = proj.LanguageId,
                Language = proj.Language,
                Mime = "text/plain",
                AuthorId = author.Id,
                Author = author,
            };
            return context
                .WithNew(f)
                .WithUpdated(proj,
                    new(proj)
                    {
                        WrapperFileId = f.Id,
                        WrapperFile = f,
                    });
        }

        public static string ValidateAddCustomWrapperCode(
            this Project proj,
            FileAttachment file) =>
            file.Name.EndsWith("txt") ? null : $"File's name must have extension '.txt'";

        public static string DisableAddCustomWrapperCode(this Project proj) =>
    proj.CommonHiddenCodeFileId is null ? null : "Either go to file and reload/edit it, or Clear Custom Wrapper Code to create a new file here.";

        public static IContext ClearCustomWrapperCode(
             this Project proj,
             FileAttachment file,
             IContext context) =>
                context.WithUpdated(proj,
                new(proj)
                {
                    WrapperFileId = null,
                    WrapperFile = null,
                });

        public static bool HideClearCustomWrapperCode(this Project proj) =>
            proj.WrapperFileId == null;

        #endregion

        #region Custom RegEx Rules
        [MemberOrder(30)]
        [Named("Add Custom RegEx Rules")]
        public static IContext AddCustomRegExRules(
            this Project proj,
            FileAttachment file,
            IContext context)
        {
            var author = Users.Me(context);
            var f = new File()
            {
                Name = file.Name,
                Content = file.GetResourceAsByteArray(),
                ContentType = ContentType.RexExRules,
                LanguageId = proj.LanguageId,
                Language = proj.Language,
                Mime = "text/plain",
                AuthorId = author.Id,
                Author = author,
            };
            return context
                .WithNew(f)
                .WithUpdated(proj,
                    new(proj)
                    {
                        RegExRulesFileId = f.Id,
                        RegExRulesFile = f,
                    });
        }

        public static string ValidateAddCustomRegExRules(
            this Project proj,
            FileAttachment file) =>
            file.Name.EndsWith("txt") ? null : $"File's name must have extension '.txt'";

        public static string DisableAddCustomRegExRules(this Project proj) =>
    proj.CommonHiddenCodeFileId is null ? null : "Either go to file and reload/edit it, or Clear Custom RegEx Rules to create a new file here.";

        public static IContext ClearCustomRegExRules(
             this Project proj,
             FileAttachment file,
             IContext context) =>
                context.WithUpdated(proj,
                new(proj)
                {
                    RegExRulesFileId = null,
                    RegExRulesFile = null,
                });

        public static bool HideClearOverriddenRegExRules(this Project proj) =>
            proj.RegExRulesFileId == null;

        #endregion

        #endregion

        #region Assignment

        [MemberOrder(5)]
        public static (Assignment, IContext) AssignToMe(this Project project, IContext context) =>
        Assignments.NewAssignmentToIndividual(Users.Me(context), project, context.Today(), context);

        public static string DisableAssignToMe(this Project project, IContext context) =>
            project.IsAssignedToMe(context) ? "Project is already assigned to you" : null;

        [MemberOrder(10)]
        public static IContext AssignToIndividual(this Project project, User singleUser, DateTime dueBy, IContext context) =>
            Assignments.NewAssignmentToIndividual(singleUser, project, dueBy, context).Item2;

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

        internal static bool IsAssignedToMe(this Project project, IContext context)
        {
            var pid = project.Id;
            return Assignments.AssignmentsForCurrentUser(pid, context).Any();
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
                Hints = new List<Hint>(copyFrom.Hints)
            };
            var p2 = new Project(project)
            {
                Tasks = project.Tasks.Append(t).ToList()
            };
            var context2 = context.WithNew(t).WithUpdated(project, p2);
            var context3 = previousTask is null ? context2 : context2.WithUpdated(previousTask,
                new Task(previousTask) { NextTask = t });
            return context3;
        }

        public static Task Default1CopyNextTaskFromAnotherProject(this Project project) =>
            project.Tasks.LastOrDefault();

        #endregion

        #region Create Task
        [MemberOrder(50)]
        public static IContext CreateTask(
            this Project project,
            int number,
            [Optionally] Task previousTask,
            IContext context)
        {
            var t = new Task
            {
                ProjectId = project.Id,
                Project = project,
                Number = number,
                PreviousTaskId = previousTask is null ? null : previousTask.Id,
                PreviousTask = previousTask,
            };
            var updatedPrevious = previousTask is null ? null :
                new Task(previousTask) { NextTask = t };
            var context2 = updatedPrevious is null ? context : context.WithUpdated(previousTask, updatedPrevious);
            return context2.WithNew(t);
        }

        public static int Default1CreateTask(
            this Project project) =>
            project.Tasks.Count == 0 ? 1 : project.Tasks.Last().Number + 1;

        public static Task Default2CreateTask(
            this Project project) =>
            project.Tasks.LastOrDefault();
        #endregion
    }
}
