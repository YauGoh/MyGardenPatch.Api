﻿using Microsoft.Extensions.Logging;

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
        }

        private static object? GetInnerException(Exception? ex)
        {
            if (ex == null) return null;

            return new
            {
                ex.Message,
                InnerExceptopn = GetInnerException(ex.InnerException)
            };
        }
    }
}
