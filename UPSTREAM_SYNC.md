# Upstream sync ledger

Upstream: <https://github.com/Jnnshschl/AmeisenBotX> branch `master`, cloned at
`$USERPROFILE/Downloads/AmeisenBotX`, wired into this repo as the `upstream-local` remote.
Fork point: `7f2e3bc8` - use bezier-curve as default path smoothing
High-water mark: `e23d5844` (every commit up to and including this one is decided - **caught up with upstream HEAD**)
Upstream HEAD when last checked: `e23d5844` `ui overhaul and some new auto mode wip` (checked 2026-09-20)
Remaining after the high-water mark: `0`
Local customizations: guarded by `USE_CUSTOM_CHANGES`, defined in `AmeisenBotX.Core/AmeisenBotX.Core.csproj`

Build prerequisite (since `1e0ef441`): the NativeAOT `AmeisenBotX.Bridge` implant is published by a
pre-build target and its link step calls `vswhere.exe`, which is not on PATH here. Prepend it first,
or the build fails with `MSB3073 ... 'vswhere.exe' is not recognized`:

```powershell
$env:PATH = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer;" + $env:PATH
```

Statuses: `applied` (cherry-picked clean), `adapted` (landed, conflicts resolved by hand),
`partial` (part landed, part dropped - the notes say which), `skipped` (deliberately not landed),
`asked` (waiting on an answer from the user, cherry-pick left in progress or aborted).

Every commit that touched a guarded file also carries a mirror verdict in `Notes`, one of
`mirror pending` (asked, unanswered - blocks the next commit), `mirrored`, `mirror declined: <reason>`
or `mirror n/a: <reason>`. It answers: did upstream fix a bug that the `#if USE_CUSTOM_CHANGES`
branch has too? A cherry-pick only ever patches the `#else` half, so without this the fix lands in
the configuration nobody runs. A commit with a pending mirror keeps its real status - the commit
landed; only the mirror waits.

