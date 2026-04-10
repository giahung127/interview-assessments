using Minesweeper.Core.Models;

namespace Minesweeper.Core.Interfaces;

public interface ISettingsStore
{
    UserSettings Load();
    void Save(UserSettings settings);
}
