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
            RealName = cloneFrom.RealName;
            EmailAddress = cloneFrom.EmailAddress;
            OrganisationId = cloneFrom.OrganisationId;
            Organisation = cloneFrom.Organisation;
        }

        [Hidden]
        public int Id { get; init; }

        [Hidden]
        public int UserId { get; init; }
        [MemberOrder(1)]
        public virtual User User { get; init; }

        [MemberOrder(2)][DescribedAs("Name and/or Student Id")]
        public string RealName { get; init; }

        [MemberOrder(3)]
        public string EmailAddress { get; init; }

        [Hidden]
        public int OrganisationId { get; init; }
        [MemberOrder(6)]
        public virtual Organisation Organisation { get; init; }

        public override string ToString() => RealName;
    }
}
