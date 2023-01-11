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
            Number = cloneFrom.Number;
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


        [Hidden]
        public int? ProjectId { get; init; }

        [MemberOrder(10)]
        public virtual Project Project { get; init; }

        [MemberOrder(20)]
        [Named("Task No. within Project")]
        public int Number { get; init; }

        [Hidden]
        public string Title => ToString();

        [Hidden]
        public string Language => Project.Language.AlphaName;

        //Marks awarded for completing the task with no hints taken
        [MemberOrder(60)]
        public int MaxMarks { get; init; }

        #region Description
        [Hidden]
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
        [Named("Task Specific Hidden Code")]
        public virtual File HiddenCodeFile { get; init; }
        #endregion

        #region Tests
        [Hidden]
        public string Tests => TestsFile is null ? Project.CommonTests : TestsFile.ContentsAsString();

        [Hidden]
        public bool HasTests => Tests is not null;

        [Hidden]
        public int? TestsFileId { get; init; }

        [MemberOrder(90)]
        [Named("Task Specific Tests")]
        public virtual File TestsFile { get; init; }
        #endregion

        #region Wrapper

        [Hidden]
        public string Wrapper => Project.Wrapper;

        #endregion

        #region Helpers

        [Hidden]
        public string Helpers => Project.Helpers;

        #endregion

        #region RegExRules

        [Hidden]
        public string RegExRules => Project.RegExRules;

        #endregion

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

        public virtual ICollection<Hint> Hints { get; set; } = new List<Hint>();

        public override string ToString() => $"{Project.Title} {Number} {Project.Language.Name}";
    }
}