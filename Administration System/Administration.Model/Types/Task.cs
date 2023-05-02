using NakedFramework.Value;
using System.Text;

namespace Model.Types
{
    public class Task
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Task() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Task(Task cloneFrom)
        {
            Id = cloneFrom.Id;
            ProjectId = cloneFrom.ProjectId;
            Project = cloneFrom.Project;
            Number = cloneFrom.Number;
            Title = cloneFrom.Title;
            Summary = cloneFrom.Summary;
            DescriptionFileId = cloneFrom.DescriptionFileId;
            DescriptionFile = cloneFrom.DescriptionFile;
            HiddenCodeFileId = cloneFrom.HiddenCodeFileId;
            HiddenCodeFile = cloneFrom.HiddenCodeFile;
            TestsFileId = cloneFrom.TestsFileId;
            TestsFile = cloneFrom.TestsFile;
            TestsRunOnClient = cloneFrom.TestsRunOnClient;
            WrapperFileId = cloneFrom.WrapperFileId;
            WrapperFile = cloneFrom.WrapperFile;
            RegExRulesFileId = cloneFrom.RegExRulesFileId;
            RegExRulesFile = cloneFrom.RegExRulesFile;
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

        [MemberOrder(1)]
        [UrlLink("Start Task")]
        public string Link => $"/task/{Id}";

        [MemberOrder(10)]
        public virtual Project Project { get; init; }

        [MemberOrder(20)]
        [Named("No.")]
        public int? Number { get; init; }

        [MemberOrder(25)]
        [Named("Title (override)")]
        public string Title { get; init; }

        [MemberOrder(30)]
        public string Summary { get; init; }

        [Hidden]
        public string Language => Project.Language.CSSstyle;

        #region Description
        internal FileAttachment Description => DescriptionFile?.ViewContent;

        [Hidden]
        public int? DescriptionFileId { get; init; }

        [MemberOrder(70)]
        public virtual File DescriptionFile { get; init; }
        #endregion

        #region Hidden Code
        internal string HiddenCode => HiddenCodeFile is null ?  Project.CommonHiddenCode :HiddenCodeFile.ContentsAsString();

        [Hidden]
        public int? HiddenCodeFileId { get; init; }

        [MemberOrder(80)]
        [Named("Task Specific Hidden Code")]
        public virtual File HiddenCodeFile { get; init; }
        #endregion

        #region Tests
        internal string Tests => TestsFile is null ? Project.CommonTests : TestsFile.ContentsAsString();

        [Hidden]
        public bool HasTests => Tests is not null;

        [Hidden]
        public int? TestsFileId { get; init; }

        [MemberOrder(90)]
        [Named("Task Specific Tests")]
        public virtual File TestsFile { get; init; }
        #endregion

        //Normally false. Set true for ARMlite or other Tasks where tests are written in JavaScript to run on client.
        [MemberOrder(91)]
        public bool TestsRunOnClient { get; init; }

        #region Wrapper

        internal string Wrapper => WrapperFileId == null ? Project.Wrapper : WrapperFile.ContentsAsString();

        [Hidden]
        public int? WrapperFileId { get; init; }

        [MemberOrder(100)]
        [Named("Task specific Wrapper Code")]
        public virtual File WrapperFile { get; init; }

        #endregion
        #region RegExRules

        internal string RegExRules => RegExRulesFileId == null ? Project.RegExRules : RegExRulesFile.ContentsAsString();

        [Hidden]
        public int? RegExRulesFileId { get; init; }

        [MemberOrder(110)]
        [Named("Task specific RegEx Rules")]
        public virtual File RegExRulesFile { get; init; }

        #endregion
        #region Helpers
        //Helpers should be generic
        internal string Helpers => Project.Helpers;

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

        public override string ToString() => Number is null ?
            $"{Title} ({Project.Language.Name})" :
            $"{Project.Title} Task {Number} ({Project.Language.Name})";
    }  
}