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

open Router


type ApplicationEnvironment =
  abstract Router: Router



type ApplicationEnvironmentImpl(?router: Router) =
  let router = defaultArg router (Router.Default Page.Todos)

  interface ApplicationEnvironment with
    member _.Router = router

module private Shell =
  open Library.FsFediverse
  open Library.Todos
  open Avalonia.Threading

  let Content (appEnv: ApplicationEnvironment) : Control =

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
        | Page.Todos -> UI.TodosPage()
        | Page.FsNotes(page, limit) -> UI.FedNotesPage(page, limit)
        | Page.FsNote note -> UI.FedNotePage(note)
      )

    StackPanel()
      .spacing(4.)
      .children(
        StackPanel()
          .orientation(Layout.Orientation.Horizontal)
          .spacing(4.)
          .children(
            Button()
              .content("Todos")
              .OnClickHandler(fun _ _ ->
                Dispatcher.UIThread.Post(fun _ ->
                  appEnv.Router.Navigate(Page.Todos)
                )
              ),
            Button()
              .content("FsNotes")
              .OnClickHandler(fun _ _ ->
                Dispatcher.UIThread.Post(fun _ ->
                  appEnv.Router.Navigate(Page.FsNotes(1, 10))
                )
              )
          ),

        UserControl()
          .name("MainContent")
          .content(content.ToBinding())
      // possible footer
      )

type SharedApplication(appEnv: ApplicationEnvironment) =
  inherit Application()

  override this.Initialize() =
    this.Styles.Add(FluentTheme())
    this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

  override this.OnFrameworkInitializationCompleted() =
    match this.ApplicationLifetime with
    | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
      let window =
        Window()
          .title("Mandadin F#")
          .width(428)
          .height(428)
          .content(Shell.Content appEnv)

      desktopLifetime.MainWindow <- window
    | :? ISingleViewApplicationLifetime as singleViewLifetime ->
      singleViewLifetime.MainView <- Shell.Content appEnv
    | _ -> ()

    base.OnFrameworkInitializationCompleted()
