using Microsoft.AspNetCore.Mvc;
using MyGardenPatch.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGardenPatch.WebApiExtensions.Commands;

[ApiController]
[GenericCommandController]
[Route("commands/[controller]")]
internal class GenericCommandController<TCommand> : ControllerBase
        where TCommand : ICommand
{
    private readonly ICommandExecutor _commander;

    public GenericCommandController(ICommandExecutor commander)
    {
        _commander = commander;
    }

    [HttpPost]
    public Task PostAsync(TCommand command, CancellationToken cancellationToken) => _commander.HandleAsync(command, cancellationToken);
}

