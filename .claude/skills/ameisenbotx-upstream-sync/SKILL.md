---
name: ameisenbotx-upstream-sync
description: Land upstream AmeisenBotX commits in the ornfelt fork, one commit at a time. First run guards every local customization behind the USE_CUSTOM_CHANGES preprocessor directive (toggleable per csproj, default on) and proves the solution builds with it both on and off. Later runs cherry-pick the next unprocessed upstream commit from the read-only clone in ~/Downloads/AmeisenBotX, resolve conflicts against the guarded blocks, rebuild both configurations, and record the result in UPSTREAM_SYNC.md so the next run continues where this one stopped.
disable-model-invocation: true
argument-hint: "[empty means the next unprocessed commit, or: setup, status, <commit-id>, skip <commit-id> <reason>, replay <commit-id>, abort]"
---

# Sync upstream AmeisenBotX commits into the fork

`Code2/C#/AmeisenBotX` is a fork of `Jnnshschl/AmeisenBotX`. It carries a handful of local gameplay
customizations on top of the shared history. Upstream keeps moving; this skill walks upstream's
history forward from the fork point, **one commit at a time**, and lands each commit in the fork
while keeping the local customizations working.

This is the **AmeisenBotX tree**. It has nothing to do with `wow-client-upstream-sync`, with the
`wc_clean_new` ports, with `libwow`, or with any of the `cpp-csharp-*` server ports. Different
source, different target, different rules. Do not apply any `.claude/rules/wc-clean-new-*.md` file
here - none of them apply to this tree.

## Source and target

Upstream reference clone (read-only), branch `master`, remote
`https://github.com/Jnnshschl/AmeisenBotX`:

`$USERPROFILE/Downloads/AmeisenBotX` (PowerShell: `$env:USERPROFILE\Downloads\AmeisenBotX`)

Target - the fork this skill writes to, remote `https://github.com/ornfelt/AmeisenBotX`:

`$code_root_dir/Code2/C#/AmeisenBotX` (PowerShell: `$env:code_root_dir\Code2\C#\AmeisenBotX`)

`code_root_dir` is an environment variable.

Unlike the `wow_client` sync, **these two repositories share history** - both descend from the same
GitHub repo. That makes `git cherry-pick` available, and cherry-pick is the transfer mechanism this
skill uses. Upstream commits here are very large (one of them touches 609 files and adds 23,000
lines); hand-transcribing them is not realistic, so git does the bulk of the work and the run only
hand-resolves what actually conflicts.

The fork point is `7f2e3bc8` ("use bezier-curve as default path smoothing"), the merge base of the
two trees. Everything at or before it is already in the fork by definition and never comes up again.

## Scope guard

- Write only inside `$code_root_dir/Code2/C#/AmeisenBotX`.
- **Never write anything under `~/Downloads/AmeisenBotX`** - no commits, no checkouts, no
  `git stash`, no branch switching, no `git clean`. It is read-only reference material and must
  stay on `master` with a clean worktree so the next run finds the same history. Reading is
  `git log`, `git show`, `git diff`, `git cat-file`, `git ls-tree`; that is the whole allowed set.
- Do not `git fetch` or `git pull` from GitHub unless the user asks. A run works with the history
  that is already in the Downloads clone. Fetching *from the local clone into the fork*
  (`git fetch upstream-local`) is not that - it is required plumbing and is always fine.
- Work on the fork's `master` branch. The fork's `commented` branch (12 commits of generated XML
  documentation, diverged at `6998fede`) is **out of scope** - never check it out, never merge it,
  never cherry-pick onto it.
- Do not push. Cherry-pick creates commits in the fork's local `master`; that is expected and is as
  far as a run goes.
- Do not run the bot: it needs a WoW 3.3.5a/5.4.8 client, game data and a server. Building is the
  whole verification story here.

## The local customizations

As of the fork point the local diff is three files in `AmeisenBotX.Core`, +46/-6:

| File | What the fork changed |
| --- | --- |
| `Engines/Battleground/Jannis/UniversalBattlegroundEngine.cs` | Stop movement while the player is a ghost, so BG auto-resurrect can happen |
| `Engines/Battleground/KamelBG/ArathiBasin.cs` | Same ghost check, plus null-conditional guards on `AllBaseList` lookups |
| `Engines/Grinding/DefaultGrindEngine.cs` | Unreachable-target blacklist + `FightingTargetCounter`, fall through to `MoveToNextGrindNode` instead of failing, level floor of `Player.Level - 4` on target selection, `NeedToRepair`/`NeedToSell` forced off, extra `AmeisenLogger` debug lines |

