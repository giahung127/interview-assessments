using Minesweeper.Core.Interfaces;
using Minesweeper.Core.Models;

namespace Minesweeper.App.Services;

public sealed class SqliteStatsStore : IStatsStore
{
    private readonly SqliteStorage _storage;

    public SqliteStatsStore(SqliteStorage storage)
    {
        _storage = storage;
    }

    public void RecordGame(GameResult result)
    {
        using var connection = _storage.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO game_results (
                played_at_utc,
                difficulty_name,
                rows,
                cols,
                mine_count,
                did_win,
                elapsed_seconds,
                action_count,
                seed,
                is_daily_challenge
            ) VALUES (
                $playedAtUtc,
                $difficultyName,
                $rows,
                $cols,
                $mineCount,
                $didWin,
                $elapsedSeconds,
                $actionCount,
                $seed,
                $isDailyChallenge
            );";

        command.Parameters.AddWithValue("$playedAtUtc", result.PlayedAtUtc.ToString("O"));
        command.Parameters.AddWithValue("$difficultyName", result.Difficulty.Name);
        command.Parameters.AddWithValue("$rows", result.Difficulty.Rows);
        command.Parameters.AddWithValue("$cols", result.Difficulty.Cols);
        command.Parameters.AddWithValue("$mineCount", result.Difficulty.MineCount);
        command.Parameters.AddWithValue("$didWin", result.DidWin ? 1 : 0);
        command.Parameters.AddWithValue("$elapsedSeconds", Math.Max(0, result.ElapsedSeconds));
        command.Parameters.AddWithValue("$actionCount", Math.Max(0, result.ActionCount));
        command.Parameters.AddWithValue("$seed", (object?)result.Seed ?? DBNull.Value);
        command.Parameters.AddWithValue("$isDailyChallenge", result.IsDailyChallenge ? 1 : 0);

        command.ExecuteNonQuery();
    }

    public PlayerStatsSummary GetSummary()
    {
        using var connection = _storage.OpenConnection();

        int gamesPlayed;
        int gamesWon;
        using (var totals = connection.CreateCommand())
        {
            totals.CommandText = @"
                SELECT
                    COUNT(*) AS played,
                    COALESCE(SUM(did_win), 0) AS won
                FROM game_results;";

            using var reader = totals.ExecuteReader();
            reader.Read();
            gamesPlayed = reader.GetInt32(0);
            gamesWon = reader.GetInt32(1);
        }

        var currentWinStreak = 0;
        var bestWinStreak = 0;
        var running = 0;

        using (var streaks = connection.CreateCommand())
        {
            streaks.CommandText = @"
                SELECT did_win
                FROM game_results
                ORDER BY played_at_utc ASC, id ASC;";

            using var reader = streaks.ExecuteReader();
            while (reader.Read())
            {
                var didWin = reader.GetInt32(0) == 1;
                running = didWin ? running + 1 : 0;
                if (running > bestWinStreak)
                {
                    bestWinStreak = running;
                }
            }
        }

        using (var current = connection.CreateCommand())
        {
            current.CommandText = @"
                SELECT did_win
                FROM game_results
                ORDER BY played_at_utc DESC, id DESC;";

            using var reader = current.ExecuteReader();
            while (reader.Read())
            {
                var didWin = reader.GetInt32(0) == 1;
                if (!didWin)
                {
                    break;
                }

                currentWinStreak++;
            }
        }

        var averageSolveSeconds = 0.0;
        var averageActionsPerWin = 0.0;
        var averageActionsPerSecond = 0.0;

        using (var perf = connection.CreateCommand())
        {
            perf.CommandText = @"
                SELECT
                    AVG(CASE WHEN did_win = 1 THEN elapsed_seconds END),
                    AVG(CASE WHEN did_win = 1 THEN action_count END),
                    AVG(CASE WHEN elapsed_seconds > 0 THEN (CAST(action_count AS REAL) / elapsed_seconds) END)
                FROM game_results;";

            using var reader = perf.ExecuteReader();
            if (reader.Read())
            {
                if (!reader.IsDBNull(0))
                {
                    averageSolveSeconds = reader.GetDouble(0);
                }

                if (!reader.IsDBNull(1))
                {
                    averageActionsPerWin = reader.GetDouble(1);
                }

                if (!reader.IsDBNull(2))
                {
                    averageActionsPerSecond = reader.GetDouble(2);
                }
            }
        }

        return new PlayerStatsSummary(
            gamesPlayed,
            gamesWon,
            currentWinStreak,
            bestWinStreak,
            new PerformanceStatsSummary(averageSolveSeconds, averageActionsPerWin, averageActionsPerSecond));
    }

    public BestTimes GetBestTimes()
    {
        using var connection = _storage.OpenConnection();
        return new BestTimes(
            GetBestTimeByDifficulty(connection, DifficultyPreset.Beginner.Name),
            GetBestTimeByDifficulty(connection, DifficultyPreset.Intermediate.Name),
            GetBestTimeByDifficulty(connection, DifficultyPreset.Expert.Name));
    }

    private static int? GetBestTimeByDifficulty(Microsoft.Data.Sqlite.SqliteConnection connection, string difficultyName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT MIN(elapsed_seconds)
            FROM game_results
            WHERE did_win = 1
              AND difficulty_name = $difficultyName;";
        command.Parameters.AddWithValue("$difficultyName", difficultyName);

        var value = command.ExecuteScalar();
        if (value == null || value is DBNull)
        {
            return null;
        }

        return Convert.ToInt32(value);
    }
}
