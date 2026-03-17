namespace CitizenRegistryApi.Utils;

public static class CsvHelper
{
    public static List<string[]> ReadCsv(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new List<string[]>();
        }

        return File.ReadAllLines(filePath).Where(line => !string.IsNullOrWhiteSpace(line)).Select(line => line.Split(',')).ToList();
    }

    public static void WriteCsv(string filePath, List<string[]> data)
    {
        var lines = data.Select(values => string.Join(",", values)).ToArray();
        File.WriteAllLines(filePath, lines);
    }
}