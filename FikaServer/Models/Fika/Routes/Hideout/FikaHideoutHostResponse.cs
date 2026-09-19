using System.Text.Json.Serialization;

namespace FikaServer.Models.Fika.Routes.Hideout;

public record FikaHideoutHostResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("ips")]
    public string[] Ips { get; set; } = [];

    [JsonPropertyName("serverGuid")]
    public Guid ServerGuid { get; set; }

    [JsonPropertyName("port")]
    public ushort Port { get; set; }

    [JsonPropertyName("natPunch")]
    public bool NatPunch { get; set; }

    [JsonPropertyName("useFikaNatPunchServer")]
    public bool UseFikaNatPunchServer { get; set; }

    [JsonPropertyName("isHeadless")]
    public bool IsHeadless { get; set; }
}
