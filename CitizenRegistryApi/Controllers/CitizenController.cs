using CitizenRegistryApi.Dtos;
using CitizenRegistryApi.Models;
using CitizenRegistryApi.Services;
using CitizenRegistryApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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

        Log.Debug("CitizenController initialized. Citizens loaded: {Count}", _citizens.Count);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        Log.Debug("Returning all citizens. Count: {Count}", _citizens.Count);
        return Ok(_citizens);
    }

    [HttpGet("{ci}")]
    public IActionResult GetByCi(string ci)
    {
        Log.Debug("Searching citizen by CI {CI}", ci);

        var citizen = _citizens.FirstOrDefault(c => c.CI == ci);

        if (citizen == null)
        {
            Log.Warning("Citizen with CI {CI} was not found", ci);
            return NotFound(new { message = "Citizen not found" });
        }

        Log.Information("Citizen with CI {CI} was found", ci);
        return Ok(citizen);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCitizen request)
    {
        try
        {
            Log.Debug("Creating citizen with CI {CI}", request.CI);

            bool citizenExists = _citizens.Any(c => c.CI == request.CI);

            if (citizenExists)
            {
                Log.Warning("Citizen with CI {CI} already exists", request.CI);
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

            Log.Information("Citizen created successfully with CI {CI}", citizen.CI);

            return CreatedAtAction(nameof(GetByCi), new { ci = citizen.CI }, citizen);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while creating citizen with CI {CI}", request.CI);
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("{ci}")]
    public IActionResult Update(string ci, [FromBody] UpdateCitizen request)
    {
        try
        {
            Log.Debug("Updating citizen with CI {CI}", ci);

            Citizen? citizen = _citizens.FirstOrDefault(c => c.CI == ci);

            if (citizen == null)
            {
                Log.Warning("Citizen with CI {CI} was not found for update", ci);
                return NotFound(new { message = "Citizen not found" });
            }

            citizen.FirstName = request.FirstName;
            citizen.LastName = request.LastName;

            SaveCitizens();

            Log.Information("Citizen updated successfully with CI {CI}", ci);

            return Ok(citizen);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while updating citizen with CI {CI}", ci);
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{ci}")]
    public IActionResult Delete(string ci)
    {
        try
        {
            Log.Debug("Deleting citizen with CI {CI}", ci);

            Citizen? citizen = _citizens.FirstOrDefault(c => c.CI == ci);

            if (citizen == null)
            {
                Log.Warning("Citizen with CI {CI} was not found for deletion", ci);
                return NotFound(new { message = "Citizen not found" });
            }

            _citizens.Remove(citizen);
            SaveCitizens();

            Log.Information("Citizen deleted successfully with CI {CI}", ci);

            return Ok(new { message = "Citizen deleted successfully" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while deleting citizen with CI {CI}", ci);
            return StatusCode(500, new { message = ex.Message });
        }
    }

    private List<Citizen> LoadCitizens()
    {
        try
        {
            Log.Debug("Loading citizens from CSV");

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
                    Log.Warning("Invalid CSV row skipped at index {Index}", i);
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

            Log.Information("Citizens loaded successfully. Count: {Count}", citizens.Count);

            return citizens;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while loading citizens from CSV");
            throw;
        }
    }

    private void SaveCitizens()
    {
        try
        {
            Log.Debug("Saving citizens to CSV. Count: {Count}", _citizens.Count);

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

            Log.Information("Citizens saved successfully to CSV");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while saving citizens to CSV");
            throw;
        }
    }

    private static string GetRandomBloodGroup()
    {
        string[] bloodGroups =
        {
            "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-"
        };

        int randomIndex = Random.Shared.Next(bloodGroups.Length);
        string selectedBloodGroup = bloodGroups[randomIndex];

        Log.Debug("Random blood group selected: {BloodGroup}", selectedBloodGroup);

        return selectedBloodGroup;
    }
}