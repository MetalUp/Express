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

        public Guid Id { get; init; } //This must be specified in mapping as the PK

        public int InviteeId { get; init; }
        public virtual User Invitee { get; init; }

        public int SenderId { get; init; }
        public virtual User Sender { get; init; }

        public DateTime Sent { get; init; }
    }



}
