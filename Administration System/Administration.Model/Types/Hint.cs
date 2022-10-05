
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
            FileName = cloneFrom.FileName;
            FileMime = cloneFrom.FileMime;
            TaskId = cloneFrom.TaskId;
            Task = cloneFrom.Task;
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
                 new FileAttachment(FileContent, FileName, FileMime);

        [Hidden]
        public byte[] FileContent { get; init; }

        [MemberOrder(4)]
        public string FileName { get; init; }

        [Hidden]
        public string FileMime { get; init; }
        #endregion

        [Hidden]
        public int TaskId { get; init; }
        [MemberOrder(10)]
        public virtual Task Task { get; init; }

        public override string ToString() => $"{Title} (-{CostInMarks}) marks";
    }
}