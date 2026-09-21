using FikaServer.Models.Fika.HideoutCoop;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Controllers;
using SPTarkov.Server.Core.Helpers.Commerce;
using SPTarkov.Server.Core.Helpers.Profile;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Hideout;
using SPTarkov.Server.Core.Models.Eft.Inventory;
using SPTarkov.Server.Core.Models.Eft.ItemEvent;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Utils;

namespace FikaServer.Controllers;

/// <summary>
/// 共同建造：Spend 只扣当前 session 仓库；Apply/Complete 只改当前 session 的藏身处，不扣主人物品。
/// </summary>
[Injectable]
public class HideoutCoopController(
    ISptLogger<HideoutCoopController> logger,
    EventOutputHolder eventOutputHolder,
    HttpResponseUtil httpResponseUtil,
    ProfileHelper profileHelper,
    InventoryHelper inventoryHelper,
    PaymentHelper paymentHelper,
    HideoutController hideoutController,
    HideoutTable hideoutTable,
    TimeUtil timeUtil)
{
    public ItemEventRouterResponse Spend(FikaHideoutCoopSpendRequest body, MongoId sessionId)
    {
        var output = eventOutputHolder.GetOutput(sessionId);
        var pmcData = profileHelper.GetPmcProfile(sessionId);
        if (pmcData?.Inventory?.Items == null)
        {
            return httpResponseUtil.AppendErrorToOutput(output, "PMC inventory not found");
        }

        foreach (var requested in body.Items ?? [])
        {
            var inventoryItem = pmcData.Inventory.Items.FirstOrDefault(item => item.Id == requested.Id);
            if (inventoryItem is null)
            {
                logger.Error($"Fika hideout coop spend could not find item {requested.Id}");
                return httpResponseUtil.AppendErrorToOutput(output, "Item not found in inventory");
            }

            var requestedCount = requested.Count ?? 1;
            if (paymentHelper.IsMoneyTpl(inventoryItem.Template)
                && inventoryItem.Upd?.StackObjectsCount is not null
                && inventoryItem.Upd.StackObjectsCount > requestedCount)
            {
                inventoryItem.Upd.StackObjectsCount -= requestedCount;
                continue;
            }

            inventoryHelper.RemoveItem(pmcData, inventoryItem.Id, sessionId, output);
            if (output.Warnings?.Count > 0)
            {
                return output;
            }
        }

        return output;
    }

    public ItemEventRouterResponse ApplyUpgrade(FikaHideoutCoopApplyRequest body, MongoId sessionId)
    {
        var output = eventOutputHolder.GetOutput(sessionId);
        var pmcData = profileHelper.GetPmcProfile(sessionId);
        if (pmcData?.Hideout?.Areas is null || body.AreaType is null)
        {
            return httpResponseUtil.AppendErrorToOutput(output, "Hideout area not found");
        }

        var profileHideoutArea = pmcData.Hideout.Areas.FirstOrDefault(area => area.Type == body.AreaType);
        if (profileHideoutArea is null)
        {
            return httpResponseUtil.AppendErrorToOutput(output, "Hideout area not found");
        }

        var hideoutDataDb = hideoutTable.Areas.FirstOrDefault(area => area.Type == body.AreaType);
        if (hideoutDataDb is null)
        {
            return httpResponseUtil.AppendErrorToOutput(output, "Hideout area not in database");
        }

        var nextLevel = (profileHideoutArea.Level ?? 0) + 1;
        if (hideoutDataDb.Stages is null || !hideoutDataDb.Stages.TryGetValue(nextLevel.ToString(), out var stage))
        {
            return httpResponseUtil.AppendErrorToOutput(output, "Hideout stage not found");
        }

        var ctime = stage.ConstructionTime;
        if (ctime > 0)
        {
            if (profileHelper.IsDeveloperAccount(sessionId))
            {
                ctime = 40;
            }

            profileHideoutArea.CompleteTime = (int)Math.Round(timeUtil.GetTimeStamp() + ctime.Value);
            profileHideoutArea.Constructing = true;
        }

        return output;
    }

    public ItemEventRouterResponse Complete(FikaHideoutCoopCompleteRequest body, MongoId sessionId)
    {
        var output = eventOutputHolder.GetOutput(sessionId);
        var pmcData = profileHelper.GetPmcProfile(sessionId);
        if (pmcData is null || body.AreaType is null)
        {
            return httpResponseUtil.AppendErrorToOutput(output, "Hideout area not found");
        }

        hideoutController.UpgradeComplete(
            pmcData,
            new HideoutUpgradeCompleteRequestData
            {
                Action = "HideoutUpgradeComplete",
                AreaType = body.AreaType
            },
            sessionId,
            output);

        return output;
    }

    public ItemEventRouterResponse Refund(FikaHideoutCoopRefundRequest body, MongoId sessionId)
    {
        var output = eventOutputHolder.GetOutput(sessionId);
        var pmcData = profileHelper.GetPmcProfile(sessionId);
        if (pmcData is null)
        {
            return httpResponseUtil.AppendErrorToOutput(output, "PMC profile not found");
        }

        foreach (var requested in body.Items ?? [])
        {
            if (requested.Template.IsEmpty)
            {
                continue;
            }

            var count = Math.Max(1, requested.Count ?? 1);
            var item = new Item
            {
                Id = new MongoId(),
                Template = requested.Template,
                Upd = new Upd { StackObjectsCount = count }
            };

            inventoryHelper.AddItemToStash(
                sessionId,
                new AddItemDirectRequest
                {
                    ItemWithModsToAdd = [item],
                    FoundInRaid = false,
                    Callback = null,
                    UseSortingTable = true
                },
                pmcData,
                output);

            if (output.Warnings?.Count > 0)
            {
                return output;
            }
        }

        return output;
    }
}
