using Minesweeper.Core.Interfaces;
using Minesweeper.Core.Models;

namespace Minesweeper.App.Services;

public sealed class SqliteSettingsStore : ISettingsStore
{
    private const string DifficultyKey = "last_selected_difficulty";
    private const string HighContrastKey = "high_contrast_enabled";

    private readonly SqliteStorage _storage;

    public SqliteSettingsStore(SqliteStorage storage)
    {
        _storage = storage;
    }

    public UserSettings Load()
    {
        using var connection = _storage.OpenConnection();

        var difficulty = ReadSetting(connection, DifficultyKey) ?? UserSettings.Default.LastSelectedDifficulty;
        var highContrast = ParseBool(ReadSetting(connection, HighContrastKey));

        return new UserSettings(difficulty, highContrast);
    }

    public void Save(UserSettings settings)
    {
        using var connection = _storage.OpenConnection();

        WriteSetting(connection, DifficultyKey, settings.LastSelectedDifficulty);
        WriteSetting(connection, HighContrastKey, settings.HighContrastEnabled ? "1" : "0");
    }

    private static string? ReadSetting(Microsoft.Data.Sqlite.SqliteConnection connection, string key)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT value FROM settings WHERE key = $key";
        command.Parameters.AddWithValue("$key", key);
        return command.ExecuteScalar() as string;
    }

    private static void WriteSetting(Microsoft.Data.Sqlite.SqliteConnection connection, string key, string value)
    {
        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO settings(key, value)
            VALUES ($key, $value)
            ON CONFLICT(key) DO UPDATE SET value = excluded.value;";
        command.Parameters.AddWithValue("$key", key);
        command.Parameters.AddWithValue("$value", value);
        command.ExecuteNonQuery();
    }

    private static bool ParseBool(string? value)
    {
        return value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }
}
