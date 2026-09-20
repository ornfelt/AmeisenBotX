# Upstream sync ledger

Upstream: <https://github.com/Jnnshschl/AmeisenBotX> branch `master`, cloned at
`$USERPROFILE/Downloads/AmeisenBotX`, wired into this repo as the `upstream-local` remote.
Fork point: `7f2e3bc8` - use bezier-curve as default path smoothing
High-water mark: `7fb9071e` (every commit up to and including this one is decided)
Upstream HEAD when last checked: `e23d5844` `ui overhaul and some new auto mode wip` (checked 2026-09-20)
Remaining after the high-water mark: `5`
Local customizations: guarded by `USE_CUSTOM_CHANGES`, defined in `AmeisenBotX.Core/AmeisenBotX.Core.csproj`

Statuses: `applied` (cherry-picked clean), `adapted` (landed, conflicts resolved by hand),
`partial` (part landed, part dropped - the notes say which), `skipped` (deliberately not landed),
`asked` (waiting on an answer from the user, cherry-pick left in progress or aborted).

| # | Commit | Subject | Status | Conflicts | Notes |
| --- | --- | --- | --- | --- | --- |
| 1 | `7fb9071e` | add some ai stuff and icon reading | adapted | `DefaultGrindEngine.cs` | Upstream rewrote the three `ChangeTarget` calls in `ThreatsNearby()` from `FirstOrDefault().Guid` to a local `IWowUnit firstEnemy = ....First();`. Took upstream's text; the guarded `AmeisenLogger` lines above each call are pure additions and stayed put - no `#else` needed, no customization reshaped. `AmeisenBotX.Core.csproj` auto-merged: upstream's `net10.0-windows7.0` bump, System.CodeDom/System.Drawing.Common 10.0.1 and the new `AmeisenBotX.MPQ` project reference landed alongside the `UseCustomChanges` property groups, which survived intact. Guard count unchanged at 18. Both builds green. |

## Guarded customizations

Setup (phase 0) put every local divergence behind `#if USE_CUSTOM_CHANGES`. With the symbol off,
all three files are byte-identical to upstream `7f2e3bc8` (verified mechanically, not just by a
green build).

| File | Guards | What the customization does |
| --- | --- | --- |
| `Engines/Battleground/Jannis/UniversalBattlegroundEngine.cs` | 1 | `StopMovement()` while the player is a ghost, so BG auto-resurrect can happen |
| `Engines/Battleground/KamelBG/ArathiBasin.cs` | 3 | Same ghost check; null-conditional guards on the two `AllBaseList[CurrentNodeCounter]` lookups |
| `Engines/Grinding/DefaultGrindEngine.cs` | 14 | Unreachable-target blacklist (`FightingTargetCounter` > 1000 blacklists the target), `MoveToNextGrindNode` instead of `BtStatus.Failed`, `Player.Level - 4` floor on target selection, `NeedToRepair`/`NeedToSell` forced off, `AmeisenLogger` debug lines |

Find them with `git grep -n "USE_CUSTOM_CHANGES"`. Total guard count: **18** `#if USE_CUSTOM_CHANGES` sites
in C# sources, plus the one `DefineConstants` line in the csproj.

Two upstream `#else` branches are worth knowing about before the first cherry-pick, because they
are what upstream patches will land on:

- `DefaultGrindEngine.NeedToRepair()` / `NeedToSell()` - the `#else` holds upstream's real bodies,
  which the fork previously made unreachable with a bare `return false;`. That removed the two
  `CS0162 Unreachable code detected` warnings the tree used to emit.
- `ArathiBasin.cs` - the fork's unused `using System.Diagnostics;` (it only served a commented-out
  `Debug.WriteLine`) was dropped, so the using block now matches upstream exactly.

## Open questions

None.
