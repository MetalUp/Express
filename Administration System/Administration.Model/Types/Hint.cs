
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
            CostInMarks = cloneFrom.CostInMarks;
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

        [MemberOrder(3)]
        public int CostInMarks { get; init; }

        #region HtmlFile
        [Hidden]
        public FileAttachment HtmlFile => File?.ViewContent;

        [Hidden]
        public int? FileId { get; init; }

        [MemberOrder(70)]
        public virtual File File { get; init; }

        [Hidden]
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

        #endregion
        public override string ToString() => $"Hint {Number}";
    }
}