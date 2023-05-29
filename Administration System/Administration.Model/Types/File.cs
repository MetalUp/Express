using NakedFramework;
using NakedFramework.Value;

namespace Model.Types
{
    public class File
    {
        public File() { }

        public File(File cloneFrom)
        {
            Id = cloneFrom.Id;
            Name = cloneFrom.Name;
            ContentType = cloneFrom.ContentType;
            LanguageId = cloneFrom.LanguageId;
            Language = cloneFrom.Language;
            Content = cloneFrom.Content;
            AuthorId = cloneFrom.AuthorId;
            Author = cloneFrom.Author;
            UniqueRef = cloneFrom.UniqueRef;
        }

        [Hidden]
        public int Id { get; init; }

        [MemberOrder(10)]
        public FileAttachment ViewContent => new FileAttachment(Content, "Click here to open view", this.MIMEType());

        [MemberOrder(20)]
        [UrlLink(true,"Click here to open editor")]
        public string EditContent => $"/dashboard/editor/{Id}";

        [MemberOrder(30)]
        public string Name { get; init; }

        [MemberOrder(40)]
        public ContentType? ContentType { get; init; }

        [Hidden]
        public string LanguageId { get; init; }

        [MemberOrder(60)]
        public virtual Language Language { get; init; }

        [Hidden]
        public byte[] Content { get; init; }

        [Hidden]
        public int AuthorId { get; init; }

        [MemberOrder(70)]
        public virtual User Author { get; init; }

        [MemberOrder(80)] //Used by build, across databases, and independent of (auto-generated) Id
        public Guid? UniqueRef { get; init; }

        public override string ToString() => $"{Name} {ContentType} {Language}";

    }
}
