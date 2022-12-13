namespace Model.Types
{
    public class Project
    {
        public Project() { }

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
            Tasks = new List<Task>(cloneFrom.Tasks);
        }

        [Hidden]
        public int Id { get; init; }

        [MemberOrder(1)]
        [UrlLink("Start or Continue Project")]
        public string Link => $"https://express.metalup.org/task/{Tasks.First().Id}";

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
        [Hidden]
        public string CommonHiddenCode => CommonHiddenCodeFile?.ContentsAsString();
        [Hidden]
        public int? CommonHiddenCodeFileId { get; init; }

        [MemberOrder(80)]
        public virtual File CommonHiddenCodeFile { get; init; }
        #endregion

        #region CommonTests
        [Hidden]
        public string CommonTests => CommonTestsFile?.ContentsAsString();

        [Hidden]
        public int? CommonTestsFileId { get; init; }

        [MemberOrder(90)]
        public virtual File CommonTestsFile { get; init; }
        #endregion

        #region Wrapper

        [Hidden]
        public string Wrapper => WrapperFileId == null ? Language.Wrapper : WrapperFile.ContentsAsString();

        [Hidden]
        public int? WrapperFileId { get; init; }

        [MemberOrder(100)]
        public virtual File WrapperFile { get; init; }

        #endregion

        #region Helpers

        [Hidden]
        public string Helpers => HelpersFileId == null ? Language.Helpers : HelpersFile.ContentsAsString();

        [Hidden]
        public int? HelpersFileId { get; init; }

        [MemberOrder(110)]
        public virtual File HelpersFile { get; init; }

        #endregion

        #region RegExRules
        [Hidden]
        public string RegExRules => RegExRulesFileId == null ? Language.RegExRules  : RegExRulesFile.ContentsAsString();

        [Hidden]
        public int? RegExRulesFileId { get; init; }

        [MemberOrder(110)]
        public virtual File RegExRulesFile { get; init; }

        #endregion

        [MemberOrder(120)]
        [MultiLine(10)]
        public string Description { get; init; }

        [RenderEagerly]
        public virtual ICollection<Task> Tasks { get; init; } = new List<Task>();

        public override string ToString() => $"{Title} ({Language})";
    }
}
