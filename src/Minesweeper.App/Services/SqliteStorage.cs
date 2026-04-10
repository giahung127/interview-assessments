using Microsoft.Data.Sqlite;
using System.IO;

namespace Minesweeper.App.Services;

public sealed class SqliteStorage
{
    public string DatabasePath { get; }

    public SqliteStorage(string databasePath)
    {
        DatabasePath = databasePath;

        var directory = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        EnsureSchema();
    }

    public SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection($"Data Source={DatabasePath}");
        connection.Open();
        return connection;
    }

    private void EnsureSchema()
    {
        using var connection = OpenConnection();

        using var versionCommand = connection.CreateCommand();
        versionCommand.CommandText = "PRAGMA user_version;";
        var currentVersion = (long)(versionCommand.ExecuteScalar() ?? 0L);

        if (currentVersion < 1)
        {
            using var settingsTable = connection.CreateCommand();
            settingsTable.CommandText = @"
                CREATE TABLE IF NOT EXISTS settings (
                    key TEXT PRIMARY KEY,
                    value TEXT NOT NULL
                );";
            settingsTable.ExecuteNonQuery();

            using var gamesTable = connection.CreateCommand();
            gamesTable.CommandText = @"
                CREATE TABLE IF NOT EXISTS game_results (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    played_at_utc TEXT NOT NULL,
                    difficulty_name TEXT NOT NULL,
                    rows INTEGER NOT NULL,
                    cols INTEGER NOT NULL,
                    mine_count INTEGER NOT NULL,
                    did_win INTEGER NOT NULL,
                    elapsed_seconds INTEGER NOT NULL,
                    action_count INTEGER NOT NULL,
                    seed INTEGER NULL,
                    is_daily_challenge INTEGER NOT NULL
                );";
            gamesTable.ExecuteNonQuery();

            using var setVersion = connection.CreateCommand();
            setVersion.CommandText = "PRAGMA user_version = 1;";
            setVersion.ExecuteNonQuery();
        }
    }
}