All three files still exist at upstream HEAD, and all three are touched by upstream `1e0ef441`
("big refactoring"); `DefaultGrindEngine.cs` is additionally touched by `7fb9071e` and `9a1c0369`.
**Conflicts on those commits are certain, not hypothetical.** That is the normal case, not a
failure.

Once setup has run, this table is no longer the source of truth - the `#if USE_CUSTOM_CHANGES`
blocks in the tree are. Find them with:

```bash
git -C "$FORK" grep -n "USE_CUSTOM_CHANGES"
```

## The USE_CUSTOM_CHANGES directive

Every local customization lives behind `#if USE_CUSTOM_CHANGES`. The symbol is defined per project
file, defaults to **on**, and is flipped from the command line for the verification build.

Each `.csproj` that contains guarded code gets, right after the first `PropertyGroup`:

```xml
  <PropertyGroup>
    <!-- Local customizations on top of upstream AmeisenBotX. Set to false to build
         the tree as upstream wrote it. Override per build with -p:UseCustomChanges=false -->
    <UseCustomChanges Condition="'$(UseCustomChanges)'==''">true</UseCustomChanges>
  </PropertyGroup>

  <PropertyGroup Condition="'$(UseCustomChanges)'=='true'">
    <DefineConstants>$(DefineConstants);USE_CUSTOM_CHANGES</DefineConstants>
  </PropertyGroup>
```

The `Condition="'$(UseCustomChanges)'==''"` on the default is what lets `-p:UseCustomChanges=false`
win; without it the csproj value overrides the command line.

Today only `AmeisenBotX.Core/AmeisenBotX.Core.csproj` needs it. When a future customization lands in
another project, add the same two groups to that project's csproj in the same run - a guarded block
in a project that does not define the symbol silently compiles as the upstream branch, which is a
bug that no build catches.

Follow the `preprocessor-directives` skill's shape: C# gets real `#if` / `#else` / `#endif`, nothing
else. Do not invent a `BuildFlags` class here.

### Guarding styles

Pure addition - no `#else` branch:

```csharp
#if USE_CUSTOM_CHANGES
            if (Bot.Player.IsGhost)
            {
                Bot.Movement.StopMovement();
            }
#endif
```

Replacement of upstream code - upstream's original goes in the `#else`, verbatim, never deleted:

```csharp
#if USE_CUSTOM_CHANGES
                        new Leaf(MoveToNextGrindNode)
#else
                        new Leaf(() => BtStatus.Failed)
#endif
```

Early-return overrides keep upstream's body reachable in the `#else` branch, so the
off-configuration does not warn:

```csharp
        private bool NeedToRepair()
        {
#if USE_CUSTOM_CHANGES
            return false;
#else
            return Bot.Character.Equipment.Items.Any(e => e.Value.MaxDurability > 0
                   && (e.Value.Durability / (double)e.Value.MaxDurability * 100.0) <= Config.ItemRepairThreshold);
#endif
        }
```

Rules:

- The `#else` branch must always be upstream's exact code. It is what future cherry-picks patch, and
  it is the only record of what upstream intended. A customization that just deletes upstream code
  still gets an `#else` holding that code.
- A `using` directive needed only by guarded code gets guarded too, or the off-build warns about an
  unused using.
- Do not guard whitespace, comment or formatting differences. Only real behavioral divergence.
- Keep the guards as tight as the change allows - a statement, not a whole method, unless the whole
  method is what diverged.
- Commented-out upstream lines the fork left behind (`//new Leaf(() => BtStatus.Failed)`) are
  replaced by a proper `#else` branch; do not carry both.

## Modes

Decide from the argument:

1. **Continue (no argument)** - the default and the everyday mode. Work out the next unprocessed
   upstream commit from the ledger, then land it. This is the form meant to be rerun over and over.
   If `UPSTREAM_SYNC.md` does not exist, this runs Setup instead and then stops - setup is a full
   run on its own.
2. **Setup** - `setup`, or the ledger does not exist. Phase 0 below: guard the customizations,
   wire up the csproj toggle, prove both builds, create the ledger. Does **not** continue into a
   commit; the guarded tree is worth reviewing on its own.
3. **Specific commit** - a bare upstream commit id (`9a1c0369`), optionally spelled
   `apply <commit-id>`. Land that commit even if it is out of order.
4. **Status** - `status`. Report where the sync stands: fork point, high-water mark, how many
   commits remain, what the next one is, whether a cherry-pick is in progress, and the open
   questions. **Write nothing**, ledger included.
