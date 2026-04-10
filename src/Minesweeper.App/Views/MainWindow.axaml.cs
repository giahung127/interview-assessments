using Avalonia.Controls;
using Avalonia.Input;
using Minesweeper.App.ViewModels;

namespace Minesweeper.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.R && DataContext is MainWindowViewModel vm)
        {
            if (vm.GameViewModel.RestartCommand.CanExecute(null))
            {
                vm.GameViewModel.RestartCommand.Execute(null);
            }
        }
    }
}
