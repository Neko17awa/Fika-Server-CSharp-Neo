using System.Collections.Concurrent;
using FikaServer.Models.Fika.Routes.Hideout;
using SPTarkov.DI.Annotations;

namespace FikaServer.Services;

/// <summary>
/// 本服藏身处 Host 地址。按主人 accountId 登记，并用 Aid 做别名，不进入战局大厅。
/// </summary>
[Injectable(InjectionType.Singleton)]
public class HideoutHostService
{
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(45);
    private readonly ConcurrentDictionary<string, HideoutHostEntry> _hosts = new(StringComparer.Ordinal);

    public void SetHost(FikaHideoutHostRequest request, IEnumerable<string>? aliases = null)
    {
        if (string.IsNullOrWhiteSpace(request.AccountId))
        {
            return;
        }

        Guid.TryParse(request.ServerGuid, out var guid);
        var keys = new HashSet<string>(StringComparer.Ordinal) { request.AccountId };
        if (aliases != null)
        {
            foreach (var alias in aliases)
            {
                if (!string.IsNullOrWhiteSpace(alias))
                {
                    keys.Add(alias);
                }
            }
        }

        if (_hosts.TryGetValue(request.AccountId, out var existing))
        {
            existing.Ips = request.Ips ?? [];
            existing.Port = request.Port;
            existing.ServerGuid = guid;
            existing.NatPunch = request.NatPunch;
            existing.UseFikaNatPunchServer = request.UseFikaNatPunchServer;
            existing.UpdatedAt = DateTime.UtcNow;
            foreach (var key in keys)
            {
                if (!existing.Keys.Contains(key))
                {
                    existing.Keys = [.. existing.Keys, key];
                }

                _hosts[key] = existing;
            }

            return;
        }

        var entry = new HideoutHostEntry
        {
            Keys = [.. keys],
            Ips = request.Ips ?? [],
            Port = request.Port,
            ServerGuid = guid,
            NatPunch = request.NatPunch,
            UseFikaNatPunchServer = request.UseFikaNatPunchServer,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var key in entry.Keys)
        {
            _hosts[key] = entry;
        }
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
            RemoveEntry(entry);
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

        if (_hosts.TryGetValue(accountId, out var entry))
        {
            RemoveEntry(entry);
        }
    }

    private void RemoveEntry(HideoutHostEntry entry)
    {
        foreach (var key in entry.Keys)
        {
            _hosts.TryRemove(key, out _);
        }
    }

    private sealed class HideoutHostEntry
    {
        public string[] Keys { get; set; } = [];
        public string[] Ips { get; set; } = [];
        public ushort Port { get; set; }
        public Guid ServerGuid { get; set; }
        public bool NatPunch { get; set; }
        public bool UseFikaNatPunchServer { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
