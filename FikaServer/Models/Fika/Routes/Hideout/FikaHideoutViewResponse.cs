using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Profile;
using System.Text.Json.Serialization;
using HideoutState = SPTarkov.Server.Core.Models.Eft.Common.Tables.Hideout;

namespace FikaServer.Models.Fika.Routes.Hideout;

/// <summary>
/// 本服藏身处参观快照。字段形状对齐 <see cref="GetOtherProfileResponse"/> 的 hideout 段，
/// 找不到目标档案时 <see cref="Ok"/> 为 false，且不回退到查看者自己的藏身处。
/// </summary>
public record FikaHideoutViewResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("aid")]
    public int? Aid { get; set; }

    [JsonPropertyName("info")]
    public OtherProfileInfo? Info { get; set; }

    [JsonPropertyName("hideout")]
    public HideoutState? Hideout { get; set; }

    [JsonPropertyName("customizationStash")]
    public string? CustomizationStash { get; set; }

    [JsonPropertyName("hideoutAreaStashes")]
    public Dictionary<string, MongoId>? HideoutAreaStashes { get; set; }

    [JsonPropertyName("items")]
    public List<Item>? Items { get; set; }
}
