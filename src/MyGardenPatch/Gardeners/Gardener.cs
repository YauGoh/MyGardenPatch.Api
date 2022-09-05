namespace MyGardenPatch.Gardeners;

public partial record struct GardenerId : IEntityId { }

public class Gardener : Aggregate<GardenerId>, INameable
{
    public Gardener(
        GardenerId id,
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

    public Gardener(
        string name,
        string emailAddress) : this(new GardenerId(), name, emailAddress, null, false)
    { }

    public Gardener(
        GardenerId id,
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

        this.Raise(new NewGardenerRegistered(this.Id, dateTime));
    }
}
