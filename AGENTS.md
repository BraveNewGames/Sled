# Agent instructions

This repository is a standalone Unity dog AI sandbox. Keep work focused on its documented task. Do not implement dog behaviors until a relevant `Docs/Dogs/` spec has concrete **Approved** rules and the task explicitly asks for them.

Read `Docs/README.md`, then only the specs relevant to the task. Treat Proposed ideas and Open questions as design discussion, not implementation requirements. Record new design decisions only after user approval.

Use the Editor version in `ProjectSettings/ProjectVersion.txt`. Run `Scripts/Invoke-UnityChecks.ps1` from the current checkout for compile and EditMode checks, after closing any interactive Editor using that same checkout. Inspect the generated log and XML result. Never run two Unity processes on one project directory. Orchestrator worktrees are separate Unity projects with their own ignored `Library` cache.

Keep Unity `.meta` files with their assets. Keep project assets and settings in text form for Git review. Do not commit generated `Library`, `Temp`, `Logs`, test output, or local machine paths. Do not add packages unless the task requires them.