5. **Skip** - `skip <commit-id> <reason>`. Record the commit as skipped in the ledger without
   landing it, and move the high-water mark past it if it is the next one. Use this when the user
   has answered "no" to a question a previous run asked.
6. **Replay** - `replay <commit-id>`. Redo a commit whose earlier attempt was wrong or incomplete.
   Update the existing ledger row rather than adding a second one. Do not `git reset` the fork to
   undo the earlier attempt unless the user asks - fix forward.
7. **Abort** - `abort`. An interrupted cherry-pick is in the working tree and the user wants out:
   `git cherry-pick --abort`, leave the ledger row as `asked`, report.
8. **Mirror** - `mirror <commit-id>`. The user answered "yes" to a pending mirror question from
   step 6: port that commit's upstream fix into the `#if USE_CUSTOM_CHANGES` branch as well, in a
   commit of its own, rebuild both configurations, and clear the question. Lands no new upstream
   commit.
9. **No-mirror** - `no-mirror <commit-id> <reason>`. The user answered "no": leave the `#if` branch
   as it is, record `mirror declined: <reason>` in that row's `Notes`, clear the question. Writes
   only the ledger.

A bare `mirror` or `no-mirror` with no commit id means the newest row whose `Notes` say
`mirror pending`. If more than one row is pending and no id was given, list them and ask which.

**Before anything else**, check two things:

```bash
FORK="$code_root_dir/Code2/C#/AmeisenBotX"
cat "$FORK/UPSTREAM_SYNC.md" 2>/dev/null || echo "NO LEDGER - setup mode"
git -C "$FORK" status --short
ls "$FORK/.git/CHERRY_PICK_HEAD" 2>/dev/null && echo "CHERRY-PICK IN PROGRESS"
```

A cherry-pick already in progress means a previous run stopped on a conflict question. Do not start
a new commit. Read the ledger's open question, and if the user's answer is in this invocation,
resolve and continue that pick; otherwise re-state the question and stop.

A ledger row whose `Notes` say `mirror pending` is the other kind of unanswered question. That one
leaves no cherry-pick in progress - the commit landed fine - so the tree looks clean and it is easy
to walk straight past. In Continue mode, settle every pending mirror before starting a new commit:
if the user's answer is in this invocation, act on it; otherwise re-state the question and stop.

## Phase 0 - setup

Runs once, before any upstream commit is touched.

### 1. Confirm the fork point and the local diff

```bash
FORK="$code_root_dir/Code2/C#/AmeisenBotX"
UP="$USERPROFILE/Downloads/AmeisenBotX"

git -C "$FORK" merge-base master 7f2e3bc8          # must print 7f2e3bc8...
git -C "$FORK" diff --stat 7f2e3bc8 master
git -C "$FORK" diff 7f2e3bc8 master
```

Derive the fork point rather than trusting `7f2e3bc8` blindly - if the user has merged upstream
again since this skill was written, the real merge base is
`git -C "$FORK" merge-base master upstream-local/master` once the remote exists. The literal above
is the value at the time of writing and a sanity check, not an assumption.

That diff is the complete list of what has to end up behind a guard. Read all of it before editing.

### 2. Wire the remote

```bash
git -C "$FORK" remote add upstream-local "$USERPROFILE/Downloads/AmeisenBotX" 2>/dev/null || true
git -C "$FORK" fetch upstream-local
```

`upstream-local` is a path remote into the read-only clone. It is only ever fetched from.

### 3. Guard every customization

Apply the guarding styles above, file by file. Follow `minimal-code-changes`: add the directives and
the `#else` bodies, change nothing else. Do not reformat, do not rename, do not fix unrelated
warnings noticed on the way.

Recovering upstream's original text for an `#else` branch:

```bash
git -C "$FORK" show 7f2e3bc8:AmeisenBotX.Core/Engines/Grinding/DefaultGrindEngine.cs
```

### 4. Add the csproj toggle

`AmeisenBotX.Core/AmeisenBotX.Core.csproj`, plus any other project that ended up with a guard.

### 5. Prove both configurations

Both must compile. This is the point of the whole phase.

```powershell
cd "$env:code_root_dir\Code2\C#\AmeisenBotX"
dotnet build AmeisenBotX.sln -c Debug --nologo -v m
dotnet build AmeisenBotX.sln -c Debug --nologo -v m -p:UseCustomChanges=false
```

Then confirm the symbol actually flipped, rather than trusting the exit code:

```powershell
dotnet build AmeisenBotX.sln -c Debug -v n -p:UseCustomChanges=false 2>&1 | Select-String "USE_CUSTOM_CHANGES"
```

