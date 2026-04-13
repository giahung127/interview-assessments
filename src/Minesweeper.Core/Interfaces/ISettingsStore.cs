namespace Minesweeper.Core.Interfaces;

using Minesweeper.Core.Models;

public interface ISettingsStore
{
    UserSettings Load();
    void Save(UserSettings settings);
}
