using Microsoft.AspNetCore.Mvc;
using CitizenRegistryApi.Models;

namespace CitizenRegistryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitizenController : ControllerBase
{
    private static readonly List<Citizen> citizens = new();

    [HttpGet]
    public ActionResult<IEnumerable<Citizen>> GetAll()
    {
        return Ok(citizens);
    }
}