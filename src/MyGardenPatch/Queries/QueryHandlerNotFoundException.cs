namespace MyGardenPatch.Queries;

[Serializable]
internal class QueryHandlerNotFoundException<TQuery> : Exception
{
    public override string Message => $"No query handler found for {typeof(TQuery).FullName}";
}
