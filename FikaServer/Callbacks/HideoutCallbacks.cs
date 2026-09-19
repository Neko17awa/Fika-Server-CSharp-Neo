using FikaServer.Models.Fika.Routes.Hideout;
using FikaServer.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers.Profile;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
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
        hideoutHostService.SetHost(info, HostAliases(sessionID, info.AccountId, info.Aliases));
        return new ValueTask<string>(httpResponseUtil.NullResponse());
    }

    /// <summary>
    /// Handle /fika/hideout/gethost
    /// </summary>
    public ValueTask<string> HandleHideoutGetHost(string url, FikaHideoutHostRequest info, MongoId sessionID)
    {
        var host = hideoutHostService.GetHost(info.AccountId);
        if (!host.Ok)
        {
            foreach (var key in HostLookupKeys(info.AccountId))
            {
                if (string.Equals(key, info.AccountId, StringComparison.Ordinal))
                {
                    continue;
                }

                host = hideoutHostService.GetHost(key);
                if (host.Ok)
                {
                    break;
                }
            }
        }

        return new ValueTask<string>(httpResponseUtil.NoBody(host));
    }

    /// <summary>
    /// Handle /fika/hideout/host/leave
    /// </summary>
    public ValueTask<string> HandleHideoutHostLeave(string url, FikaHideoutHostRequest info, MongoId sessionID)
    {
        hideoutHostService.Leave(info.AccountId);
        return new ValueTask<string>(httpResponseUtil.NullResponse());
    }

    private string[] HostAliases(MongoId sessionId, string? accountId, string[]? extra)
    {
        var keys = new HashSet<string>(StringComparer.Ordinal);
        AddKeys(keys, accountId);
        AddProfileKeys(keys, profileHelper.GetPmcProfile(sessionId));
        AddFullProfileKeys(keys, profileHelper.GetFullProfileByAccountId(accountId ?? ""));
        if (extra != null)
        {
            foreach (var key in extra)
            {
                AddKeys(keys, key);
            }
        }

        return [.. keys];
    }

    private string[] HostLookupKeys(string? accountId)
    {
        var keys = new HashSet<string>(StringComparer.Ordinal);
        AddKeys(keys, accountId);
        AddFullProfileKeys(keys, profileHelper.GetFullProfileByAccountId(accountId ?? ""));
        return [.. keys];
    }

    private static void AddFullProfileKeys(HashSet<string> keys, SPTarkov.Server.Core.Models.Eft.Profile.SptProfile? profile)
    {
        if (profile == null)
        {
            return;
        }

        if (profile.ProfileInfo?.Aid is int infoAid)
        {
            AddKeys(keys, infoAid.ToString());
        }

        if (profile.ProfileInfo?.ProfileId is { } profileId)
        {
            AddKeys(keys, profileId.ToString());
        }

        AddProfileKeys(keys, profile.CharacterData?.PmcData);
    }

    private static void AddProfileKeys(HashSet<string> keys, PmcData? pmc)
    {
        if (pmc == null)
        {
            return;
        }

        if (pmc.Aid is int aid)
        {
            AddKeys(keys, aid.ToString());
        }

        if (pmc.Id is { } id)
        {
            AddKeys(keys, id.ToString());
        }
    }

    private static void AddKeys(HashSet<string> keys, string? key)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            keys.Add(key);
        }
    }
}
