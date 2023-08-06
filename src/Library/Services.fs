namespace Library.Services

open System
open System.Reactive.Subjects

open FSharp.Control.Reactive
open Thoth.Json.Net
open Library.Types


module Todos =
  type TodoService =

    abstract TodosSnapshot: Todo list

    abstract Todos: IObservable<Todo list>

    abstract AddNew: title: string * ?isCompleted: bool -> unit

    abstract Toggle: todo: Todo -> bool

    abstract Delete: todo: Todo -> bool

    abstract Update: todo: Todo * title: string * ?completed: bool -> bool

  let Default () =
    let todosObs: BehaviorSubject<Todo list> = Subject.behavior []

    { new TodoService with
        member _.TodosSnapshot = todosObs.Value

        member _.Todos = todosObs

        member this.AddNew(title, ?isCompleted) =
          {
            Id = Guid.NewGuid()
            Title = title
            Completed = defaultArg isCompleted false
          }
          :: this.TodosSnapshot
          |> todosObs.OnNext

        member this.Toggle todo =
          let snapshot = this.TodosSnapshot

          match
            snapshot |> List.tryFindIndex(fun existing -> existing = todo)
          with
          | Some index ->
            snapshot
            |> List.updateAt index {
              todo with
                  Completed = not todo.Completed
            }
            |> todosObs.OnNext

            true
          | None -> false

        member this.Delete todo =
          let snapshot = this.TodosSnapshot

          match
            snapshot |> List.tryFindIndex(fun existing -> existing = todo)
          with
          | Some index ->
            snapshot |> List.removeAt index |> todosObs.OnNext

            true
          | None -> false


        member this.Update(todo, title, ?completed) =
          let completed = defaultArg completed todo.Completed

          let snapshot = this.TodosSnapshot

          match
            snapshot |> List.tryFindIndex(fun existing -> existing = todo)
          with
          | Some index ->
            snapshot
            |> List.updateAt index {
              todo with
                  Title = title
                  Completed = completed
            }
            |> todosObs.OnNext

            true
          | None -> false
    }


module Fediverse =
  open IcedTasks
  open FsHttp

  type FediverseNotesService =
    abstract member find:
      ?page: int * ?limit: int -> ColdTask<Result<MkNote list, string>>

    abstract member findOne: note: string -> ColdTask<Result<MkNote, string>>

  let Default (baseUrl: string) =

    { new FediverseNotesService with
        member _.find(?page: int, ?limit: int) = coldTask {
          let page = defaultArg page 1
          let limit = defaultArg limit 10

          let req = http {
            GET baseUrl
            query [ "page", page; "limit", limit ]
          }

          let! response = req |> Request.sendAsync
          let! response = response |> Response.toTextAsync

          return Decode.fromString (Decode.list MkNote.Decoder) response
        }

        member _.findOne(note: string) = coldTask {
          let req = http {
            GET baseUrl
            query [ ("note", note) ]
          }

          let! response = req |> Request.sendAsync
          let! response = response |> Response.toTextAsync

          return Decode.fromString MkNote.Decoder response
        }
    }
