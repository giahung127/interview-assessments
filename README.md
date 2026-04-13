# Avalonia Minesweeper

A modern, fast, and fully featured local desktop version of the classic Minesweeper built with .NET + Avalonia UI. It provides classic gameplay with some modern quality-of-life enhancements.

## Controls

- **Left Click**: Reveal a cell.
- **Right Click**: Toggle a flag on a block.
- **Middle Click (or Chord Click)**: Quick-reveal neighboring cells if flags around a tile perfectly match the bomb count on the opened cell.

## Mechanics
- Standard difficulties including Beginner `9x9 (10 Mines)`, Intermediate `16x16 (40 Mines)`, and Expert `30x16 (99 Mines)`. 
- **First-Click Safety**: You will mathematically never hit a mine on your first move. The underlying deterministic game engine relocates the initial mine before evaluation.
- Classic chording / flood revealing are exact replicas of original mechanics.
- A fully stable UI with scrollable layouts designed specifically against window scaling failures gracefully handling expert board densities on tightly cramped screen sizes. 

## Supported Platforms
This application natively supports local execution on **Windows** and **macOS**. You can download pre-compiled versions directly from the `publish` directory:
- `publish/Windows/Minesweeper.App.exe`
- **macOS**: `publish/macOS_Bundle/Minesweeper.app`

## Compiling from source

If you want to run the project securely through the core MSBuild loop via dot-net CLI bindings:
1. Guarantee `.NET 10.0` or greater is available in the environment paths.
2. `dotnet restore`
3. `dotnet run --project src/Minesweeper.App` 
(or `dotnet build` then execution). 

## Credits
Done as part of a demonstration of agentic development capabilities.
