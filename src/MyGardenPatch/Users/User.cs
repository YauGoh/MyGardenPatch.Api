using MyGardenPatch.Aggregates;
using MyGardenPatch.Users.DomainEvents;

namespace MyGardenPatch.Users
{
    public partial record struct UserId : IEntityId { }

    public class User : Aggregate<UserId>, INameable
    {
        public User(
            UserId id,
            string name,
            string emailAddress,
            DateTime? registeredAt,
            bool receivesEmail) : base(id)
        {
            Name = name;
            EmailAddress = emailAddress;
            RegisteredAt = registeredAt;
            ReceivesEmail = receivesEmail;
        }

        public User(
            string name,
            string emailAddress) : this(new UserId(), name, emailAddress, null, false)
        { }

        public User(
            UserId id,
            string name,
            string emailAddress) : base(id)
        {
            Name = name;
            EmailAddress = emailAddress;
            RegisteredAt = null;
            ReceivesEmail = false;
        }

        public string Name { get; private set; }

        public string EmailAddress { get; private set; }

        public DateTime? RegisteredAt { get; private set; }

        public bool ReceivesEmail { get; private set; }

        public void Register(DateTime dateTime, bool recievesEmail)
        {
            this.RegisteredAt = dateTime;
            this.ReceivesEmail = recievesEmail;

            this.Raise(new NewUserRegistered(this.Id, dateTime));
        }
    }
}
