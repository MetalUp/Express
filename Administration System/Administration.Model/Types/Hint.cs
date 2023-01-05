﻿
using NakedFramework.Value;

namespace Model.Types
{
    public class Hint
    {
        public Hint() { }
        public Hint(Hint cloneFrom)
        {
            Id = cloneFrom.Id;
            Name = cloneFrom.Name;
            Number = cloneFrom.Number;
            CostInMarks = cloneFrom.CostInMarks;
            Content = cloneFrom.Content;
            Tasks = cloneFrom.Tasks;
        }

        [Hidden]
        public int Id { get; init; }

        [MemberOrder(1)]
        public int Number { get; init; }

        [MemberOrder(2)]
        public string Name { get; init; }

        [Hidden]
        public string Title => ToString();

        [MemberOrder(3)]
        public int CostInMarks { get; init; }

        #region HtmlFile
        [Hidden]
        public FileAttachment HtmlFile => File?.Details;

        [Hidden]
        public int? FileId { get; init; }

        [MemberOrder(70)]
        public virtual File File { get; init; }

        [Hidden]
        public byte[] Content { get; init; } //To be deleted when content has been moved into File objects and associated

        [Hidden]
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

        #endregion
        public override string ToString() => $"Hint {Number}";
    }
}