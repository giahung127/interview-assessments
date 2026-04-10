# Avalonia Minesweeper v1 Plan (Balanced Modern, Local-First)

## Summary
Build a desktop Minesweeper app in `.NET + Avalonia UI` with complete classic mechanics, modern UX polish, and competitive local-only value features.

Success criteria:
- Core gameplay is bug-free and matches expected Minesweeper behavior.
- UI is fast, intuitive, keyboard-accessible, and pleasant on Windows/macOS.
- Competitive value comes from local progression systems (daily challenge, stats, personal records) without backend dependencies.

## Implementation changes

### Phase 1: Discovery + foundation planning (orchestrator-led)
Owners: `orchestrator`, `project-planner`, `explorer-agent`, `code-archaeologist`.
- Confirm no existing app code conflicts.
- Define solution structure:
  - `Minesweeper.App` (Avalonia UI)
  - `Minesweeper.Core` (pure game engine/domain)
  - `Minesweeper.Tests` (unit/integration tests)
- Produce a task breakdown per phase with explicit handoff criteria.

### Phase 2: Core domain and engine
Owners: `game-developer` + `architecture` skill.
- Implement pure domain model and rules.
- Types: `GameStatus`, `CellVisibility`, `Cell`, `Board`, `DifficultyPreset`, `GameSession`.
- Interfaces: `IGameEngine`, `IBoardGenerator`, `IClockService`, `IRandomProvider`.
- Lock gameplay behaviors:
  - Difficulties: Beginner `9x9/10`, Intermediate `16x16/40`, Expert `30x16/99`, plus custom.
  - First-click safety (regenerate/relocate so first reveal is never mine).
  - Left reveal, right flag toggle, chord reveal on revealed numbered cells.
  - Iterative flood reveal (queue/BFS, no recursion).
  - Win when all non-mine cells are revealed; loss on mine reveal.
  - Timer, remaining mine counter, restart/new game, replay same seed.

### Phase 3: Avalonia UI + interaction design
Owners: `game-developer`, `clean-code`, `tdd-workflow`.
- Implement MVVM app shell.
- Main HUD: difficulty selector, mine counter, timer, restart face/status.
- Board view: responsive grid with clear visual states (`hidden/revealed/flagged/exploded`).
- Input: mouse + keyboard (arrow navigation, reveal/flag shortcuts, restart shortcut).
- UX quality rules:
  - Immediate visual feedback for reveal/flag/chord.
  - High-contrast theme option + reduced motion toggle.
  - Wrong-flag indication on loss; clean win/loss result panel.

### Phase 4: Competitive local-first features
Owners: `game-developer`, `performance-optimizer`, `qa-automation-engineer`.
- Add value features without cloud:
  - Daily Challenge: deterministic board seeded by local date (one puzzle/day).
  - Personal Records: best time per difficulty, win streaks, win rate, games played.
  - Performance stats: average solve time and action efficiency summary.
  - Session quality-of-life: last-used settings persistence and quick rematch.
- Persistence interfaces:
  - `ISettingsStore`, `IStatsStore`, `IDailyChallengeService`.
- Storage choice: local SQLite database for reliable stats/history and schema evolution.

### Phase 5: Verification + release hardening
Owners: `test-engineer`, `qa-automation-engineer`, `debugger`, `documentation-writer`.
- Build validation pipeline:
  - `dotnet format --verify-no-changes`
  - `dotnet build -c Release`
  - `dotnet test -c Release`
- Package and smoke-test release artifacts for Windows and macOS.
- Create user-facing README with controls, rules, and feature overview.

## Phase breakdown with handoff criteria

### Phase 1 output (foundation planning)
Owners: `orchestrator`, `project-planner`, `explorer-agent`, `code-archaeologist`.
- Discovery complete:
  - Repository has no conflicting app implementation code.
  - Existing docs (`PLAN.md`, `AGENT.md`) were reviewed and constraints captured.
- Solution baseline complete:
  - Root solution exists and includes:
    - `src/Minesweeper.App/Minesweeper.App.csproj`
    - `src/Minesweeper.Core/Minesweeper.Core.csproj`
    - `tests/Minesweeper.Tests/Minesweeper.Tests.csproj`
  - Project references are wired:
    - `Minesweeper.App -> Minesweeper.Core`
    - `Minesweeper.Tests -> Minesweeper.Core`
