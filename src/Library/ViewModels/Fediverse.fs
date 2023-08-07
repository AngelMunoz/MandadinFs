namespace Library.ViewModels


open System

open Avalonia
open Avalonia.Data
open Avalonia.Controls

open NXUI.FSharp.Extensions

open FSharp.Control.Reactive

open Library.Services
open Library.Types


[<Struct>]
type ChangePageOptions =
  | Next
  | Prev
  | Page of uint

type FediversePageViewModel =
  abstract member NotesSnapshot: MkNote list

  abstract member Notes: IObservable<MkNote list>

  abstract member Page: int with get

  abstract member Limit: int with get

  abstract member ChangePage: options: ChangePageOptions -> Async<unit>

type FediversePageViewModelImpl(notes: FediverseNotesService) =
  let mutable page = 1
  let mutable limit = 10
  let _notes = Subject.behavior []

  interface FediversePageViewModel with
    member _.Page = page
    member _.Limit = limit
    member _.Notes = _notes
    member _.NotesSnapshot = _notes.Value

    member _.ChangePage(options: ChangePageOptions) = async {
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
