namespace Model.Types
{
    public class Invitation
    {
        public Invitation() { }

        public Invitation(Invitation cloneFrom)
        {
            Id = cloneFrom.Id;
            InviteeId = cloneFrom.InviteeId;
            Invitee = cloneFrom.Invitee;
            SenderId = cloneFrom.SenderId;
            Sender = cloneFrom.Sender;
            Sent = cloneFrom.Sent;
        }

        [Hidden]
        public Guid Id { get; init; } //This must be specified in mapping as the PK

        [Hidden]
        public int InviteeId { get; init; }
        [MemberOrder(10)]
        public virtual User Invitee { get; init; }

        [Hidden]
        public int SenderId { get; init; }
        [MemberOrder(20)]
        public virtual User Sender { get; init; }

        [MemberOrder(30)]
        public DateTime Sent { get; init; }

        public override string ToString() => $"An invitation";
    }



}
