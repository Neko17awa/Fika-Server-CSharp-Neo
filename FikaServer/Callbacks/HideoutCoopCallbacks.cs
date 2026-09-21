using FikaServer.Controllers;
using FikaServer.Models.Fika.HideoutCoop;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.ItemEvent;

namespace FikaServer.Callbacks;

[Injectable]
public class HideoutCoopCallbacks(HideoutCoopController hideoutCoopController)
{
    public ValueTask<ItemEventRouterResponse> HandleSpend(FikaHideoutCoopSpendRequest body, MongoId sessionId)
    {
        return new ValueTask<ItemEventRouterResponse>(hideoutCoopController.Spend(body, sessionId));
    }

    public ValueTask<ItemEventRouterResponse> HandleApplyUpgrade(FikaHideoutCoopApplyRequest body, MongoId sessionId)
    {
        return new ValueTask<ItemEventRouterResponse>(hideoutCoopController.ApplyUpgrade(body, sessionId));
    }

    public ValueTask<ItemEventRouterResponse> HandleComplete(FikaHideoutCoopCompleteRequest body, MongoId sessionId)
    {
        return new ValueTask<ItemEventRouterResponse>(hideoutCoopController.Complete(body, sessionId));
    }

    public ValueTask<ItemEventRouterResponse> HandleRefund(FikaHideoutCoopRefundRequest body, MongoId sessionId)
    {
        return new ValueTask<ItemEventRouterResponse>(hideoutCoopController.Refund(body, sessionId));
    }
}
