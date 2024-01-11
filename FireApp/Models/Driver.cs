using Newtonsoft.Json;

namespace FireApp.Models;

public class Driver
{
    [JsonProperty("id")] public string Id { get; set; } = null!;
    [JsonProperty("name")] public string Name { get; set; } = "";
    [JsonProperty("number")] public int Number { get; set; }
    [JsonProperty("status")] public int Status { get; set; }
}