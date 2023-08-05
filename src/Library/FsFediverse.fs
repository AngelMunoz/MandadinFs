namespace Library.FsFediverse


open System.Reactive.Subjects

open Avalonia
open Avalonia.Data
open Avalonia.Controls
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent

open NXUI.FSharp.Extensions


module UI =

  let FedNotesPage (page: int, limit: int) : Control =
    StackPanel()
      .children(TextBlock().text("Fediverse Notes"))

  let FedNotePage (note: string) : Control =
    StackPanel()
      .children(TextBlock().text("Fediverse note"))
