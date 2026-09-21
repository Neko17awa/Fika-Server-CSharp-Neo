using SPTarkov.Server.Core.Models.Eft.Common.Request;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums.Hideout;
using System.Text.Json.Serialization;

namespace FikaServer.Models.Fika.HideoutCoop;

public sealed record FikaHideoutCoopSpendRequest : BaseInteractionRequestData
{
    [JsonPropertyName("items")]
    public List<HideoutItem>? Items { get; set; }
}

public sealed record FikaHideoutCoopApplyRequest : BaseInteractionRequestData
{
    [JsonPropertyName("areaType")]
    public HideoutAreas? AreaType { get; set; }
}

public sealed record FikaHideoutCoopCompleteRequest : BaseInteractionRequestData
{
    [JsonPropertyName("areaType")]
    public HideoutAreas? AreaType { get; set; }
}

public sealed record FikaHideoutCoopRefundRequest : BaseInteractionRequestData
{
    [JsonPropertyName("items")]
    public List<HideoutItem>? Items { get; set; }
}
