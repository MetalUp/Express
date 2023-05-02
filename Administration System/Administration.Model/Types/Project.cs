namespace Model.Types
{
    public class Project
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Project() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Project(Project cloneFrom)
        {
            Id = cloneFrom.Id;
            AuthorId = cloneFrom.AuthorId;
            Author = cloneFrom.Author;
            Status = cloneFrom.Status;
            Title = cloneFrom.Title;
            LanguageId = cloneFrom.LanguageId;
            Language = cloneFrom.Language;
            Description = cloneFrom.Description;
            CommonHiddenCodeFileId = cloneFrom.CommonHiddenCodeFileId;
            CommonHiddenCodeFile = cloneFrom.CommonHiddenCodeFile;
            CommonTestsFileId = cloneFrom.CommonTestsFileId;
            CommonTestsFile = cloneFrom.CommonTestsFile;    
            WrapperFileId = cloneFrom.WrapperFileId;
            WrapperFile = cloneFrom.WrapperFile;
            RegExRulesFileId = cloneFrom.RegExRulesFileId;
            RegExRulesFile = cloneFrom.RegExRulesFile;
            Tasks = new List<Task>(cloneFrom.Tasks);
        }

        [Hidden]
        public int Id { get; init; }

        [MemberOrder(1)]
        [UrlLink("Start Project")]
        public string Link => $"/task/{Tasks.First().Id}";

        [Hidden]
        public int AuthorId { get; init; }
        [Hidden]
        public virtual User Author { get; init; }

        [MemberOrder(5)]
        public ProjectStatus Status { get; init; }

        [MemberOrder(10)]
        public string Title { get; init; }

        [Hidden]
        public virtual string LanguageId { get; init; }

        [MemberOrder(20)]
        public virtual Language Language { get; init; }

        #region Common Hidden Code
        internal string CommonHiddenCode => CommonHiddenCodeFile is null ? null : CommonHiddenCodeFile.ContentsAsString();

        [Hidden]
        public int? CommonHiddenCodeFileId { get; init; }

        [MemberOrder(80)]
        [Named("Common Hidden Code")]
        public virtual File CommonHiddenCodeFile { get; init; }
        #endregion

        #region CommonTests
       internal string CommonTests => CommonTestsFile?.ContentsAsString();

        [Hidden]
        public int? CommonTestsFileId { get; init; }

        [MemberOrder(90)]
        [Named("Common Tests")]
        public virtual File CommonTestsFile { get; init; }
        #endregion

        #region Wrapper

        internal string Wrapper => WrapperFileId == null ? Language.Wrapper : WrapperFile.ContentsAsString();

        [Hidden]
        public int? WrapperFileId { get; init; }

        [MemberOrder(100)]
        [Named("Custom Wrapper Code")]
        public virtual File WrapperFile { get; init; }

        #endregion

        #region Helpers

        [Hidden]
        public string Helpers => Language.Helpers;

        #endregion

        #region RegExRules
        internal string RegExRules => RegExRulesFileId == null ? Language.RegExRules  : RegExRulesFile.ContentsAsString();

        [Hidden]
        public int? RegExRulesFileId { get; init; }

        [MemberOrder(110)]
        [Named("Custom RegEx Rules")]
        public virtual File RegExRulesFile { get; init; }

        #endregion

        [MemberOrder(120)]
        [MultiLine(5)]
        public string Description { get; init; }


        [RenderEagerly]
        [TableView(false, nameof(Task.Number), nameof(Task.Summary), nameof(Task.Hints))]
        public virtual ICollection<Task> Tasks { get; init; } = new List<Task>();

        public override string ToString() => $"{Title} ({Language})";
    }
}
