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
            Name = cloneFrom.Name;
            MaxMarks = cloneFrom.MaxMarks;
            DescriptionFileId = cloneFrom.DescriptionFileId;
            DescriptionFile = cloneFrom.DescriptionFile;
            HiddenFunctionsFileId = cloneFrom.HiddenFunctionsFileId;
            HiddenFunctionsFile = cloneFrom.HiddenFunctionsFile;
            PasteExpression = cloneFrom.PasteExpression;
            PasteFunctions = cloneFrom.PasteFunctions;
            TestsFileId = cloneFrom.TestsFileId;
            TestsFile = cloneFrom.TestsFile;    
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
        public string Name { get; init; }

        [HideInClient]
        public string Title => ToString();

        [MemberOrder(40)]
        public ProgrammingLanguage Language => Project.Language;

        //Marks awarded for completing the task with no hints taken
        [MemberOrder(60)]
        public int MaxMarks { get; init; }

        #region Description
        [HideInClient]
        public FileAttachment Description => DescriptionFile is null ? null : DescriptionFile.Details;
        [Hidden]
        public int? DescriptionFileId { get; init; }

        [MemberOrder(70)]
        public virtual File DescriptionFile { get; init; }
        #endregion

        #region Hidden Functions
        [HideInClient]
        public FileAttachment ReadyMadeFunctions => HiddenFunctionsFile is null ? null : HiddenFunctionsFile.Details;

        [Hidden]
        public int? HiddenFunctionsFileId { get; init; }

        [MemberOrder(80)]
        public virtual File HiddenFunctionsFile { get; init; }
        #endregion

        #region Tests
        [HideInClient]
        public FileAttachment Tests => TestsFile is null ? null : TestsFile.Details;

        public int? TestsFileId { get; init; }

        [MemberOrder(90)]
        public virtual File TestsFile { get; init; }
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

        public override string ToString() => $"{Name} {Language}";
    }
}