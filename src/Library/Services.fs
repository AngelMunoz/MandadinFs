namespace Library.Services

open System
open FSharp.Control.Reactive
open System.Reactive.Subjects


module Todos =
  [<Struct>]
  type Todo = {
    Id: Guid
    Title: string
    Completed: bool
  }

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
