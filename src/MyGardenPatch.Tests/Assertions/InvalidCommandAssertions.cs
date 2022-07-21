namespace MyGardenPatch.Tests.Assertions;

internal static class InvalidCommandExtensions
{
    internal static Task<ExceptionAssertions<InvalidCommandException<TCommand>>> WhereHasError<TCommand>(
        this Task<ExceptionAssertions<InvalidCommandException<TCommand>>> exceptionAssertion,
        string validationError,
        string validaionPropertyPath,
        string because = "",
        params object[] becauseArgs) where TCommand : ICommand
    {
        return exceptionAssertion
            .Where(
                ex => ShouldMatch(
                    ex, 
                    validationError, 
                    validaionPropertyPath),
                because,
                becauseArgs);
    }

    private static bool ShouldMatch<T>(InvalidCommandException<T> ex, string validationError, string validaionPropertyPath)
    {
        string? error = null;

        if (ex.Errors.TryGetValue(validaionPropertyPath, out var errors))
            error = errors.First();

        error.Should().NotBeNull(
            $"Expected error with Property path: {validaionPropertyPath}\r\nFound: {string.Join("; ", ex.Errors.SelectMany(e => e.Value))}");

        error.Should().Be(validationError);

        return true;
    }

    internal static Task<ExceptionAssertions<InvalidQueryException<TQuery>>> WhereHasError<TQuery>(
        this Task<ExceptionAssertions<InvalidQueryException<TQuery>>> exceptionAssertion,
        string validationError,
        string validaionPropertyPath,
        string because = "",
        params object[] becauseArgs)
    {
        return exceptionAssertion
            .Where(
                ex => ex.Errors
                    .Any(e => e.Key == validaionPropertyPath && 
                              e.Value.Contains(validationError)),
                because,
                becauseArgs);
    }
}
