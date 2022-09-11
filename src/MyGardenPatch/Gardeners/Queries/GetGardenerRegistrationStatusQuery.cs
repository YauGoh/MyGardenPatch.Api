namespace MyGardenPatch.Gardeners.Queries;

[Role(WellKnownRoles.Gardener)]
public record GetGardenerRegistrationStatusQuery : IQuery<GardenerRegistrationStatus>
{
}

public enum RegistrationStatus
{
    NotRegistered,
    Registered
}

public record GardnerInfo(GardenerId? GardenerId, string Name, string EmailAddress);

public record GardenerRegistrationStatus(RegistrationStatus Status, GardnerInfo? User) { }

public class GetGardenerRegistrationStatusQueryHandler : IQueryHandler<GetGardenerRegistrationStatusQuery, GardenerRegistrationStatus>
{
    private readonly IRepository<Gardener, GardenerId> _gardeners;
    private readonly ICurrentUserProvider _userProvider;

    public GetGardenerRegistrationStatusQueryHandler(IRepository<Gardener, GardenerId> gardeners, ICurrentUserProvider userProvider)
    {
        _gardeners = gardeners;
        _userProvider = userProvider;
    }

    public async Task<GardenerRegistrationStatus> HandleAsync(
        GetGardenerRegistrationStatusQuery query,
        CancellationToken cancellationToken = default)
    {
        var gardener = await _gardeners.GetByExpressionAsync(u => u.EmailAddress == _userProvider.EmailAddress, cancellationToken);

        return new GardenerRegistrationStatus(
            gardener is null ? RegistrationStatus.NotRegistered : RegistrationStatus.Registered,
            gardener is null ?
                new GardnerInfo(null, _userProvider.Name!, _userProvider.EmailAddress!)
                :
                new GardnerInfo(gardener.Id, gardener.Name, gardener.EmailAddress));
    }
}

public class GetGardenerRegistrationStatusQueryHandlerValidator : AbstractValidator<GetGardenerRegistrationStatusQuery>, IQueryValidator<GetGardenerRegistrationStatusQuery>
{
    public GetGardenerRegistrationStatusQueryHandlerValidator(ICurrentUserProvider userProvider)
    {
        RuleFor(q => q)
            .Must(q => userProvider.EmailAddress != null)
            .WithMessage("Not logged in with a valid email address");
    }
}