A guarded block that compiles in both configurations *because the symbol was never defined* is the
classic silent failure. Check the output of the ON build actually contains the constant and the OFF
build does not.

### 6. Seed the ledger and stop

Create `UPSTREAM_SYNC.md` (shape below) with the fork point as the high-water mark and no commit
rows. Report, and do not continue into the first commit.

## The ledger

`$FORK/UPSTREAM_SYNC.md` is the progress file. It exists so a run never has to re-derive the history
and so the user never has to remember which commit came last. Every run reads it first and updates
it last.

Setup creates it with this shape:

```markdown
# Upstream sync ledger

Upstream: <https://github.com/Jnnshschl/AmeisenBotX> branch `master`, cloned at
`$USERPROFILE/Downloads/AmeisenBotX`, wired into this repo as the `upstream-local` remote.
Fork point: `7f2e3bc8` - use bezier-curve as default path smoothing
High-water mark: `7f2e3bc8` (every commit up to and including this one is decided)
Upstream HEAD when last checked: `<sha>` `<subject>` (<date of the run>)
Remaining after the high-water mark: `<n>`
Local customizations: guarded by `USE_CUSTOM_CHANGES`, defined in `<list of csproj files>`

Statuses: `applied` (cherry-picked clean), `adapted` (landed, conflicts resolved by hand),
`partial` (part landed, part dropped - the notes say which), `skipped` (deliberately not landed),
`asked` (waiting on an answer from the user, cherry-pick left in progress or aborted).

A commit that landed but raised a step 6 mirror question keeps its real status - it is `applied` or
`adapted`, not `asked`, because the commit itself is done. The pending question lives in the `Notes`
column as `mirror pending` and under "Open questions", and it still blocks the next commit.

| # | Commit | Subject | Status | Conflicts | Notes |
| --- | --- | --- | --- | --- | --- |

## Open questions

<one bullet per `asked` commit, per `mirror pending` commit, and per skipped-but-arguable commit,
with the question that was put to the user and what it is waiting on. A `mirror pending` bullet
names the commit, the defect, the file and line in the `#if` branch that shares it, and the two
answers: `mirror <commit-id>` or `no-mirror <commit-id> <reason>`>
```

Rules for the ledger:

- One row per upstream commit that has been looked at, in upstream order, newest last.
- The **high-water mark** is the newest commit such that every commit from the fork point up to it
  has a row. A commit landed out of order by mode 3 gets a row but does **not** move the mark; the
  mark moves later, when the gap is closed.
- `Remaining` is recomputed each run, not carried over:
  `git -C "$FORK" rev-list --count <high-water>..upstream-local/master`.
- The `Conflicts` column lists the files git actually conflicted on. It is the column that matters
  next time - it is how a later run knows which files the customizations keep colliding with.
- The `Notes` column carries the real information: how each conflict was resolved, whether a guard
  was moved or rewritten, whether a customization was dropped as obsolete, whether a new csproj
  needed the toggle, and whether either build broke.
- `Notes` also carries the step 6 verdict for every commit that touched a guarded file, in exactly
  one of four words: `mirror pending` (asked, unanswered - blocks the next commit), `mirrored` (the
  fix went into the `#if` branch too, in its own commit), `mirror declined: <reason>` (the user said
  no), or `mirror n/a: <reason>` (the change was a preference or could not apply, so nothing was
  asked). A guarded file touched with no mirror verdict recorded means step 6 was skipped - go back
  and do it rather than assuming it was fine.
- The ledger is a hint that makes orientation cheap, not the source of truth. If it disagrees with
  the fork's working tree, the tree wins and the ledger gets corrected in the same run.

## Landing a commit

### 1. Orient

```bash
FORK="$code_root_dir/Code2/C#/AmeisenBotX"
UP="$USERPROFILE/Downloads/AmeisenBotX"

cat "$FORK/UPSTREAM_SYNC.md"                       # the cheap answer first
git -C "$FORK" fetch upstream-local

# HIGH is the ledger's high-water mark
git -C "$FORK" log --oneline --reverse "$HIGH..upstream-local/master"
git -C "$FORK" rev-list --count "$HIGH..upstream-local/master"
```

The next commit is the first line of that `--reverse` listing. In mode 3 it is the commit the user
named instead.

At the time of writing, six commits sit after the fork point:

| Commit | Subject | Size |
| --- | --- | --- |
| `7fb9071e` | add some ai stuff and icon reading | 119 files, +5689/-393 |
| `9a1c0369` | cleanup, fixes here and there | 143 files, +1714/-5913 |
| `1e0ef441` | big refactoring, add implant logic to inejct into wow | 609 files, +23480/-5048 |
| `725dd113` | add more cool stuff | 38 files, +2487/-723 |
| `7d591737` | new launch ui, WIP | 6 files, +1066/-62 |
| `e23d5844` | ui overhaul and some new auto mode wip | 164 files, +8880/-6390 |

