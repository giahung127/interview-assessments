# Persistence and Migration Strategy

The app stores local data in a SQLite database at:

- `LocalApplicationData/Minesweeper/minesweeper.db`

## Tables

- `settings`
  - Key/value store for user preferences.
  - Current keys:
    - `last_selected_difficulty`
    - `high_contrast_enabled`
    - `reduced_motion_enabled`

- `game_results`
  - One row per completed game.
  - Captures timestamp, difficulty shape, outcome, elapsed time, action count, seed, and daily-challenge marker.

## Schema versioning

Schema versioning uses `PRAGMA user_version`.

- Version `1`:
  - Creates `settings` and `game_results`.
  - Sets `PRAGMA user_version = 1`.

Future changes should follow this pattern:

1. Read `PRAGMA user_version`.
2. Apply incremental migrations in ascending order.
3. Update `PRAGMA user_version` after each successful migration step.
