using Microsoft.AspNetCore.Mvc;
using CitizenRegistryApi.Models;
using CitizenRegistryApi.Utils;

namespace CitizenRegistryApi.Controllers;

[ApiController]
[Route("api/citizen")]
public class CitizenController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly List<Citizen> _citizens;

    public CitizenController(IConfiguration configuration)
    {
        _configuration = configuration;
        _citizens = LoadCitizens();
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_citizens);
    }

    [HttpGet("{ci}")]
    public IActionResult GetByCi(string ci)
    {
        var citizen = _citizens.FirstOrDefault(c => c.CI == ci);

        if (citizen == null)
        {
            return NotFound(new { message = "Citizen not found" });
        }

        return Ok(citizen);
    }

    private List<Citizen> LoadCitizens()
    {
        var filePath = _configuration["Data:Location"] ?? "CitizensData.csv";
        var data = CsvHelper.ReadCsv(filePath);
        var citizens = new List<Citizen>();

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].Length < 5)
            {
                continue;
            }

            var citizen = new Citizen
            {
                FirstName = data[i][0],
                LastName = data[i][1],
                CI = data[i][2],
                BloodGroup = data[i][3],
                PersonalAsset = data[i][4]
            };

            citizens.Add(citizen);
        }

        return citizens;
    }
}