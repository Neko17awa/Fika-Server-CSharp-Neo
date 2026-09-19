using FikaServer.Models.Fika.Routes.Hideout;
using FikaServer.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Utils;

namespace FikaServer.Callbacks;

[Injectable]
public class HideoutCallbacks(HttpResponseUtil httpResponseUtil, HideoutViewService hideoutViewService)
{
    /// <summary>
    /// Handle /fika/hideout/view
    /// </summary>
    public ValueTask<string> HandleHideoutView(string url, FikaHideoutViewRequest info, MongoId sessionID)
    {
        return new ValueTask<string>(httpResponseUtil.NoBody(hideoutViewService.View(info.AccountId)));
    }
}
