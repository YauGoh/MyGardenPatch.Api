using Microsoft.AspNetCore.Mvc;

namespace MyGardenPatch.Webapi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AboutController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var user = new
        {
            Name = User.Identity.Name,
            Scheme = User.Identity.AuthenticationType,
            Claims = User.Claims.Select(c => new
            {
                Type = c.Type,
                Value = c.Value,
                ValueType = c.ValueType,
            }).ToList()
        };

        return Ok(user);
    }
}
