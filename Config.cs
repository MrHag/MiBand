using System.Text.Json;
using System.Text.Json.Serialization;

public class Config
{
    [JsonRequired]
    public string Key { get; set; }

    public Config()
    {
        Key = "deviceKey";
    }

    public static Config Load(string file)
    {
        try
        {
            using (StreamReader r = new StreamReader(file))
            {
                string json = r.ReadToEnd();
                var opt = new JsonSerializerOptions();
                return JsonSerializer.Deserialize<Config>(json);
            }
        }
        catch
        {
            using (StreamWriter r = new StreamWriter(file))
            {
                string json = JsonSerializer.Serialize(new Config());
                r.Write(json);
                throw;
            }
        }
    }
}