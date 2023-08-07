namespace Library.Services

open System

open Library.Types

type TodoService =

  abstract find: unit -> Todo list
  abstract findOne: id: Guid -> Todo option
  abstract create: title: string * ?completed: bool -> unit
  abstract update: todo: Todo -> unit
  abstract delete: todo: Todo -> bool

[<RequireQualifiedAccess>]
module TodoService =
  let Default () =
    // usually they come from an embedded db or from the network
    // we'll settle for a ResizeArray for now
    let todos = ResizeArray<Todo>()

    { new TodoService with
        member _.create(title: string, completed: bool option) =
          let todo = {
            Id = Guid.NewGuid()
            Title = title
            Completed = defaultArg completed false
          }

          todos.Add todo


        member _.delete(todo: Todo) = todos.Remove todo

        member _.find() : Todo list = todos |> Seq.toList

        member _.findOne(id: Guid) : Todo option =
          todos |> Seq.tryFind(fun todo -> todo.Id = id)

        member _.update(todo: Todo) =

          match todos |> Seq.tryFindIndex(fun existing -> existing = todo) with
          | Some index ->
            let updated = {
              todo with
                  Completed = not todo.Completed
            }

            todos.[index] <- updated
          | None -> ()
    }
