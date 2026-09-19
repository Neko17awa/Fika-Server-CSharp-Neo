using FikaServer.Callbacks;
using FikaServer.Models.Fika.Routes.Hideout;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Utils;

namespace FikaServer.Routers.Static;

[Injectable]
public class HideoutStaticRouter(HideoutCallbacks fikaHideoutCallbacks, JsonUtil jsonUtil) : StaticRouter(jsonUtil, [
        new RouteAction<FikaHideoutViewRequest>(
            "/fika/hideout/view",
            async (
                url,
                info,
                sessionId,
                output,
                cancellationToken
            ) => await fikaHideoutCallbacks.HandleHideoutView(url, info, sessionId)
            ),
    ])
{
}