`1e0ef441` also introduces a root `Directory.Build.props` that redirects output to
`build\$(Configuration)\` and sets `AllowUnsafeBlocks` globally. That is upstream's file - take it
as upstream wrote it; it does not become the home of `UseCustomChanges`, which stays per-csproj.

### 2. Read the whole commit before touching anything

```bash
git -C "$UP" show --stat <commit>
git -C "$UP" show <commit> -- <the guarded files>
```

The second command is the one that predicts the run: it shows whether this commit collides with a
customization at all, and it is also the first look at the step 6 question - read it asking whether
upstream is correcting a defect or changing a preference, because a correction in a file that
carries guards is a mirror candidate even when the pick goes in clean. Cross-check against the files
that carry guards:

```bash
git -C "$FORK" grep -l "USE_CUSTOM_CHANGES"
```

A full `git show` of a 609-file commit is not worth reading end to end. Read the stat, read the
hunks touching guarded files, and read anything that looks like it removes a subsystem the
customizations depend on (`AmeisenLogger`, `Bot.Movement.StopMovement`, `BtStatus`, the grind
engine's behavior tree).

### 3. Cherry-pick

```bash
git -C "$FORK" cherry-pick <commit>
```

Three outcomes:

- **Clean** - git committed it. Skip step 4 and go to step 6 - a clean pick still has to answer the
  mirror question if it touched a guarded file. Status `applied`.
- **Conflicts** - git stopped. Go to step 4. Status will be `adapted`.
- **Empty** - "The previous cherry-pick is now empty". The change is already in the fork. Use
  `git cherry-pick --skip`, status `applied`, note says it was already present.

A merge commit cannot be cherry-picked without `-m 1`. Test for one first - the command prints `1`
for a merge and `0` otherwise:

```bash
git -C "$UP" rev-list --no-walk --count --merges <commit>
```

None of the six commits listed above are merges, so this only matters for history fetched later.

### 4. Resolve conflicts

```bash
git -C "$FORK" status --short | grep "^UU\|^AA\|^DU\|^UD"
git -C "$FORK" diff --diff-filter=U
```

For each conflicted file, work out which of these it is:

- **Conflict away from the guards** - upstream changed code the fork never touched, and git only
  failed because of nearby context. Take upstream's side. Resolve and move on.
- **Upstream edits the `#else` branch** - upstream improved the code the fork replaced. Take
  upstream's new text into the `#else` branch and leave the `#if` branch alone for now. The
  customization still wins when the symbol is on, and the off-build now tracks upstream. This is the
  common, clean case - but it is also exactly where a bug fix can end up applying only to the
  configuration nobody runs, so step 6 asks whether it belongs in the `#if` branch too.
- **Upstream edits the `#if` branch's subject** - upstream changed the same behavior the fork
  customized. Update **both** branches: upstream's text in `#else`, and the customization
  re-expressed on top of upstream's new shape in `#if`. Say in the notes what the customization now
  looks like.
- **Upstream deletes or renames the code the guard wraps** - the customization has nowhere to live.
  This is an **ask**, always.
- **Upstream makes the customization obsolete** - upstream fixed the same bug, possibly better.
  This is an **ask**, always.

Resolve the first three yourself when the resolution is unambiguous. The conflict markers never
survive - a file with `<<<<<<<` left in it is a failed run.

**Ask the user, and write nothing further, when:**

- a guard's subject was deleted, renamed or restructured beyond recognition upstream;
- upstream implemented the same thing the customization does, so keeping both is redundant;
- the resolution is a judgement call about behavior rather than a mechanical merge - which of two
  competing target-selection policies should win, for instance;
- resolving would mean rewriting the customization rather than moving it.

Put the question in the summary with: what upstream did, what the customization does, the two or
three concrete options, and a recommendation with its reasoning. Record the commit as `asked`, put
the question under "Open questions" in the ledger, and stop. Leave the cherry-pick in progress so
the next run can finish it - say so explicitly in the summary, and say that `abort` backs it out.

Do not guess at a behavioral resolution to keep a run moving. An `asked` run is a complete, correct
run.

### 5. Finish the pick

```bash
git -C "$FORK" add <resolved files>
git -C "$FORK" cherry-pick --continue --no-edit
```

Then append the fork's own trailer to the message, so the history says what was adapted:

```bash
git -C "$FORK" commit --amend --no-edit
```

