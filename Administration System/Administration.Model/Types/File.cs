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
            Mime = cloneFrom.Mime;
            Content = cloneFrom.Content;
            AuthorId = cloneFrom.AuthorId;
            Author = cloneFrom.Author;
        }

        [Hidden]
        public int Id { get; init; }

        [Hidden]
        public string Name { get; init; }

        [Hidden]
        public string Mime { get; init; }

        [Hidden]
        public byte[] Content { get; init; }

        [MemberOrder(10)]
        public FileAttachment Details => new FileAttachment(Content, Name, Mime);

        [Hidden]
        public int AuthorId { get; init; }

        [MemberOrder(40)]
        public virtual User Author { get; init; }

        //Display content as string

        //Provide ViewInTab as FileAttachment

        public override string ToString() => Name;
    }
}
