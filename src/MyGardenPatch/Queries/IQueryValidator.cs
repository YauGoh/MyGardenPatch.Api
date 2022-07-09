using FluentValidation;

namespace MyGardenPatch.Queries;

public interface IQueryValidator<TQuery> : IValidator<TQuery>
{
}
