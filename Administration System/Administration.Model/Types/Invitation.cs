namespace Model.Types
{
    public class Invitation
    {
        public Invitation() { }

        public Invitation(Invitation cloneFrom)
        {
            Id = cloneFrom.Id;
            ToUser = cloneFrom.ToUser;
            FromUser = cloneFrom.FromUser;
            ToJoin = cloneFrom.ToJoin;
            AsRole = cloneFrom.AsRole;
            IssuedOn = cloneFrom.IssuedOn;
            ValidForDays = cloneFrom.ValidForDays;
        }

        [Hidden]
        public int Id { get; init; }

        public virtual string ToUser { get; init; } //Email address

        public int FromUserId { get; init; }
        public virtual User FromUser { get; init; }

        public  int ToJoinId { get; init; }
        public virtual Organisation ToJoin { get; init; }

        public Role AsRole { get; init; }

        public DateTime IssuedOn { get; init; }

        public int ValidForDays { get; init; }

        public InvitationStatus Status {get; init;}  

        public override string ToString() => "An invitation";
    }

    public enum InvitationStatus
    {
        Pending, Accepted, Expired, Rescinded
    }
}
