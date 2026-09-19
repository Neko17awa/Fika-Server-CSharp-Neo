# Fika-Neo · Server

Unofficial C# SPT.Server fork of [project-fika/Fika-Server-CSharp](https://github.com/project-fika/Fika-Server-CSharp). Companion client: [Fika-Plugin-Neo](https://github.com/Neko17awa/Fika-Plugin-Neo).

**This is not official Fika.** It is not affiliated with, sponsored by, or endorsed by [Project Fika](https://github.com/project-fika), [SP-Tarkov](https://sp-tarkov.com/), or Battlestate Games. For the supported stack, use the official server mod and the [Fika Wiki](https://wiki.project-fika.com/).

这是 [Fika-Server-CSharp](https://github.com/project-fika/Fika-Server-CSharp) 的非官方服务端分支（Fika-Neo）。对应客户端为 [Fika-Plugin-Neo](https://github.com/Neko17awa/Fika-Plugin-Neo)。**不是**官方 Fika；需要官方联机请走上游仓库和 Wiki。

[<img src="https://mirrors.creativecommons.org/presskit/buttons/88x31/svg/by-nc-sa.svg" alt="CC BY-NC-SA 4.0" width="120">](https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode.en)

| Branch | Role |
| --- | --- |
| `main` | Tracks [upstream `main`](https://github.com/project-fika/Fika-Server-CSharp) |
| `neo` | **Unofficial modification branch** (default) |

Current upstream snapshot: `v2.4.1` (`a6bbfb54`). Server source on the first `neo` commit matches that snapshot; later `neo` commits are Fika-Neo changes.

Sync:

```powershell
git fetch upstream
git checkout main
git merge --ff-only upstream/main
git checkout neo
git merge main
```

GitHub native fork: `gh repo sync Neko17awa/Fika-Server-CSharp-Neo` also fast-forwards `main` from the parent.

## License / 许可

Adapted from Project Fika under **[CC BY-NC-SA 4.0](https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode.en)** (ShareAlike: same license elements). Non-commercial use only. Keep attribution, mark modifications, and do not add extra restrictions. Legal text: [`Licenses/LICENSE-Fika.md`](Licenses/LICENSE-Fika.md). Attribution: [`NOTICE.md`](NOTICE.md).

本仓库是上游的改编作品，仅限非商业使用，必须署名、标明修改、以 CC BY-NC-SA 4.0 再分发。

---

# Upstream: Fika — C# SPT.Server mod

Server-side changes to make multiplayer work. The sections below are the original Project Fika build notes, kept so the unofficial fork stays usable.

## Requirements

- [Visual Studio Code](https://code.visualstudio.com/)
- [.NET SDK 9.0.x](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

## Credits (from upstream)

**Project** | **License**
----------- | -----------------------------------------------------------------------
SPT.Server | [NCSA](https://github.com/sp-tarkov/server-csharp/blob/main/LICENSE)
LiteNetLib | [MIT](https://github.com/RevenantX/LiteNetLib/blob/master/LICENSE.txt) (for NatPunch implementation)

## Disclaimer / 声明

Escape From Tarkov is a trademark of Battlestate Games. Fika-Neo Server is an unofficial, non-commercial adaptation for private modification and study, provided as-is under CC BY-NC-SA 4.0 Section 5.

Escape From Tarkov 为 Battlestate Games 的商标。本仓库仅用于非商业的私人修改与研究，按现状提供，不作任何担保。
