using FluentValidation.Results;

namespace MyGardenPatch.Queries;

[Serializable]
internal class InvalidQueryException<TQuery> : Exception
{
    public InvalidQueryException(TQuery query, Dictionary<string, IEnumerable<string>> errors)
    {
        Query = query;
        Errors = errors;
    }

    public TQuery Query { get; }

    public Dictionary<string, IEnumerable<string>> Errors { get; }

    override public string Message => $"Validation errors encountered with ${typeof(TQuery).FullName}";
}
