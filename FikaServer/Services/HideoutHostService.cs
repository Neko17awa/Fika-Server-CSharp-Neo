using System.Collections.Concurrent;
using FikaServer.Models.Fika.Routes.Hideout;
using SPTarkov.DI.Annotations;

namespace FikaServer.Services;

/// <summary>
/// 本服藏身处 Host 地址。按主人 accountId 登记，不进入战局大厅。
/// </summary>
[Injectable(InjectionType.Singleton)]
public class HideoutHostService
{
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(45);
    private readonly ConcurrentDictionary<string, HideoutHostEntry> _hosts = new(StringComparer.Ordinal);

    public void SetHost(FikaHideoutHostRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AccountId))
        {
            return;
        }

        Guid.TryParse(request.ServerGuid, out var guid);
        _hosts[request.AccountId] = new HideoutHostEntry
        {
            Ips = request.Ips ?? [],
            Port = request.Port,
            ServerGuid = guid,
            NatPunch = request.NatPunch,
            UseFikaNatPunchServer = request.UseFikaNatPunchServer,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public FikaHideoutHostResponse GetHost(string? accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
        {
            return new FikaHideoutHostResponse { Ok = false };
        }

        if (!_hosts.TryGetValue(accountId, out var entry))
        {
            return new FikaHideoutHostResponse { Ok = false };
        }

        if (DateTime.UtcNow - entry.UpdatedAt > Ttl)
        {
            _hosts.TryRemove(accountId, out _);
            return new FikaHideoutHostResponse { Ok = false };
        }

        return new FikaHideoutHostResponse
        {
            Ok = true,
            Ips = entry.Ips,
            Port = entry.Port,
            ServerGuid = entry.ServerGuid,
            NatPunch = entry.NatPunch,
            UseFikaNatPunchServer = entry.UseFikaNatPunchServer,
            IsHeadless = false
        };
    }

    public void Leave(string? accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
        {
            return;
        }

        _hosts.TryRemove(accountId, out _);
    }

    private sealed class HideoutHostEntry
    {
        public string[] Ips { get; set; } = [];
        public ushort Port { get; set; }
        public Guid ServerGuid { get; set; }
        public bool NatPunch { get; set; }
        public bool UseFikaNatPunchServer { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