Keep upstream's subject line verbatim - that is what makes the ledger and `git log` line up. Add a
body paragraph only when conflicts were resolved, saying which files and how. **Never use double
quotes (`"`) in the message** - where a quote is needed, use a single quote (`'`). **Never add a
`Co-Authored-By` trailer** - these commits carry upstream's authorship, not Claude's.

```text
add some ai stuff and icon reading

Cherry-picked from upstream 7fb9071e. Resolved DefaultGrindEngine.cs: upstream's
rewritten SelectTarget went into the #else branch, the blacklist customization was
re-expressed on top of it in the #if branch.
```

### 6. Does the fix also belong in the `#if` branch?

A guard splits the code in two, and a cherry-pick only ever patches upstream's half. So an upstream
**bug fix** that lands in an `#else` branch fixes the configuration nobody runs and leaves the
customization - the code that actually executes - carrying the bug. That is the single most likely
way this sync quietly goes wrong, and no build catches it: both configurations still compile.

Run this check on **every** commit that touched a guarded file, whether or not it conflicted. A
clean cherry-pick is not evidence that the change was irrelevant to the `#if` branch; it usually
just means git found enough context to place the hunk without asking. With commits this size, most
of upstream's 609 files are nowhere near a guard - this narrows it to the ones that are:

```bash
# which guarded files did this commit touch?
comm -12 <(git -C "$FORK" show --name-only --format= <commit> | sort -u) \
         <(git -C "$FORK" grep -l "USE_CUSTOM_CHANGES" | sort -u)
```

For each one, read what upstream changed and then read the `#if` branch beside it, and decide which
of these it is:

- **A fix whose subject exists in the `#if` branch too** - upstream corrected something the fork
  copied, inherited, or rewrote around, and the same defect is sitting in the customization. This is
  an **ask**. Typical shapes: a null or bounds check added before a dereference, an inverted or
  off-by-one condition, a wrong constant or id, a handle or state that was never reset, an exception
  that was swallowed and should not be, a call that has to move above or below another to be
  correct, a `BtStatus` returned from the wrong branch.
- **A fix that cannot apply** - the `#if` branch already handles that case its own way, or the code
  upstream fixed is code the fork deleted outright, or the fix is in a path the customization never
  reaches. Say so in the notes and move on. No ask.
- **Not a fix at all** - a tuning value, a feature, a preference, a refactor, a log line. The fork
  chose its own value or shape on purpose. Leave the `#if` branch alone; that is what the guard is
  for. No ask.

The hard part is telling the first from the third, and the tie-break is intent: ask whether upstream
was **correcting** something or **changing** something. Upstream widening its target-selection level
window is a change - the fork's `Player.Level - 4` floor stands. Upstream adding a null-conditional
to an `AllBaseList` lookup is a correction, and if the fork's rewritten `SelectTarget` dereferences
the same thing unguarded, it needs the same check.

**When it is a fix that applies: land the commit as normal, then ask before mirroring it.** Do not
edit the `#if` branch on your own judgement, and do not hold the commit back waiting for an answer -
the commit is upstream's and is correct on its own terms. Finish steps 5 and 7 as usual, record the
commit with its real status, and add the mirror question on top:

- put it under "Open questions" in the ledger, keyed to the commit;
- append `mirror pending` to that row's `Notes`;
- state it in the summary with: the defect upstream fixed, the exact place in the `#if` branch that
  has the same defect (file and line), what the mirrored fix would look like, and a recommendation.

Then stop. The next run answers it with `mirror <commit-id>` or `no-mirror <commit-id> <reason>`.

A run may mirror in the same pass **only** when the user has already said yes in that invocation -
for instance `apply <commit-id>` together with an explicit instruction to mirror, or a `mirror`
argument naming the commit just landed. When mirroring:

- edit the `#if` branch only; the `#else` branch keeps upstream's text exactly as cherry-picked;
- keep the mirrored fix as close to upstream's wording as the customization's shape allows, so the
  two branches stay comparable next time;
- rebuild both configurations (step 7) - a mirror is a real code change and can break either side;
- commit it **separately** from the cherry-pick, so a bad mirror can be reverted without losing the
  upstream commit. Subject shape: `Mirror <commit> fix into the USE_CUSTOM_CHANGES branch`;
- clear the open question and change `mirror pending` to `mirrored` in the row's `Notes`.

If the answer was no, change `mirror pending` to `mirror declined: <reason>` and clear the question.
Do not raise the same question again on a later run.

### 7. Build both configurations

