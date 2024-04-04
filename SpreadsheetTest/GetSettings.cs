using Newtonsoft.Json;
using SpreadsheetTest.Models;

namespace SpreadsheetTest;

public static class GetSettings
{
    public static Authentication GetAuthSettings()
    {
        var root = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(root, "client.secrets.json");
        if (!File.Exists(filePath))
            return null;

        return JsonConvert.DeserializeObject<Authentication>(File.ReadAllText(filePath));
    }
}