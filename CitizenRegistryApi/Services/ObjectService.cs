using CitizenRegistryApi.Models;
using Newtonsoft.Json;

namespace CitizenRegistryApi.Services;

public class ObjectService
{
    private HttpClient _httpClient;

    public ObjectService(IConfiguration configuration)
    {
        _httpClient = new HttpClient();

        string? baseUrl = configuration["ExternalServices:ObjectsApi:BaseUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new Exception("ObjectsApi BaseUrl is not configured.");
        }

        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    public async Task<string> GetRandomObjectName()
    {
        try
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "objects");
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                List<ExternalObject>? objects =
                    JsonConvert.DeserializeObject<List<ExternalObject>>(responseContent);

                if (objects == null || objects.Count == 0)
                {
                    throw new Exception("No objects were returned by the external API.");
                }

                int randomIndex = Random.Shared.Next(objects.Count);
                return objects[randomIndex].Name;
            }
            else
            {
                throw new Exception("Error while getting objects from external API.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("ObjectService failed: " + ex.Message);
        }
    }
}