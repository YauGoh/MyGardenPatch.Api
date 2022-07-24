namespace MyGardenPatch.GardenBeds.Queries
{
    public interface IPlantQuery : IGardenBedQuery
    {
        GardenBedId GardenBedId { get; }
    }
}
