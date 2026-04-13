using Minesweeper.Core.Interfaces;
using Minesweeper.Core.Models;
using ReactiveUI;
using System.Windows.Input;

namespace Minesweeper.App.ViewModels;

public class CellViewModel : ViewModelBase
{
    private readonly IGameEngine _engine;
    private readonly GameViewModel _parent;

    public int Row { get; }
    public int Col { get; }

    private CellVisibility _visibility;
    public CellVisibility Visibility
    {
        get => _visibility;
        set
        {
            this.RaiseAndSetIfChanged(ref _visibility, value);
            this.RaisePropertyChanged(nameof(ContentText));
            this.RaisePropertyChanged(nameof(ContentPathData));
            this.RaisePropertyChanged(nameof(HasText));
            this.RaisePropertyChanged(nameof(HasPath));
            this.RaisePropertyChanged(nameof(CellBackground));
            this.RaisePropertyChanged(nameof(ForegroundScale));
        }
    }

    private bool _isMine;
    public bool IsMine
    {
        get => _isMine;
        set
        {
            this.RaiseAndSetIfChanged(ref _isMine, value);
            this.RaisePropertyChanged(nameof(ContentText));
            this.RaisePropertyChanged(nameof(ContentPathData));
            this.RaisePropertyChanged(nameof(HasText));
            this.RaisePropertyChanged(nameof(HasPath));
        }
    }

    private int _neighborMines;
    public int NeighborMines
    {
        get => _neighborMines;
        set
        {
            this.RaiseAndSetIfChanged(ref _neighborMines, value);
            this.RaisePropertyChanged(nameof(ContentText));
            this.RaisePropertyChanged(nameof(HasText));
            this.RaisePropertyChanged(nameof(ForegroundScale));
        }
    }

    private bool _exploded;
    public bool Exploded
    {
        get => _exploded;
        set
        {
            this.RaiseAndSetIfChanged(ref _exploded, value);
            this.RaisePropertyChanged(nameof(CellBackground));
        }
    }

    private const string SVGBomb = "M512 48C518.9 48 525 52.4 527.2 58.9L540.7 99.3L581.1 112.8C587.6 115 592 121.1 592 128C592 134.9 587.6 141 581.1 143.2L540.7 156.7L527.2 197.1C525 203.6 518.9 208 512 208C505.1 208 499 203.6 496.8 197.1L483.3 156.7L442.9 143.2C436.4 141 432 134.9 432 128C432 121.1 436.4 115 442.9 112.8L483.3 99.3L496.8 58.9C499 52.4 505.1 48 512 48zM353.4 161.4C365.9 148.9 386.2 148.9 398.7 161.4L478.7 241.4C491.2 253.9 491.2 274.2 478.7 286.7L467.8 297.6C475.7 319.6 480 343.3 480 368.1C480 483 386.9 576.1 272 576.1C157.1 576.1 64 482.9 64 368C64 253.1 157.1 160 272 160C296.7 160 320.5 164.3 342.5 172.3L353.4 161.4zM176 368C176 315 219 272 272 272C285.3 272 296 261.3 296 248C296 234.7 285.3 224 272 224C192.5 224 128 288.5 128 368C128 381.3 138.7 392 152 392C165.3 392 176 381.3 176 368z";
    private const string SVGFlag = "M160 96C160 78.3 145.7 64 128 64C110.3 64 96 78.3 96 96L96 544C96 561.7 110.3 576 128 576C145.7 576 160 561.7 160 544L160 422.4L222.7 403.6C264.6 391 309.8 394.9 348.9 414.5C391.6 435.9 441.4 438.5 486.1 421.7L523.2 407.8C535.7 403.1 544 391.2 544 377.8L544 130.1C544 107.1 519.8 92.1 499.2 102.4L487.4 108.3C442.5 130.8 389.6 130.8 344.6 108.3C308.2 90.1 266.3 86.5 227.4 98.2L160 118.4L160 96z";

    public string ContentPathData
    {
        get
        {
            if (Visibility == CellVisibility.Flagged) return SVGFlag;
            if (Visibility == CellVisibility.Revealed && IsMine) return SVGBomb;
            return "";
        }
    }

    public string ContentText
    {
        get
        {
            if (Visibility == CellVisibility.Revealed && !IsMine && NeighborMines > 0)
                return NeighborMines.ToString();
            return "";
        }
    }

    public bool HasPath => !string.IsNullOrEmpty(ContentPathData);
    public bool HasText => !string.IsNullOrEmpty(ContentText);

    public string CellBackground
    {
        get
        {
            if (Exploded) return "Red";
            if (Visibility == CellVisibility.Revealed)
            {
                return _parent.HighContrastEnabled ? "#FFFFFF" : "#DDDDDD";
            }

            return _parent.HighContrastEnabled ? "#222222" : "#F0F0F0";
        }
    }

    public string ForegroundScale
    {
        get
        {
            if (Visibility == CellVisibility.Flagged) return _parent.HighContrastEnabled ? "#FF3B30" : "Red";
            if (IsMine) return _parent.HighContrastEnabled ? "#000000" : "Black";
            if (_parent.HighContrastEnabled) return "#000000";

            return NeighborMines switch
            {
                1 => "Blue",
                2 => "Green",
                3 => "Red",
                4 => "DarkBlue",
                5 => "Maroon",
                6 => "Teal",
                7 => "Purple",
                _ => "Black"
            };
        }
    }

    public void RefreshVisualState()
    {
        this.RaisePropertyChanged(nameof(ContentText));
        this.RaisePropertyChanged(nameof(ContentPathData));
        this.RaisePropertyChanged(nameof(HasText));
        this.RaisePropertyChanged(nameof(HasPath));
        this.RaisePropertyChanged(nameof(CellBackground));
        this.RaisePropertyChanged(nameof(ForegroundScale));
    }

    public ICommand RevealCommand { get; }
    public ICommand FlagCommand { get; }
    public ICommand ChordCommand { get; }

    public CellViewModel(Cell cell, IGameEngine engine, GameViewModel parent)
    {
        _engine = engine;
        _parent = parent;
        Row = cell.Row;
        Col = cell.Col;

        RevealCommand = ReactiveCommand.Create(() =>
        {
            _parent.RegisterAction();
            _engine.RevealCell(Row, Col);
            _parent.UpdateFromSnapshot();
        });

        FlagCommand = ReactiveCommand.Create(() =>
        {
            _parent.RegisterAction();
            _engine.ToggleFlag(Row, Col);
            _parent.UpdateFromSnapshot();
        });

        ChordCommand = ReactiveCommand.Create(() =>
        {
            _parent.RegisterAction();
            _engine.ChordReveal(Row, Col);
            _parent.UpdateFromSnapshot();
        });

        Update(cell);
    }

    public void Update(Cell cell)
    {
        Visibility = cell.Visibility;
        IsMine = cell.IsMine;
        NeighborMines = cell.NeighborMines;
        Exploded = cell.Exploded;
    }
}
