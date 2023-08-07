namespace Library.ViewModels

open System

open System.Reactive.Subjects
open FSharp.Control.Reactive

open Library.Types
open Library.Services

type TodoPageViewModel =

  abstract TodosSnapshot: Todo list

  abstract Todos: IObservable<Todo list>

  abstract AddNew: title: string * ?isCompleted: bool -> unit

  abstract Toggle: todo: Todo -> unit

  abstract Delete: todo: Todo -> bool

  abstract Update: todo: Todo * title: string * ?completed: bool -> unit

[<RequireQualifiedAccess>]
module TodoPageViewModel =

  let Default (service: TodoService) =
    let todosObs: BehaviorSubject<Todo list> = Subject.behavior(service.find())

    { new TodoPageViewModel with
        member _.TodosSnapshot = todosObs.Value

        member _.Todos = todosObs

        member _.AddNew(title, ?isCompleted) =
          service.create(title, ?completed = isCompleted)
          service.find() |> todosObs.OnNext

        member _.Toggle todo =
          service.update(
            {
              todo with
                  Completed = not todo.Completed
            }
          )

          service.find() |> todosObs.OnNext


        member _.Delete todo =
          if service.delete todo then
            service.find() |> todosObs.OnNext
            true
          else
            false


        member _.Update(todo, title, ?completed) =
          service.update(
            {
              todo with
                  Title = title
                  Completed = defaultArg completed todo.Completed
            }
          )

          service.find() |> todosObs.OnNext
    }
