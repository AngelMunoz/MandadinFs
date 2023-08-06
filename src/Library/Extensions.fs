[<AutoOpen>]
module Library.Extensions

open System.Runtime.CompilerServices
open Avalonia.Controls
open Avalonia.Controls.Presenters
open Avalonia


[<Extension>]
type GridExtensions =

  [<Extension>]
  static member inline rowDefinitions(grid: Grid, rows: string) =
    grid.RowDefinitions <- RowDefinitions.Parse rows
    grid

  [<Extension>]
  static member inline columnDefinitions(grid: Grid, columns: string) =
    grid.ColumnDefinitions <- ColumnDefinitions.Parse columns
    grid
