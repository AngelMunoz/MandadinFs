namespace Library.Todos


open System.Reactive.Subjects

open Avalonia
open Avalonia.Data
open Avalonia.Controls
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent

open NXUI.FSharp.Extensions


module UI =

  let TodosPage () =
    StackPanel()
      .children(TextBlock().text("Todos"))
