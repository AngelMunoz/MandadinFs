namespace Library

open Avalonia
open Avalonia.Data
open Avalonia.Controls
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent

open NXUI
open NXUI.Extensions
open NXUI.FSharp.Extensions

open FSharp.Control.Reactive

open Library.Router
open Library.Services
open Library.Services.Todos
open Library.Services.Fediverse
open Library.FsFediverse.ViewModel


type ApplicationEnvironment =
  abstract Router: Router
  abstract Todos: TodoService
  abstract FsNotes: FediverseNotesService

type PersistentViewModels =
  abstract FsNotes: FediverseNotesViewModel


type ApplicationEnvironmentImpl
  (
    baseUrl: string,
    ?router: Router,
    ?todos: TodoService,
    ?fsNotes: FediverseNotesService
  ) =
  let router = defaultArg router (Router.Default Page.Todos)
  let todos = defaultArg todos (Todos.Default())
  let fsNotes = defaultArg fsNotes (Fediverse.Default(baseUrl))

  interface ApplicationEnvironment with
    member _.Router = router
    member _.Todos = todos

    member _.FsNotes = fsNotes

  interface PersistentViewModels with
    member _.FsNotes = new FediverseNotesVm(fsNotes)

[<AutoOpen>]
module AppEnvPatterns =
  let inline (|AppEnv|) (appEnv: #ApplicationEnvironment) =
    AppEnv(appEnv :> ApplicationEnvironment)

  let inline (|PersistentVm|) (appEnv: #PersistentViewModels) =
    PersistentVm(appEnv :> PersistentViewModels)



module private Shell =
  open Library.FsFediverse
  open Library.Todos

  let Content (AppEnv appEnv & PersistentVm persistentVms) : Control =

    let content =
      appEnv.Router.CurrentPage
#if DEBUG
      |> Observable.log "Page Change"
#endif
      |> Observable.distinctUntilChanged
#if DEBUG
      |> Observable.log "Page Change After Distinct"
#endif
      |> Observable.map(fun page ->
        match page with
        | Page.Todos -> UI.TodosPage(appEnv.Todos)
        | Page.FsNotes(page, limit) -> UI.FedNotesPage(persistentVms.FsNotes)
        | Page.FsNote note -> UI.FedNotePage(note)
      )

    DockPanel()
      .lastChildFill(true)
      .children(
        StackPanel()
          .DockTop()
          .margin(8, 4)
          .orientation(Layout.Orientation.Horizontal)
          .spacing(4.)
          .children(
            Button()
              .content("Todos")
              .OnClickHandler(fun _ _ -> appEnv.Router.Navigate(Page.Todos)),
            Button()
              .content("FsNotes")
              .OnClickHandler(fun _ _ ->
                appEnv.Router.Navigate(Page.FsNotes(1, 10))
              )
          ),
        // possible footer as .DockBottom()
        ContentControl()
          .margin(8, 4)
          .AlignHorizontalCenterWithPanel(true)
          .content(content.ToBinding(), mode = BindingMode.OneWay)
      )

type SharedApplication(appEnv: ApplicationEnvironmentImpl) =
  inherit Application()

  override this.Initialize() =
    this.Styles.Add(FluentTheme())
    this.RequestedThemeVariant <- Styling.ThemeVariant.Default

  override this.OnFrameworkInitializationCompleted() =
    match this.ApplicationLifetime with
    | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
      let window =
        Window()
          .title("Mandadin F#")
          .minWidth(320)
          .minHeight(428)
          .content(Shell.Content appEnv)

      desktopLifetime.MainWindow <- window
    | :? ISingleViewApplicationLifetime as singleViewLifetime ->
      singleViewLifetime.MainView <- Shell.Content appEnv
    | _ -> ()

    base.OnFrameworkInitializationCompleted()
