using CitizenRegistryApi.Dtos;
using CitizenRegistryApi.Models;
using CitizenRegistryApi.Services;
using CitizenRegistryApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CitizenRegistryApi.Controllers;

[ApiController]
[Route("api/citizen")]
public class CitizenController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ObjectService _objectService;
    private readonly List<Citizen> _citizens;

    public CitizenController(IConfiguration configuration, ObjectService objectService)
    {
        _configuration = configuration;
        _objectService = objectService;
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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCitizen request)
    {
        try
        {
            bool citizenExists = _citizens.Any(c => c.CI == request.CI);

            if (citizenExists)
            {
                return BadRequest(new { message = "Citizen with this CI already exists" });
            }

            Citizen citizen = new Citizen
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                CI = request.CI,
                BloodGroup = GetRandomBloodGroup(),
                PersonalAsset = await _objectService.GetRandomObjectName()
            };

            _citizens.Add(citizen);
            SaveCitizens();

            return CreatedAtAction(nameof(GetByCi), new { ci = citizen.CI }, citizen);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
    
    [HttpPut("{ci}")]
    public IActionResult Update(string ci, [FromBody] UpdateCitizen request)
    {
        try
        {
            Citizen? citizen = _citizens.FirstOrDefault(c => c.CI == ci);

            if (citizen == null)
            {
                return NotFound(new { message = "Citizen not found" });
            }

            citizen.FirstName = request.FirstName;
            citizen.LastName = request.LastName;

            SaveCitizens();

            return Ok(citizen);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    private List<Citizen> LoadCitizens()
    {
        string? filePath = _configuration["Data:Location"];

        if (string.IsNullOrWhiteSpace(filePath))
        {
            filePath = "CitizensData.csv";
        }

        var data = CsvHelper.ReadCsv(filePath);
        var citizens = new List<Citizen>();

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].Length < 5)
            {
                continue;
            }

            Citizen citizen = new Citizen
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

    private void SaveCitizens()
    {
        string? filePath = _configuration["Data:Location"];

        if (string.IsNullOrWhiteSpace(filePath))
        {
            filePath = "CitizensData.csv";
        }

        var data = _citizens.Select(c => new[]
        {
            c.FirstName,
            c.LastName,
            c.CI,
            c.BloodGroup,
            c.PersonalAsset
        }).ToList();

        CsvHelper.WriteCsv(filePath, data);
    }

    private static string GetRandomBloodGroup()
    {
        string[] bloodGroups =
        {
            "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-"
        };

        int randomIndex = Random.Shared.Next(bloodGroups.Length);
        return bloodGroups[randomIndex];
    }
}