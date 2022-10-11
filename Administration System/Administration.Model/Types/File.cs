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

        [MemberOrder(10)]
        public string Name { get; init; }

        [MemberOrder(20)]
        public string Mime { get; init; }

        [MemberOrder(30)]
        public byte[] Content { get; init; }

        [Hidden]
        public int AuthorId { get; init; }

        [MemberOrder(40)]
        public virtual User Author { get; init; }

        //Display content as string

        //Provide ViewInTab as FileAttachment

        public override string ToString() => Name;
    }
}
