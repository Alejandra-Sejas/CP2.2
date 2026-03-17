namespace CitizenRegistryApi.Dtos;

public class CreateCitizen
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string CI { get; set; } = string.Empty;
}