namespace MyGardenPatch.Queries;

internal class QueryValidatorNotFoundException<TQuery> : Exception
{
    public override string Message => $"No query validator found for {typeof(TQuery).FullName}";
}
