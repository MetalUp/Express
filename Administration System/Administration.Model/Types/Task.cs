using NakedFramework.Value;
using System.Text;

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
            HiddenCodeFileId = cloneFrom.HiddenCodeFileId;
            HiddenCodeFile = cloneFrom.HiddenCodeFile;
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

        [MemberOrder(30)]
        public string Name { get; init; }

        [HideInClient]
        public string Title => ToString();

        [HideInClient]
        public string Language => Project.Language.AlphaName;

        //Marks awarded for completing the task with no hints taken
        [MemberOrder(60)]
        public int MaxMarks { get; init; }

        #region Description
        [HideInClient]
        public FileAttachment Description => DescriptionFile?.Details;
        [Hidden]
        public int? DescriptionFileId { get; init; }

        [MemberOrder(70)]
        public virtual File DescriptionFile { get; init; }
        #endregion

        #region Hidden Code
        [Hidden]
        public string HiddenCode => HiddenCodeFile is null ?  Project.CommonHiddenCode :HiddenCodeFile.ContentsAsString();

        [Hidden]
        public int? HiddenCodeFileId { get; init; }

        [MemberOrder(80)]
        public virtual File HiddenCodeFile { get; init; }
        #endregion

        #region Tests
        [Hidden]
        public string Tests => TestsFile is null ? Project.CommonTests : TestsFile.ContentsAsString();

        [HideInClient]
        public bool HasTests => TestsFileId != null;

        [Hidden]
        public int? TestsFileId { get; init; }

        [MemberOrder(90)]
        public virtual File TestsFile { get; init; }
        #endregion

        #region Wrapper

        [HideInClient]
        public string Wrapper => Project.Wrapper;

        #endregion

        #region Helpers

        [HideInClient]
        public string Helpers => Project.Helpers;

        #endregion

        #region RegExRules

        [HideInClient]
        public string RegExRules => Project.RegExRules;

        #endregion


        [HideInClient]
        public bool PasteExpression => Project.PasteExpression;

        [HideInClient]
        public bool PasteCode => Project.PasteCode;

        [Hidden]
        public int? PreviousTaskId { get; init; }

        [MemberOrder(200)]
        public virtual Task PreviousTask { get; init; }

        [Hidden]
        public int? NextTaskId { get; init; }

        [MemberOrder(210)]
        public virtual Task NextTask { get; init; }

        [MemberOrder(220)]
        public bool NextTaskClearsFunctions { get; init; }

        [Hidden]
        public int? ProjectId { get; init; }

        [MemberOrder(300)]
        public virtual Project Project { get; init; }

        public virtual ICollection<Hint> Hints { get; set; } = new List<Hint>();

        public override string ToString() => $"{Name} {Project.Language.Name}";
    }
}