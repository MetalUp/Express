namespace Model.Functions
{
    public static class Project_Functions
    {
        #region Display

        public static bool HideCommonHiddenCodeFile(this Project project) => project.CommonHiddenCodeFileId == null;

        public static bool HideCommonTestsFile(this Project project) => project.CommonTestsFileId == null;

        public static bool HideWrapperFile(this Project project) => project.WrapperFileId == null;

        public static bool HideRegExRulesFile(this Project project) => project.RegExRulesFileId == null;

        public static bool HideLink(this Project project) => project.Tasks.Count == 0;

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
             [Optionally] Language language,
             IContext context) =>
                 context.WithUpdated(proj, new(proj) { Language = language });

        [Edit]
        public static IContext EditDescription(
            this Project proj,
            string description,
            IContext context) =>
             context.WithUpdated(proj, new(proj) { Description = description });


        [Edit]
        public static IContext EditCommonHiddenCodeFile(
            this Project project,
            [Optionally] File commonHiddenCodeFile,
            IContext context) => 
                context.WithUpdated(project, new Project(project) {CommonHiddenCodeFile = commonHiddenCodeFile});


        public static string ValidateEditCommonHiddenCodeFile(this Project project, File commonHiddenCodeFile) =>
            ValidateLanguageAndContentType(project, commonHiddenCodeFile, ContentType.HiddenCode);

        internal static string ValidateLanguageAndContentType(Project project, File commonHiddenCodeFile, ContentType required) =>
            commonHiddenCodeFile != null &&
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

        #region Assignment
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

        #region Associated Files

        #region Hidden Code
        [MemberOrder(30)]
        public static IContext AddCommonHiddenCode(
            this Project proj,
            File file,
            IContext context) =>
            context.WithUpdated(proj,
                    new(proj)
                    {
                        CommonHiddenCodeFileId = file.Id,
                        CommonHiddenCodeFile = file,
                    });

        public static string ValidateAddCommonAddHiddenCode(
            this Project proj,
            File file) =>
            file.ValidateContentType(ContentType.HiddenCode);


        public static bool HideAddCommonHiddenCode(this Project proj) => proj.CommonHiddenCodeFileId != null;

        [MemberOrder(31)]
        public static IContext ClearCommonHiddenCode(
            this Project proj,
            IContext context) =>
            context.WithUpdated(proj,
            new(proj)
            {
                CommonHiddenCodeFileId = null,
                CommonHiddenCodeFile = null,
            });

        public static bool HideClearCommonHiddenCode(this Project proj) => proj.CommonHiddenCodeFileId == null;
        #endregion

        #region Tests
        [MemberOrder(40)]
        public static IContext AddCommonTests(
            this Project proj,
            File file,
            IContext context) =>
             context.WithUpdated(proj,
                    new(proj)
                    {
                        CommonTestsFileId = file.Id,
                        CommonTestsFile = file,
                    });

        public static string ValidateAddCommonTests(
            this Project proj,
            File file) =>
                file.ValidateContentType(ContentType.Tests);


        public static bool HideAddCommonTests(this Project proj) => proj.CommonTestsFileId != null;

        [MemberOrder(41)]
        public static IContext ClearCommonTests(
             this Project proj,
             IContext context) =>
                context.WithUpdated(proj,
                new(proj)
                {
                    CommonTestsFileId = null,
                    CommonTestsFile = null,
                });

        public static bool HideClearCommonTests(this Project proj) => proj.CommonTestsFileId == null;

        #endregion

        #region Custom Wrapper Code
        [MemberOrder(50)]
        public static IContext AddCustomWrapperCode(
            this Project proj,
            File file,
            IContext context) =>
             context.WithUpdated(proj,
                    new(proj)
                    {
                        WrapperFileId = file.Id,
                        WrapperFile = file,
                    });

        public static IEnumerable<File> Choices1AddCustomWrapperCode(this Project proj, IContext context)
        {
            string langId = proj.LanguageId;
            return context.Instances<File>().Where(f => f.ContentType == ContentType.Wrapper && f.LanguageId == langId);
        }

        public static bool HideAddCustomWrapperCode(this Project proj) => proj.WrapperFileId != null;

        [MemberOrder(51)]
        public static IContext ClearCustomWrapperCode(
             this Project proj,
             IContext context) =>
                context.WithUpdated(proj,
                new(proj)
                {
                    WrapperFileId = null,
                    WrapperFile = null,
                });

        public static bool HideClearCustomWrapperCode(this Project proj) => proj.WrapperFileId == null;

        #endregion

        #region Custom RegEx Rules
        [MemberOrder(60)]
        [Named("Add Custom RegEx Rules")]
        public static IContext AddCustomRegExRules(
            this Project proj,
            File file,
            IContext context) =>
            context.WithUpdated(proj,
                    new(proj)
                    {
                        RegExRulesFileId = file.Id,
                        RegExRulesFile = file,
                    });

        public static IEnumerable<File> Choices1AddCustomRegExRules(this Project proj, IContext context)
        {
            string langId = proj.LanguageId;
            return context.Instances<File>().Where(f => f.ContentType == ContentType.RegExRules && f.LanguageId == langId);
        }

        public static bool HideAddCustomRegExRules(this Project proj) => proj.RegExRulesFileId != null;

        [MemberOrder(61)]
        public static IContext ClearCustomRegExRules(
             this Project proj,
             IContext context) =>
                context.WithUpdated(proj,
                new(proj)
                {
                    RegExRulesFileId = null,
                    RegExRulesFile = null,
                });

        public static bool HideClearCustomRegExRules(this Project proj) => proj.RegExRulesFileId == null;

        #endregion

        #endregion

        #region internal functions
        internal static bool IsAssignable(this Project project) => project.Status == ProjectStatus.Assignable;

        internal static bool IsAssignedToMe(this Project project, IContext context)
        {
            var pid = project.Id;
            return Assignments.AssignmentsForCurrentUser(pid, context).Any();
        }
        #endregion

        #region Create Task
        [MemberOrder(100)]
        public static IContext CreateTask(
            this Project project,
            [Optionally] int? taskNumber,
            [Optionally] string title,
            [Optionally] Task previousTask,
            IContext context)
        {
            var task = new Task
            {
                ProjectId = project.Id,
                Project = project,
                Number = taskNumber,
                PreviousTaskId = previousTask is null ? null : previousTask.Id,
                PreviousTask = previousTask,
            };
            var file = new File()
            {
                Name = $"{project.Title} {taskNumber}",
                ContentType = ContentType.TaskDescription,
                AuthorId = project.AuthorId,
                LanguageId = null,
                Content = new byte[0]
            };
            var updatedPrevious = previousTask is null ? null :
                new Task(previousTask) { NextTask = task };
            var context2 = updatedPrevious is null ? context : context.WithUpdated(previousTask, updatedPrevious);
            return context2.WithNew(task).WithNew(file);
        }

        public static Task Default3CreateTask(
            this Project project) =>
            project.Tasks.LastOrDefault();

        public static string ValidateCreateTask(
            this Project project,
            [Optionally] int? taskNumber,
            [Optionally] string title,
            [Optionally] Task previousTask,
            IContext context) =>
            taskNumber is null && title is null || taskNumber is not null && title is not null ?
            "Must specify Task Number, or Title, but not both"
            :
            null;
        #endregion

        #region Copying
        [MemberOrder(110)]
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

        [MemberOrder(120)]
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
                HiddenCodeFileId = project.LanguageId == copyFrom.Project.LanguageId ? copyFrom.HiddenCodeFileId : null,
                HiddenCodeFile = project.LanguageId == copyFrom.Project.LanguageId ? copyFrom.HiddenCodeFile : null,
                TestsFileId = project.LanguageId == copyFrom.Project.LanguageId ? copyFrom.TestsFileId : null,
                TestsFile = project.LanguageId == copyFrom.Project.LanguageId ? copyFrom.TestsFile : null,
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
    }
}
