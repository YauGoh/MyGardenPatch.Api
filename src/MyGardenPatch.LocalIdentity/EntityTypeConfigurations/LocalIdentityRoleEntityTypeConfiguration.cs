namespace MyGardenPatch.LocalIdentity.EntityTypeConfigurations;

internal class LocalIdentityRoleEntityTypeConfiguration : IEntityTypeConfiguration<LocalIdentityRole>
{
    public void Configure(EntityTypeBuilder<LocalIdentityRole> builder)
    {
        builder.HasData(
            new LocalIdentityRole
            {
                Id = new Guid("{0C213B73-1C49-4383-A63B-B505E9D34644}"),
                Name = WellKnownRoles.Api,
                NormalizedName = WellKnownRoles.Api.ToUpper(),
                ConcurrencyStamp = new Guid("{90AE882C-A96C-427C-83FF-0515BC199860}").ToString()
            },
            new LocalIdentityRole
            {
                Id = new Guid("{60C30FAA-E41B-427C-A08C-65E2EB39D990}"),
                Name = WellKnownRoles.Gardener,
                NormalizedName = WellKnownRoles.Gardener.ToUpper(),
                ConcurrencyStamp = new Guid("{E82D2414-EA67-491F-AD92-400889444D63}").ToString()
            });
    }
}
