namespace Library

open System
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
open Library.Views

module private Shell =

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
        | Page.Todos -> Todos.TodosPage(persistentVms.TodosPage)
        | Page.FsNotes -> Fediverse.FedNotesPage(persistentVms.FediversePage)
        | Page.FsNote note -> Fediverse.FedNotePage(note)
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
              .OnClickHandler(fun _ _ -> appEnv.Router.Navigate(Page.FsNotes))
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
