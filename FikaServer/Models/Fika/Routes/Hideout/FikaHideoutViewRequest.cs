using SPTarkov.Server.Core.Models.Utils;
using System.Text.Json.Serialization;

namespace FikaServer.Models.Fika.Routes.Hideout;

public record FikaHideoutViewRequest : IRequestData
{
    [JsonPropertyName("accountId")]
    public string? AccountId { get; set; }
}
