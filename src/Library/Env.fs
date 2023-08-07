namespace Library

open System
open NXUI
open Microsoft.Extensions.Configuration

open Library.Router
open Library.Services
open Library.Services
open Library.ViewModels


type ApplicationEnvironment =
  abstract Router: Router
  abstract Todos: TodoService
  abstract FsNotes: FediverseNotesService

type PersistentViewModels =
  abstract FediversePage: FediversePageViewModel
  abstract TodosPage: TodoPageViewModel


type ApplicationEnvironmentImpl
  (?router: Router, ?todos: TodoService, ?fsNotes: FediverseNotesService) =

  let config =
    let builder = ConfigurationBuilder()

    for assetUri in AssetLoader.GetAssets(Uri("avares://Library/Assets")) do
      let isAppSettings =
        assetUri.OriginalString.Contains(
          "appsettings",
          StringComparison.InvariantCultureIgnoreCase
        )

      if isAppSettings then
        builder.AddJsonStream(AssetLoader.Open(assetUri)) |> ignore

    builder.Build()

  let router = defaultArg router (Router.Default Page.Todos)
  let todos = defaultArg todos (TodoService.Default())

  let fsNotes =
    let server = config.GetRequiredSection("Project:SERVER_URL")
    defaultArg fsNotes (FediverseNotesService.Default(server.Value))

  interface ApplicationEnvironment with
    member _.Router = router
    member _.Todos = todos

    member _.FsNotes = fsNotes

  interface PersistentViewModels with
    member _.FediversePage = new FediversePageViewModelImpl(fsNotes)
    member _.TodosPage = TodoPageViewModel.Default(todos)

[<AutoOpen>]
module AppEnvPatterns =
  let inline (|AppEnv|) (appEnv: #ApplicationEnvironment) =
    AppEnv(appEnv :> ApplicationEnvironment)

  let inline (|PersistentVm|) (appEnv: #PersistentViewModels) =
    PersistentVm(appEnv :> PersistentViewModels)
