module Library.Router

open System
open System.Collections.Generic

open FSharp.Control.Reactive

[<Struct; RequireQualifiedAccess>]
type Page =
  | Todos
  | FsNotes of page: int * limit: int
  | FsNote of note: string

[<Struct; RequireQualifiedAccess>]
type ResetNavigationOptions =
  | Clear
  | CurrentAsStart
  | StartAs of page: Page


type Router =

  abstract member CurrentSnapshot: Page with get
  abstract member CurrentPage: IObservable<Page>
  abstract member Navigate: page: Page -> unit
  abstract member NavigateUp: unit -> unit
  abstract member ClearStack: ?resetNavOptions: ResetNavigationOptions -> unit


[<RequireQualifiedAccess>]
module Router =

  let Default (initialPage: Page) =
    let stack = Stack<Page> [ initialPage ]
    let routerFeed = Subject.behavior(stack.Peek())

    { new Router with
        member _.CurrentSnapshot = routerFeed.Value
        member _.CurrentPage = routerFeed

        member this.Navigate page =
          match stack.TryPeek() with
          | false, _ -> this.ClearStack()
          | true, value ->
            if value <> page then
              stack.Push page
              routerFeed.OnNext(page)


        member this.NavigateUp() =
          match stack.TryPop() with
          | false, _ -> this.ClearStack()
          | true, _ ->
            match stack.TryPeek() with
            | false, _ -> this.ClearStack()
            | true, value -> routerFeed.OnNext(value)

        member _.ClearStack(?resetNavOptions) =
          let resetNavOptions =
            defaultArg resetNavOptions ResetNavigationOptions.Clear

          stack.Clear()

          match resetNavOptions with
          | ResetNavigationOptions.Clear ->

            stack.Push initialPage
            routerFeed.OnNext(stack.Peek())
          | ResetNavigationOptions.CurrentAsStart ->

            stack.Push routerFeed.Value
            routerFeed.OnNext(stack.Peek())

          | ResetNavigationOptions.StartAs page ->
            stack.Push page
            routerFeed.OnNext(page)
    }