| # | Commit | Subject | Status | Conflicts | Notes |
| --- | --- | --- | --- | --- | --- |
| 1 | `7fb9071e` | add some ai stuff and icon reading | adapted | `DefaultGrindEngine.cs` | Upstream rewrote the three `ChangeTarget` calls in `ThreatsNearby()` from `FirstOrDefault().Guid` to a local `IWowUnit firstEnemy = ....First();`. Took upstream's text; the guarded `AmeisenLogger` lines above each call are pure additions and stayed put - no `#else` needed, no customization reshaped. `AmeisenBotX.Core.csproj` auto-merged: upstream's `net10.0-windows7.0` bump, System.CodeDom/System.Drawing.Common 10.0.1 and the new `AmeisenBotX.MPQ` project reference landed alongside the `UseCustomChanges` property groups, which survived intact. Guard count unchanged at 18. Both builds green. **mirror n/a:** the guards this commit landed next to are pure additions (three `AmeisenLogger` lines and a `.Where` chained into upstream's own `enemiesAround` query) with no `#else` branch, so upstream's `FirstOrDefault().Guid` -> `First()` change sits in code both configurations execute. Nothing to mirror. (Verdict backfilled; the rule postdates the commit.) |
| 2 | `9a1c0369` | cleanup, fixes here and there | applied | none | Clean cherry-pick. The single hunk in a guarded file is a brace-style reformat in the repair-npc routine of `DefaultGrindEngine` (`if (repairNpc == default) return BtStatus.Failed;` split across braces), nowhere near a customization - git auto-merged it. All 18 guards untouched, csproj toggle intact. Upstream deletes the Bia10, Shino and einTyp combat class families plus `CombatStrafer.cs`; none are referenced by a customization. **Inherited breakage:** upstream's `Vector3` rewrite drops `Divide`, `Limit`, `ToArray`, `RotateRadians`, `Subtract` and the comparison operators without updating `AmeisenBotX.Test/Vector3Tests.cs`, so the solution build fails with 14 errors in the test project - identically with the symbol on and off, and identically in upstream's own tree. Upstream fixes `Vector3Tests.cs` in the next commit, `1e0ef441`. `AmeisenBotX.csproj` (the bot and every library it pulls in) builds clean in both configurations, 64 warnings, 0 errors. **mirror n/a:** brace-style reformat, not a fix. (Verdict backfilled; the rule postdates the commit.) |
| 3 | `1e0ef441` | big refactoring, add implant logic to inejct into wow | applied | none | Clean cherry-pick despite 609 files, +23480/-5048. All four guarded files were touched, but only cosmetically: upstream stripped the UTF-8 BOM from line 1 and added the missing trailing newline in each of the three guarded `.cs` files, and renamed two project references in `AmeisenBotX.Core.csproj` (`AmeisenBotX.Wow335a` -> `AmeisenBotX.WowWotlk`, `AmeisenBotX.Wow548` -> `AmeisenBotX.WowMop`). Nothing landed near a guard; git auto-merged all of it and the `UseCustomChanges` property groups survived untouched. Guard count unchanged at 18. Verified mechanically: `git diff upstream-local/master~3 HEAD` over the three guarded sources is **72 + 7 + 18 insertions and zero deletions**, i.e. the fork now holds upstream's `1e0ef441` text verbatim plus the `#if` blocks. Upstream's new root `Directory.Build.props` was taken as written (output redirected to `build\$(Configuration)\`, global `AllowUnsafeBlocks`); `UseCustomChanges` deliberately stays per-csproj. **Resolves the inherited breakage from commit 2:** upstream updates `Vector3Tests.cs` for the rewritten `Vector3`, so the full solution builds again. **New local prerequisite:** the commit adds `AmeisenBotX.Bridge`, a NativeAOT `win-x86` shared library that `AmeisenBotX.csproj` publishes from a `BuildImplant` pre-build target. Its ILCompiler link step shells out to `vswhere.exe`, which is installed at `%ProgramFiles(x86)%\Microsoft Visual Studio\Installerswhere.exe` but is not on this machine's PATH, so a bare `dotnet build` fails with `MSB3073 ... 'vswhere.exe' is not recognized`. Prepending that directory to PATH makes both builds pass. Not a code defect and not caused by the port - record it so the next run does not re-diagnose it. **mirror n/a:** BOM removal, trailing newline and project renames - cosmetic, nothing near a guard. (Verdict backfilled; the rule postdates the commit.) |
| 4 | `725dd113` | add more cool stuff | applied | none | Clean cherry-pick, 38 files, +2487/-723. **No guarded file was touched at all** - not the three sources, not `AmeisenBotX.Core.csproj` - so there was nothing to merge and nothing to reshape. Guard count unchanged at 18. The one thing worth checking beforehand was the `#else` branches: upstream deletes `Managers/Character/Inventory/CharacterInventory.cs` and replaces it with `InventoryManager.cs`, retyping `ICharacterManager.Inventory` and `DefaultCharacterManager.Inventory` from `CharacterInventory` to `InventoryManager`. `NeedToSell()`'s `#else` reads `Bot.Character.Inventory.FreeBagSlots`, and `NeedToRepair()`'s reads `Bot.Character.Equipment.Items`. `InventoryManager` keeps `FreeBagSlots` (and `Items`), and `CharacterEquipment` is untouched, so both `#else` bodies still compile against the new type - confirmed by the off-build, which is the only thing that exercises them. Upstream also deletes `Logic/Routines/InventoryOrganizer.cs` (folded into `InventoryManager`); no customization referenced it. Verified mechanically: the fork's **entire** divergence from upstream `725dd113` is six files and **719 insertions with zero deletions** - `SKILL.md`, `UPSTREAM_SYNC.md`, the ten csproj toggle lines, and 7 + 18 + 72 lines of guards in the three sources. **mirror n/a:** no guarded file touched. (Verdict backfilled; the rule postdates the commit.) |
| 5 | `7d591737` | new launch ui, WIP | applied | none | Clean cherry-pick, 6 files, +1066/-62, all of them inside the `AmeisenBotX` WPF project (`LoadConfigWindow.xaml` / `.xaml.cs`, `MainWindow.xaml.cs`, new `Models/BotProfile.cs`, new `Views/CharacterCard.xaml` / `.xaml.cs`). **No guarded file was touched, and the commit deletes nothing** - `git show --diff-filter=D` is empty - so there was no API the `#else` branches could have lost. Guard count unchanged at 18; csproj toggle untouched. The customizations all live in `AmeisenBotX.Core` engines and this commit never reaches that assembly. Verified mechanically: the fork's entire divergence from upstream `7d591737` is the same six files and **720 insertions with zero deletions**. **mirror n/a:** no guarded file touched. (Verdict backfilled; the rule postdates the commit.) |
| 6 | `e23d5844` | ui overhaul and some new auto mode wip | applied | none | Clean cherry-pick, 164 files, +8880/-6390. **No guarded file was touched.** Guard count unchanged at 18; csproj toggle untouched. The size and the -6390 made the `#else` branches worth checking first: upstream reworks `InventoryManager.cs`, `AmeisenBotConfig.cs`, `IWowInterface.cs` and rewrites `SimpleDpsTargetSelectionLogic.cs` (-343). Confirmed before picking that every symbol the `#else` bodies read still exists at `e23d5844` - `AmeisenBotConfig.ItemRepairThreshold` (line 233), `AmeisenBotConfig.BagSlotsToGoSell` (line 94), `InventoryManager.FreeBagSlots` and `.Items`; the off-build then exercised them for real. The 13 deleted files are all WPF XAML, deleted-and-recreated under the new `AmeisenBotX/Windows/` folder, with their code-behind detected as renames - nothing in `AmeisenBotX.Core`. Big upstream additions: a new `Engines/Autopilot` subsystem (`AutopilotManager`, quest parser/pulse engine, quest batching, discovery and interaction services, map coordinates), a `QuestPriorityModule` / `QuestTargetPrioritizer` in target selection, `CombatClassMetadata`, shared warrior base classes for both the Jannis and Kamel families, and a new `BotMode`. Verified mechanically: the fork's entire divergence from upstream HEAD is six files and **721 insertions with zero deletions**. Symbol flip re-checked at this milestone: `USE_CUSTOM_CHANGES` appears twice in the ON build of `AmeisenBotX.Core` and not at all in the OFF build. **mirror n/a:** no guarded file touched. (Verdict backfilled; the rule postdates the commit.) |

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

## Status

The fork is **caught up with everything in the Downloads clone** as of 2026-09-20: all six
commits after the fork point are landed, and `git diff upstream-local/master HEAD` is six files of
pure insertions. To pick up newer upstream work, run `git -C "$USERPROFILE/Downloads/AmeisenBotX" fetch`
yourself in that clone first, then rerun this skill.