```powershell
cd "$env:code_root_dir\Code2\C#\AmeisenBotX"
dotnet build AmeisenBotX.sln -c Debug --nologo -v m
dotnet build AmeisenBotX.sln -c Debug --nologo -v m -p:UseCustomChanges=false
```

Both must reach `Build succeeded` with 0 errors. **The off-build is not optional** - it is the only
thing that proves the `#else` branches still compile against upstream's new code, and it is exactly
what a cherry-pick is most likely to have broken.

Baseline at the time of writing, for comparison: the Debug build succeeds with **7 warnings, 0
errors**. Two of those, `CS0162 Unreachable code detected` at `DefaultGrindEngine.cs:370` and `:377`,
are the `NeedToRepair`/`NeedToSell` overrides and disappear once those are properly guarded with an
`#else`. A warning count that jumps is worth a look, but only errors block a run.

Fix compile errors caused by this commit. Do not fix pre-existing breakage the commit did not cause
- report it instead. If the tree was already broken before the run, say so, so it is not mistaken
for a result of the port.

If the off-build fails while the on-build passes, the fault is almost always an `#else` branch that
was not updated with upstream's change. Go back to step 4 rather than deleting the guard.

### 8. Record

Update `UPSTREAM_SYNC.md`: add or update the row, fill the `Conflicts` column, move the high-water
mark if this commit closed the gap, refresh `Upstream HEAD when last checked` and `Remaining`, and
add or clear any open question. Commit the ledger update as part of the cherry-pick commit
(`git commit --amend`) or as a separate follow-up commit - either is fine, but do not leave it
uncommitted.

## One commit at a time

A run lands exactly one upstream commit. These commits are large; batching two of them makes the
diff unreviewable and makes a bad conflict resolution impossible to bisect later. The single
exception is a commit that plainly cannot compile without its immediate follow-up - say so, name
the follow-up, and either take it in the same run or stop and ask, whichever keeps the run
reviewable.

Setup is its own run and lands no commits. So is a `mirror` run: porting a fix into the `#if` branch
is its own commit and its own run, and it never picks up the next upstream commit afterwards.

## Verification

- `dotnet build AmeisenBotX.sln -c Debug` with `UseCustomChanges` at its default (on).
- The same build with `-p:UseCustomChanges=false`.
- `git -C "$FORK" status --short` at the end - it should be clean, or show only files the run
  deliberately left uncommitted. A stray `.orig` or `.rej` file means a resolution was left
  half-done.
- `git -C "$FORK" log --oneline -3` so the summary shows the commit that was created.
- `git -C "$FORK" grep -c "USE_CUSTOM_CHANGES"` - the guard count should only ever change for a
  reason the run can name.
- Every guarded file the commit touched has a step 6 mirror verdict in the ledger row's `Notes`.
  Silence there is not a pass; it means the check did not happen.
- If either build was skipped or failed, say so plainly rather than reporting success.

## Output expectations

Every run ends with, in this order:

1. **What the commit was** - upstream sha, subject, and the classification (`applied` / `adapted` /
   `partial` / `skipped` / `asked`). For a setup run: "Phase 0 - customizations guarded".
2. **What changed in the fork** - file by file. For a cherry-pick, the interesting part is not
   upstream's 609 files but the conflicts: which files conflicted, and how each was resolved. Say
   explicitly if a csproj was touched, if a guard was added, moved or removed, and if nothing was
   written.
3. **Behavior** - in plain words, what this changes for someone running the bot: new features from
   upstream, and whether any local customization behaves differently now. Be honest about
   "probably nothing visible". If a customization was reshaped, say what it does now.
4. **Build result** - the exact commands run and whether each passed, with the error and warning
   counts for both configurations. Never report a build as passing without having run it.
5. **Mirror verdict**, for every guarded file the commit touched - one line each saying whether
   upstream's change was a fix that also applies to the `#if` branch, and which of `mirror pending`
   / `mirrored` / `mirror declined` / `mirror n/a` was recorded. Do not leave this out because the
   pick was clean; a clean pick is when it matters most.
6. **Open question**, if the run is `asked` or a mirror is pending:
   - for `asked` - the question, the options, and a recommendation. State that the cherry-pick is
     still in progress and that `abort` backs it out.
   - for `mirror pending` - the defect upstream fixed, the file and line in the `#if` branch that
     has the same defect, what the mirrored fix would look like, and a recommendation. State that
     the commit itself has landed and that the answer is `mirror <commit-id>` or
     `no-mirror <commit-id> <reason>`.
