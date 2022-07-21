namespace MyGardenPatch.WebApiExtensions
{
    internal class ExceptionHandler
    {
        public static async Task<IResult> Try<TCommand>(Func<Task> command) where TCommand : ICommand
        {
            try
            {
                await command.Invoke()!;

                return Results.Ok();
            }
            catch(InvalidCommandException<TCommand> tex)
            {
                return Results.ValidationProblem(tex.Errors, title:tex.Message);
            }
            catch(Exception ex)
            {
                return Results.BadRequest(ex);
            }
        }
    }
}
