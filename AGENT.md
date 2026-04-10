# AGENT.md

## Dev environment tips
- Use `.NET 8 SDK` (or the SDK pinned in `global.json`) and verify with `dotnet --info`.
- Restore dependencies before coding with `dotnet restore` from repository root.
- Use `dotnet workload list` to verify required desktop workloads are installed.
- Confirm project identity from the application `.csproj` (`AssemblyName`, `RootNamespace`, and target framework).
- Prefer CLI-first workflows: `dotnet build`, `dotnet test`, and `dotnet run --project <app.csproj>`.
- Use `dotnet sln list` to locate projects instead of scanning folders manually.
- Build in `Release` mode before PR to catch configuration-specific issues: `dotnet build -c Release`.

## Testing instructions
- Find the CI plan in the `.github/workflows` folder.
- Run `dotnet test` from solution root to execute all test projects.
- From a project root, use `dotnet test` for local iteration.
- To focus on one test, use filter syntax: `dotnet test --filter "FullyQualifiedName~<test name>"`.
- Fix all build, analyzer, and test errors until the suite is fully green.
- After moving files or changing namespaces, run `dotnet format --verify-no-changes` to ensure style and analyzers pass.
- Before merging, run: `dotnet format --verify-no-changes`, `dotnet build -c Release`, and `dotnet test -c Release`.
- Add unit tests for game logic (board generation, mine placement, reveal/flood-fill, win/loss detection, flagging).
- Add UI-level tests for critical interaction flows where feasible (left click reveal, right click flag, restart game, timer/reset behavior), using Avalonia-compatible testing approaches.

## PR instructions
- Title format: `[<project_name>] <Title>`
- Always run `dotnet format --verify-no-changes`, `dotnet build -c Release`, and `dotnet test -c Release` before committing.
- Include a short test evidence section in PR description with executed commands and results.
- Include screenshots or short recordings for UI changes.

## Development rules
- [PLAN.md](PLAN.md) is the single source of truth for implementation. Follow it for every coding task.
- Before implementing anything, read `PLAN.md` and map the task to its phase and owners.
- If a requested change affects scope/architecture/contracts in `PLAN.md`, update `PLAN.md` first, then implement.
- Always use [orchestrator.md](.agent/agents/orchestrator.md) to plan every task before implementation.
- Planning must split work into several phases and assign suitable owners from [.agent](.agent): agents in `.agent/agents` and/or skills in `.agent/skills`.
- Required phase template:
  - Phase 1: Discovery and constraints (`explorer-agent`, `code-archaeologist`, `project-planner`).
  - Phase 2: Architecture and state model (`orchestrator`, `game-developer`, `architecture` skill).
  - Phase 3: Implementation (`game-developer`, `clean-code` skill, `tdd-workflow` skill).
  - Phase 4: Verification (`test-engineer`, `qa-automation-engineer`, `testing-patterns` skill).
  - Phase 5: Hardening and docs (`debugger`, `performance-optimizer`, `documentation-writer`).
- Do not start coding before the phase plan is explicit and approved by the orchestrator.
- Keep each phase output explicit: assumptions, deliverables, test plan, and rollback strategy.

## Avalonia Minesweeper rules
- Build the app as a desktop-first Avalonia UI application using MVVM (`Views`, `ViewModels`, `Models`, `Services`).
- Keep game rules engine pure and UI-agnostic; no Avalonia types inside core game logic.
- Generate boards deterministically when a seed is provided to support reproducible tests.
- Enforce first-click safety: first revealed cell must never be a mine.
- Implement flood reveal for empty cells and adjacent-number boundaries with iterative logic to avoid recursion overflow.
- Represent cell state explicitly: `Hidden`, `Revealed`, `Flagged`, and mine metadata.
- Maintain a single game state machine: `NotStarted`, `InProgress`, `Won`, `Lost`.
- Use `ReactiveUI` patterns or `INotifyPropertyChanged` consistently; avoid mixed binding paradigms.
- Keep UI responsive: never block UI thread for board generation or heavy operations.
- Add keyboard-accessible controls for restart and board navigation where practical.
- Validate win condition strictly: all non-mine cells revealed (flags alone are insufficient).
- Ensure right-click toggles flags without revealing cells.
- Reset must fully clear board state, timer, flags, and transient animations.
- Centralize difficulty presets (Beginner/Intermediate/Expert/custom) in configuration, not hardcoded in views.
- Preserve separation of concerns:
  - `Models`: board and cell domain types.
  - `Services`: mine placement, reveal logic, timer/score abstractions.
  - `ViewModels`: command orchestration and bindable state.
  - `Views`: XAML layout and styling only.
- Add logging hooks for critical game transitions (start, mine hit, win, reset) to aid debugging.
- Keep styling consistent with Avalonia resource dictionaries; avoid inline style duplication.
- Every behavior change requires corresponding automated tests and, for UI changes, updated screenshots in PR artifacts.
