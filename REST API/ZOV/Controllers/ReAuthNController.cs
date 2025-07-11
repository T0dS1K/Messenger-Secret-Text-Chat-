using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ReAuthNController : ControllerBase
{
    [HttpGet]
    public IActionResult GetResource()
    {
        return Ok();
    }
}