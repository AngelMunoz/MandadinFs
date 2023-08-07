namespace Library.Views

open System

open Avalonia
open Avalonia.Data
open Avalonia.Controls
open Avalonia.Controls.Templates

open NXUI.FSharp.Extensions

open FSharp.Control.Reactive

open IcedTasks

open Markdown.Avalonia

open Library
open Library.Types
open Library.ViewModels


module Fediverse =

  let inline MkNoteItem (note: MkNote) : Control =
    DockPanel()
      .lastChildFill(true)
      .margin(12)
      .children(
        TextBlock()
          .DockTop()
          .text(note.user.username),
        TextBlock()
          .DockBottom()
          .text(note.createdAt.ToShortDateString()),
        MarkdownScrollViewer()
          .markdown(note.text)
          .theme(MarkdownStyle.FluentAvalonia)
      )

  let FedNotesPage (vm: FediversePageViewModel) : Control =
    DockPanel()
      .lastChildFill(true)
      .OnLoadedHandler(fun _ _ ->
        asyncEx { do! vm.ChangePage(ChangePageOptions.Page 1u) } |> Async.Start
      )
      .children(
        TextBlock()
          .DockTop()
          .text("Fediverse Notes"),
        StackPanel()
          .DockBottom()
          .OrientationHorizontal()
          .spacing(4.)
          .children(Label().content($"Page: {vm.Page}")),
        ScrollViewer()
          .content(
            ItemsControl()
              .itemsSource(vm.Notes.ToBinding(), mode = BindingMode.OneWay)
              .itemTemplate(
                new FuncDataTemplate<MkNote>(
                  (fun todo _ -> MkNoteItem todo),
                  true
                )
              )
          )
      )

  let FedNotePage (note: string) : Control =
    StackPanel()
      .children(TextBlock().text("Fediverse note"))
