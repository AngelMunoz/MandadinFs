namespace Library.FsFediverse


open System

open Avalonia
open Avalonia.Data
open Avalonia.Controls
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent

open NXUI.FSharp.Extensions

open FSharp.Control.Reactive

open Library
open Library.Services.Fediverse
open Library.Types


module ViewModel =
  open IcedTasks

  [<Struct>]
  type ChangePageOptions =
    | Next
    | Prev
    | Page of uint

  type FediverseNotesViewModel =
    abstract member NotesSnapshot: MkNote list

    abstract member Notes: IObservable<MkNote list>

    abstract member Page: int with get

    abstract member Limit: int with get

    abstract member ChangePage: options: ChangePageOptions -> ColdTask<unit>

  type FediverseNotesVm(notes: FediverseNotesService) =
    let mutable page = 1
    let mutable limit = 10
    let _notes = Subject.behavior []

    interface FediverseNotesViewModel with
      member _.Page = page
      member _.Limit = limit
      member _.Notes = _notes
      member _.NotesSnapshot = _notes.Value

      member _.ChangePage(options: ChangePageOptions) = coldTask {
        match options with
        | Next -> page <- page + 1
        | Prev -> page <- if page - 1 = 0 then 1 else page - 1
        | Page p -> page <- if p = 0u then 1 else int p

        let! result = notes.find(page = page, limit = limit)

        match result with
        | Ok notes -> _notes.OnNext notes
        | Error err -> eprintfn "Error: %s" err
      }

    interface IDisposable with
      member _.Dispose() = _notes.Dispose()

open ViewModel




module UI =
  open IcedTasks
  open Avalonia.Controls.Templates
  open Markdown.Avalonia

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

  let FedNotesPage (vm: FediverseNotesViewModel) : Control =
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