- Phase handoff criteria to Phase 2:
  - Project layout exists exactly as above.
  - No gameplay/domain behavior is implemented yet.
  - Public contracts listed below are accepted as Phase 2 implementation targets.

### Phase 2 output (core engine)
Owners: `game-developer` + `architecture` skill.
- Deliverables:
  - Domain entities, interfaces, and deterministic board generation.
  - First-click safety, reveal/flag/chord behaviors, status transitions.
  - Unit test suite for engine invariants.
- Phase handoff criteria to Phase 3:
  - `Minesweeper.Core` has no Avalonia/UI references.
  - Core tests pass for deterministic generation and gameplay transitions.
  - `GameSessionSnapshot` contract is stable for UI binding.

### Phase 3 output (Avalonia UI)
Owners: `game-developer`, `clean-code`, `tdd-workflow`.
- Deliverables:
  - MVVM app shell, HUD, board rendering, and interaction bindings.
  - Keyboard accessibility baseline and user feedback states.
  - ViewModel tests for command/state behavior.
- Phase handoff criteria to Phase 4:
  - All primary gameplay interactions can be completed from UI.
  - Win/loss/reset flows are reflected correctly in the UI state.
  - Theme/accessibility toggles are wired and test-covered at ViewModel layer.

### Phase 4 output (local competitive features)
Owners: `game-developer`, `performance-optimizer`, `qa-automation-engineer`.
- Deliverables:
  - Daily challenge seed service, stats/records persistence, settings persistence.
  - SQLite-backed store layer (`ISettingsStore`, `IStatsStore` implementations).
  - Performance metrics summaries surfaced in app.
- Phase handoff criteria to Phase 5:
  - Existing core/UI behavior remains regression-free.
  - Stats and daily challenge behavior are deterministic and persisted.
  - Data schema and migration strategy are documented.

### Phase 5 output (verification + release)
Owners: `test-engineer`, `qa-automation-engineer`, `debugger`, `documentation-writer`.
- Deliverables:
  - Full quality gate runs and release smoke checks.
  - Packaged artifacts for supported platforms.
  - End-user documentation and controls reference.
- Completion criteria:
  - `dotnet format --verify-no-changes` passes.
  - `dotnet build -c Release` passes.
  - `dotnet test -c Release` passes.
  - README reflects final shipped behavior and controls.

## Public interfaces / contracts
- `IGameEngine`
  - `StartNewGame(DifficultyPreset preset, int? seed = null)`
  - `RevealCell(int row, int col)`
  - `ToggleFlag(int row, int col)`
  - `ChordReveal(int row, int col)`
  - `GetSnapshot() : GameSessionSnapshot`
- `IDailyChallengeService`
  - `GetDailySeed(DateOnly localDate)`
- `ISettingsStore`
  - `Load() : UserSettings`
  - `Save(UserSettings settings)`
- `IStatsStore`
  - `RecordGame(GameResult result)`
  - `GetSummary() : PlayerStatsSummary`
  - `GetBestTimes() : BestTimes`

## Test plan
- Engine unit tests:
  - First reveal never hits a mine.
  - Exact mine count and neighbor counts are correct after generation.
  - Flood reveal opens connected empty region + border numbers only.
  - Flagging never reveals cells; chord reveal only when flag-count condition is met.
  - Win/loss transitions are deterministic and single-transition.
- Determinism tests:
  - Same seed always creates identical board.
  - Daily seed for same date is stable across launches.
- ViewModel tests:
  - Commands update bindable state correctly (timer, counters, status text, dialogs).
  - Restart fully resets board/timer/flags/transient states.
- UI automation/smoke:
  - New game flow, difficulty switch, reveal/flag/chord interactions.
  - Keyboard-only playability baseline.
  - High-contrast and reduced-motion toggles apply correctly.
- Performance checks:
  - Large-board reveal operations remain responsive (no UI thread stalls).

## Assumptions and defaults
- Product direction: Balanced Modern.
- Supported platforms for v1: Windows + macOS.
- Data model: Local-only persistence (no accounts, no cloud leaderboard).
- Architecture: strict MVVM; game engine stays UI-agnostic in `Minesweeper.Core`.
- Competitive differentiation in v1 is through daily challenges + personal progression, not social features.
