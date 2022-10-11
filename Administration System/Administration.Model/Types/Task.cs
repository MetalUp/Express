﻿using NakedFramework.Value;

namespace Model.Types
{
    public class Task
    {
        public Task() { }
        public Task(Task cloneFrom)
        {
            Id = cloneFrom.Id;
            ProjectId = cloneFrom.ProjectId;
            Project = cloneFrom.Project;
            Title = cloneFrom.Title;
            MaxMarks = cloneFrom.MaxMarks;
            DescContent = cloneFrom.DescContent;
            DescName = cloneFrom.DescName;
            RMFContent = cloneFrom.RMFContent;
            PasteExpression = cloneFrom.PasteExpression;
            PasteFunctions = cloneFrom.PasteFunctions;
            TestsContent = cloneFrom.TestsContent;
            PreviousTaskId = cloneFrom.PreviousTaskId;
            PreviousTask = cloneFrom.PreviousTask;
            NextTaskId = cloneFrom.NextTaskId;
            NextTask = cloneFrom.NextTask;
            NextTaskClearsFunctions = cloneFrom.NextTaskClearsFunctions;
            Hints = cloneFrom.Hints;
        }

        [Hidden]
        public int Id { get; init; }

        [MemberOrder(10)]
        [UrlLink("Try out the Task")]
        public string Link => $"https://express.metalup.org/task/{Id}";

        [MemberOrder(30)]
        public string Title { get; init; }

        [MemberOrder(40)]
        public ProgrammingLanguage Language => Project.Language;

        //Marks awarded for completing the task with no hints taken
        [MemberOrder(60)]
        public int MaxMarks { get; init; }

        #region Description
        [Hidden]
        public int DescriptionFileId { get; init; }

        [MemberOrder(70)]
        public virtual File DescriptionFile { get; init; }

        [HideInClient]
        public FileAttachment Description => (DescContent == null) ? null :
                 new FileAttachment(DescContent, DescName, "text/html");

        [Hidden]
        public byte[] DescContent { get; init; }

        [Hidden]
        public string DescName { get; init; }
        #endregion

        #region Hidden Functions
        [Hidden]
        public int HiddenFunctionsFileId { get; init; }

        [MemberOrder(80)]
        public virtual File HiddenFunctionsFile { get; init; }

        [Named("Hidden Functions")]
        [HideInClient]
        public FileAttachment ReadyMadeFunctions => (RMFContent == null) ? null :
                 new FileAttachment(RMFContent, $"HiddenFunctions{Project.LanguageAsFileExtension()}", "text/plain");

        [Hidden]
        public byte[] RMFContent { get; init; }

        #endregion

        #region Tests
        [Hidden]
        public int TestsFileId { get; init; }

        [MemberOrder(90)]
        public virtual File TestsFile { get; init; }

        [HideInClient]
        public FileAttachment Tests => (TestsContent == null) ? null :
                 new FileAttachment(TestsContent, $"Tests{Project.LanguageAsFileExtension()}", "text/plain");

        [Hidden]
        public byte[] TestsContent { get; init; }
        #endregion

        [MemberOrder(100)]
        public bool PasteExpression { get; init; }

        [MemberOrder(101)]
        public bool PasteFunctions { get; init; }

        [Hidden]
        public int? PreviousTaskId { get; init; }

        [MemberOrder(110)]
        public virtual Task PreviousTask { get; init; }

        [Hidden]
        public int? NextTaskId { get; init; }

        [MemberOrder(120)]
        public virtual Task NextTask { get; init; }

        [MemberOrder(130)]
        public bool NextTaskClearsFunctions { get; init; }

        [Hidden]
        public int? ProjectId { get; init; }

        [MemberOrder(200)]
        public virtual Project Project { get; init; }

        public virtual ICollection<Hint> Hints { get; set; } = new List<Hint>();

        public override string ToString() => $"{Title} {Language}";
    }
}