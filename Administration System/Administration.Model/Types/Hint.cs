
using NakedFramework.Value;

namespace Model.Types
{
    public class Hint
    {
        public Hint() { }
        public Hint(Hint cloneFrom)
        {
            Id = cloneFrom.Id;
            Number = cloneFrom.Number;
            FileId = cloneFrom.FileId;
            File = cloneFrom.File;
            Tasks = cloneFrom.Tasks;
        }

        [Hidden]
        public int Id { get; init; }

        [MemberOrder(1)]
        public int Number { get; init; }

        [Hidden]
        public string Title => ToString();

        [MemberOrder(10)]
        public FileAttachment ViewContent => new FileAttachment(File.Content, "Click here to open view", "text/html");

        [MemberOrder(20)]
        [UrlLink("Click here to open editor")]
        public string EditContent => $"/dashboard/editor/{FileId}";

        #region HtmlFile
        [Hidden]
        public int? FileId { get; init; }

        [MemberOrder(70)]
        public virtual File File { get; init; }
        #endregion

        [Hidden]
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

        public override string ToString() => $"Hint {Number}";
    }
}