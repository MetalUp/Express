namespace Model.Types
{
    public class Invitation
    {
        public Invitation() {
            Status = InvitationStatus.Pending;
        }

        public Invitation(Invitation cloneFrom)
        {
            Id = cloneFrom.Id;
            ToUserName = cloneFrom.ToUserName;
            From = cloneFrom.From;
            ToJoin = cloneFrom.ToJoin;
            AsRole = cloneFrom.AsRole;
            IssuedOn = cloneFrom.IssuedOn;
            Valid = cloneFrom.Valid;
            Status = cloneFrom.Status;
        }

        [Hidden]
        public int Id { get; init; }

        [MemberOrder(1)]
        public string ToUserName { get; init; } //Username

        [Hidden]
        public int FromUserId { get; init; }
        [MemberOrder(2)]
        public virtual User From { get; init; }

        [Hidden]
        public  int ToJoinId { get; init; }
        [MemberOrder(3)]
        public virtual Organisation ToJoin { get; init; }

        [MemberOrder(4)]
        public Role AsRole { get; init; }

        [MemberOrder(5)]
        public DateTime IssuedOn { get; init; }

        [Named("Valid (no. of days)")][MemberOrder(6)]
        public int Valid { get; init; }

        [MemberOrder(7)]
        public InvitationStatus Status {get; init;}  

        public override string ToString() => "An invitation";
    }


}
