using Avalonia.Controls;
using Avalonia.Input;
using Minesweeper.App.ViewModels;

namespace Minesweeper.App.Views;

public partial class GameView : UserControl
{
    public GameView()
    {
        InitializeComponent();
    }

    private void Cell_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control control && control.DataContext is CellViewModel vm)
        {
            var p = e.GetCurrentPoint(control);
            if (p.Properties.IsRightButtonPressed)
            {
                vm.FlagCommand.Execute(null);
                e.Handled = true;
            }
            else if (p.Properties.IsMiddleButtonPressed)
            {
                vm.ChordCommand.Execute(null);
                e.Handled = true;
            }
        }
    }
}