7. **A `Next:` line** so the following run - and the reader - knows the entry point:

   ```text
   Next: upstream 9a1c0369 "cleanup, fixes here and there" (143 files; touches DefaultGrindEngine.cs, expect a conflict)
   ```

   When a mirror is pending, that is the entry point instead:

   ```text
   Next: answer the mirror question on 7fb9071e (mirror 7fb9071e / no-mirror 7fb9071e <reason>),
         then upstream 9a1c0369 "cleanup, fixes here and there"
   ```

## Rerunning

The bare form is designed to be run again and again, from a cold context each time. A run
re-derives its position from `UPSTREAM_SYNC.md` plus `git log`, so a fresh session, a new day or a
`/clear` in between changes nothing.

Five stopping conditions:

- **Done with the commit** - landed or skipped, both builds green, ledger updated, report and stop.
  Do not start the next one.
- **Blocked or unclear** - ask, record `asked`, leave the cherry-pick in progress, stop.
- **Mirror pending** - the commit landed and both builds are green, but upstream's fix looks like it
  belongs in the `#if` branch too. Record the commit with its real status plus `mirror pending`, ask,
  and stop. The commit stays; only the mirror waits. A later `mirror <commit-id>` or
  `no-mirror <commit-id> <reason>` settles it.
- **Setup finished** - report the guarded tree and stop; the user reviews it before commits start
  landing on top.
- **Nothing left** - the high-water mark is `upstream-local/master`. Say the fork is caught up and
  suggest `git -C "$UP" fetch` (in the Downloads clone, by the user) as the next step rather than
  inventing work. An empty run that says "nothing left" is a correct run.

## The 'N commits behind' banner

Expect GitHub to report the fork as **N commits behind** `Jnnshschl/AmeisenBotX`, where N is the
number of upstream commits landed so far. This is normal and is **not** a sign that anything is
missing.

Cherry-pick copies a commit's content but creates a new commit: different parent, different
committer, therefore a different SHA. Upstream's commits never become ancestors of the fork, and
GitHub's comparison counts ancestry, not content. The counter climbs by one per commit landed and
never falls on its own, even when the fork's content is a strict superset of upstream's.

Confirm it is cosmetic rather than assuming it:

```bash
git -C "$FORK" rev-list --left-right --count upstream-local/master...master   # left = behind, right = ahead
git -C "$FORK" diff --stat upstream-local/master master                       # must be insertions only
```

The second command is the real check. If it shows **only the expected files and zero deletions**,
the fork is upstream verbatim plus the guards and the banner is meaningless. Deletions mean the
fork is *not* a superset - investigate that before doing anything else, and never paper over it
with a merge.

### Clearing it

Only when the sync is **caught up** - the high-water mark equals `upstream-local/master`. Never
mid-sync: merging early would make unprocessed upstream commits ancestors and pull all their
content in at once, which defeats one-commit-at-a-time and makes the ledger's high-water mark a
lie. Ask the user first; this rewrites nothing but it does change a branch they publish.

```bash
git -C "$FORK" rev-parse master^{tree}                    # note this hash
git -C "$FORK" merge --no-commit --no-ff upstream-local/master
git -C "$FORK" status --short | grep -E '^(UU|AA|DU|UD|AU|UA)'
```

Read every conflict before resolving. Each one should be a `#if USE_CUSTOM_CHANGES` block against
an **empty** upstream side - upstream contributing nothing, so keeping ours discards nothing. A
conflict in a file that carries no guard means a cherry-pick did not land what it should have:
stop and investigate instead of resolving it.

```bash
git -C "$FORK" checkout --ours <the guarded files>
git -C "$FORK" add <the guarded files>
git -C "$FORK" write-tree                                 # MUST equal the hash noted above
```

That tree-hash equality is the proof the merge changed no content - not one byte gained or lost.
Do **not** shortcut this with `git merge -s ours`: that strategy *asserts* the fork already
contains upstream instead of *proving* it, and silently discards the other side if the assertion
is ever wrong.

Commit the merge with a message saying why the histories were rejoined and that the tree is
unchanged. Then stop - **the no-push rule still applies**. Tell the user to run
`git push origin master` themselves; it is a fast-forward and needs no force. Record the merge in
`UPSTREAM_SYNC.md`.

Afterwards the merge base moves from the fork point to upstream HEAD, so the next run's
`rev-list` range works from there unchanged. Keep cherry-picking one commit at a time - do not
switch to a merge-based flow, which would give up the small, bisectable conflicts that make this
skill work. Just re-merge whenever the counter has climbed again and the fork is caught up.

Done once already: merge `9574e00b` on 2026-09-20 rejoined the histories at upstream `e23d5844`,
taking the fork from 6-behind/17-ahead to 0-behind/18-ahead with a byte-identical tree.
