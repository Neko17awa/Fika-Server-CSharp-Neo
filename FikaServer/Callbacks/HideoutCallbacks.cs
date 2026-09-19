using FikaServer.Models.Fika.Routes.Hideout;
using FikaServer.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers.Profile;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Utils;

namespace FikaServer.Callbacks;

[Injectable]
public class HideoutCallbacks(
    HttpResponseUtil httpResponseUtil,
    HideoutViewService hideoutViewService,
    HideoutHostService hideoutHostService,
    ProfileHelper profileHelper)
{
    /// <summary>
    /// Handle /fika/hideout/view
    /// </summary>
    public ValueTask<string> HandleHideoutView(string url, FikaHideoutViewRequest info, MongoId sessionID)
    {
        return new ValueTask<string>(httpResponseUtil.NoBody(hideoutViewService.View(info.AccountId)));
    }

    /// <summary>
    /// Handle /fika/hideout/host
    /// </summary>
    public ValueTask<string> HandleHideoutHost(string url, FikaHideoutHostRequest info, MongoId sessionID)
    {
        hideoutHostService.SetHost(info, HostAliases(sessionID, info.AccountId));
        return new ValueTask<string>(httpResponseUtil.NullResponse());
    }

    /// <summary>
    /// Handle /fika/hideout/gethost
    /// </summary>
    public ValueTask<string> HandleHideoutGetHost(string url, FikaHideoutHostRequest info, MongoId sessionID)
    {
        return new ValueTask<string>(httpResponseUtil.NoBody(hideoutHostService.GetHost(info.AccountId)));
    }

    /// <summary>
    /// Handle /fika/hideout/host/leave
    /// </summary>
    public ValueTask<string> HandleHideoutHostLeave(string url, FikaHideoutHostRequest info, MongoId sessionID)
    {
        hideoutHostService.Leave(info.AccountId);
        return new ValueTask<string>(httpResponseUtil.NullResponse());
    }

    private string[] HostAliases(MongoId sessionId, string? accountId)
    {
        var keys = new HashSet<string>(StringComparer.Ordinal);
        if (!string.IsNullOrWhiteSpace(accountId))
        {
            keys.Add(accountId);
        }

        var pmc = profileHelper.GetPmcProfile(sessionId);
        if (pmc?.Aid is int aid)
        {
            keys.Add(aid.ToString());
        }

        return [.. keys];
    }
}
