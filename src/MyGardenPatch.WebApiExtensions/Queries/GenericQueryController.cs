namespace MyGardenPatch.WebApiExtensions.Queries;

[ApiController]
[GenericQueryController]
[Route("queries/[controller]")]
internal class GenericQueryController<TQuery, TResult> : ControllerBase
    where TQuery : IQuery<TResult>
{
    private readonly IQueryExecutor _queryExecutor;

    public GenericQueryController(IQueryExecutor queryExecutor)
    {
        _queryExecutor = queryExecutor;
    }

    [HttpPost]
    public async Task<TResult> PostAsync(TQuery query) =>await _queryExecutor.HandleAsync(query);
}
