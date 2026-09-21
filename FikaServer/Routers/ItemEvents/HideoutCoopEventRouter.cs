using FikaServer.Callbacks;
using FikaServer.Models.Fika;
using FikaServer.Models.Fika.HideoutCoop;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.DI.Routing;

namespace FikaServer.Routers.ItemEvents;

[Injectable(TypePriority = OnLoadOrder.Routers)]
public sealed class HideoutCoopEventRouter(HideoutCoopCallbacks hideoutCoopCallbacks)
    : ItemEventRouter([
        new ItemRouteAction<FikaHideoutCoopSpendRequest>(
            FikaItemEventRouter.HIDEOUT_COOP_SPEND,
            async (url, pmcData, body, sessionID, output, cancellationToken) =>
                await hideoutCoopCallbacks.HandleSpend(body, sessionID)
        ),
        new ItemRouteAction<FikaHideoutCoopApplyRequest>(
            FikaItemEventRouter.HIDEOUT_COOP_APPLY_UPGRADE,
            async (url, pmcData, body, sessionID, output, cancellationToken) =>
                await hideoutCoopCallbacks.HandleApplyUpgrade(body, sessionID)
        ),
        new ItemRouteAction<FikaHideoutCoopCompleteRequest>(
            FikaItemEventRouter.HIDEOUT_COOP_COMPLETE,
            async (url, pmcData, body, sessionID, output, cancellationToken) =>
                await hideoutCoopCallbacks.HandleComplete(body, sessionID)
        ),
        new ItemRouteAction<FikaHideoutCoopRefundRequest>(
            FikaItemEventRouter.HIDEOUT_COOP_REFUND,
            async (url, pmcData, body, sessionID, output, cancellationToken) =>
                await hideoutCoopCallbacks.HandleRefund(body, sessionID)
        )
    ]);
