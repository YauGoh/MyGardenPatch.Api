using FluentValidation.Results;

namespace MyGardenPatch.Queries;

[Serializable]
internal class InvalidQueryException<TQuery> : Exception
{
    public InvalidQueryException(TQuery query, List<ValidationFailure> validationErrors)
    {
        Query = query;
        ValidationErrors = validationErrors;
    }

    public TQuery Query { get; }

    public List<ValidationFailure> ValidationErrors { get; }

    override public string Message => $"Validation errors encountered with ${typeof(TQuery).FullName}";
}
