
using NakedFramework.Value;

namespace Model.Types
{
    public class Hint
    {
        public Hint() { }
        public Hint(Hint cloneFrom)
        {
            Id = cloneFrom.Id;
            Title = cloneFrom.Title;
            Number = cloneFrom.Number;
            CostInMarks = cloneFrom.CostInMarks;
            FileContent = cloneFrom.FileContent;
            Tasks = cloneFrom.Tasks;
        }

        [Hidden]
        public int Id { get; init; }


        [MemberOrder(1)]
        public int Number { get; init; }

        [MemberOrder(2)]
        public string Title { get; init; }

        [MemberOrder(3)]
        public int CostInMarks { get; init; }

        #region HtmlFile
        [HideInClient]
        public FileAttachment HtmlFile => (FileContent == null) ? null :
                 new FileAttachment(FileContent, $"{Title}", "text/html");

        [Hidden]
        public byte[] FileContent { get; init; }

        [Hidden]
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

        #endregion
        public override string ToString() => $"{Title} (-{CostInMarks}) marks";
    }
}