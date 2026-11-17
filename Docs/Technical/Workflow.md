# Local development workflow

## Verified setup

- Windows Unity Editor: `C:\Program Files\Unity\Hub\Editor\6000.6.9f1\Editor\Unity.exe`
- Project version: `ProjectSettings/ProjectVersion.txt` (6000.6.9f1)
- Rendering: Unity's matching URP 17.6.0 package with project settings in `Assets/_Game/Settings/`.
- Git for Windows is installed.
- Orchestrator.inc desktop is installed. Its agent catalog reports Claude Code and Codex installed and authorized.

The Editor version is read from the project at runtime by the check script. If Unity is installed elsewhere, pass its verified executable path with `-UnityPath`.

## Run checks

Close any interactive Unity Editor using this checkout, then run from the repository root in PowerShell:

```powershell
.\Scripts\Invoke-UnityChecks.ps1 -Mode Compile
.\Scripts\Invoke-UnityChecks.ps1 -Mode EditMode
```

The script writes a log and an EditMode XML report under `.artifacts/unity/`. It fails for a nonzero Unity exit, compiler errors, a missing test report, no tests, or failed tests. First launch in a new worktree can take several minutes while Unity imports assets. Do not run checks on the same checkout concurrently.

## Git and Unity files

Track `Assets` and matching `.meta` files, `Packages`, `ProjectSettings`, design docs, scripts, and agent instructions. The `.gitignore` excludes Unity caches and local output. `ProjectSettings/EditorSettings.asset` uses Force Text serialization. Keep metadata visible in the Editor's version control settings. Review `git status` before each commit.

## Orchestrator.inc

This is one local Git repository. Register its root folder in Orchestrator's **Add project** flow. Set the base branch to `main` because this local repository has no remote default branch. Choose Codex or Claude Code as the worker and the other as a reviewer if desired. Both are already detected by Orchestrator on this computer.

Each Orchestrator worker gets a separate Git worktree. Run the check script inside that worktree, never against this original checkout while Unity is open here. Each worktree builds its own ignored `Library` cache. Keep the first task limited to a documented, approved behavior once design decisions are ready. A useful first worker task now is a documentation edit or a review of this setup.

## Design handoff

Start at `Docs/README.md`. Give an agent the relevant `Docs/Dogs/` spec and a narrow outcome. Only **Approved** rules are implementation requirements; **Proposed** and **Open questions** are for discussion. Do not add dog AI code until a behavior is approved.
