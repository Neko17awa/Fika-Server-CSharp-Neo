using SPTarkov.Server.Core.Models.Utils;
using System.Text.Json.Serialization;

namespace FikaServer.Models.Fika.Routes.Hideout;

public record FikaHideoutHostRequest : IRequestData
{
    [JsonPropertyName("accountId")]
    public string? AccountId { get; set; }

    [JsonPropertyName("ips")]
    public string[]? Ips { get; set; }

    [JsonPropertyName("port")]
    public ushort Port { get; set; }

    [JsonPropertyName("serverGuid")]
    public string? ServerGuid { get; set; }

    [JsonPropertyName("natPunch")]
    public bool NatPunch { get; set; }

    [JsonPropertyName("useFikaNatPunchServer")]
    public bool UseFikaNatPunchServer { get; set; }
}
