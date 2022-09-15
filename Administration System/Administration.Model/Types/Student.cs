using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Types
{
    public class Student
    {
        public Student() { }

        public Student(Student cloneFrom) { 
            Id = cloneFrom.Id;
            UserId = cloneFrom.UserId;
            User = cloneFrom.User;
            Name = cloneFrom.Name;
            EmailAddress = cloneFrom.EmailAddress;
            OrganisationId = cloneFrom.OrganisationId;
            Organisation = cloneFrom.Organisation;
        }

        [Hidden]
        public int Id { get; init; }

        [Hidden]
        public int? UserId { get; init; }
        [MemberOrder(1)]
        public virtual User User { get; init; }

        [MemberOrder(2)][DescribedAs("Name and/or Student Id")]
        public string Name { get; init; }

        [MemberOrder(3)]
        public string EmailAddress { get; init; }

        [MemberOrder(4)]
        public MemberStatus Status { get; init; }

        [Hidden]
        public int OrganisationId { get; init; }
        [MemberOrder(6)]
        public virtual Organisation Organisation { get; init; }

        public override string ToString() => $"Name{(Status == MemberStatus.Pending ? " (PENDING)" : null)}";
    }
}
