﻿using NakedFramework.Value;

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
            Language = cloneFrom.Language;
            PasteExpression = cloneFrom.PasteExpression;
            PasteCode = cloneFrom.PasteCode;
            Description = cloneFrom.Description;
            Tasks = new List<Task>(cloneFrom.Tasks);
        }

        [Hidden]
        public int Id { get; init; }

        [MemberOrder(1)]
        [UrlLink("Try out the Project")]
        public string Link => $"https://express.metalup.org/task/{Tasks.First().Id}";

        [Hidden]
        public int AuthorId { get; init; }
        [Hidden]
        public virtual User Author { get; init; }

        [MemberOrder(5)]
        public ProjectStatus Status { get; init; }

        [MemberOrder(10)]
        public string Title { get; init; }

        [MemberOrder(20)]
        public virtual Language Language { get; init; }

        [MemberOrder(30)]
        public bool PasteExpression { get; init; }

        [MemberOrder(40)]
        public bool PasteCode { get; init; }

        #region Common Hidden Code
        [Hidden]
        public int? CommonHiddenCodeFileId { get; init; }

        [MemberOrder(80)]
        public virtual File CommonHiddenCodeFile { get; init; }
        #endregion

        #region CommonTests
        [Hidden]
        public int? CommonTestsFileId { get; init; }

        [MemberOrder(90)]
        public virtual File CommonTestsFile { get; init; }
        #endregion

        [MemberOrder(100)]
        [MultiLine(10)]
        public string Description { get; init; }

        [RenderEagerly]
        public virtual ICollection<Task> Tasks { get; init; } = new List<Task>();

        public override string ToString() => $"{Title} ({Language})";
    }
}
