using FikaServer.Models.Fika.Routes.Hideout;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Helpers.Profile;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Common.Models.Logging;

namespace FikaServer.Services;

/// <summary>
/// 本服藏身处参观：按 accountId 抽取客人快照。找不到目标时返回 Ok=false，不回退查看者自己。
/// </summary>
[Injectable]
public class HideoutViewService(ProfileHelper profileHelper, ISptLogger<HideoutViewService> logger)
{
    public FikaHideoutViewResponse View(string? accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
        {
            return new FikaHideoutViewResponse { Ok = false };
        }

        var profileToView = profileHelper.GetFullProfileByAccountId(accountId);
        var pmc = profileToView?.CharacterData?.PmcData;
        if (pmc?.Hideout is null || pmc.Inventory is null)
        {
            logger.Warning($"[Fika] hideout view: profile {accountId} not found locally (no self-fallback)");
            return new FikaHideoutViewResponse { Ok = false };
        }

        var hideoutKeys = new HashSet<string>();
        if (pmc.Inventory.HideoutAreaStashes is not null)
        {
            hideoutKeys.UnionWith(pmc.Inventory.HideoutAreaStashes.Keys);
        }

        if (pmc.Inventory.HideoutCustomizationStashId is { } stashId)
        {
            hideoutKeys.Add(stashId.ToString());
        }

        var itemsToReturn = new List<Item>();
        var inventoryItems = pmc.Inventory.Items;
        if (inventoryItems is not null)
        {
            var hideoutRootItems = inventoryItems.Where(x => hideoutKeys.Contains(x.Id));
            foreach (var rootItem in hideoutRootItems)
            {
                itemsToReturn.AddRange(inventoryItems.GetItemWithChildren(rootItem.Id));
            }
        }

        return new FikaHideoutViewResponse
        {
            Ok = true,
            Aid = pmc.Aid,
            Info = new OtherProfileInfo
            {
                Nickname = pmc.Info?.Nickname,
                Side = pmc.Info?.Side,
                Experience = pmc.Info?.Experience,
                MemberCategory = (int)(pmc.Info?.MemberCategory ?? MemberCategory.Default),
            },
            Hideout = pmc.Hideout,
            CustomizationStash = pmc.Inventory.HideoutCustomizationStashId?.ToString(),
            HideoutAreaStashes = pmc.Inventory.HideoutAreaStashes ?? [],
            Items = itemsToReturn,
        };
    }
}
