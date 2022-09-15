using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Types
{
    public class StudentGroup
    {
        public int StudentId { get; init; }
        public virtual User Student { get; init; }
        public int GroupId { get; init; }
        public virtual Group Group { get; init; }
    }
}
