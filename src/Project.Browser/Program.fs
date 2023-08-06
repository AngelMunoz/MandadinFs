open System
open System.Runtime.Versioning
open Avalonia
open Avalonia.Browser
open Avalonia.ReactiveUI
open Microsoft.Extensions.Configuration
open Library


module Program =
  [<assembly: SupportedOSPlatform("browser")>]
  do ()

  let Environment =
    match Environment.GetEnvironmentVariable("MANDADIN_ENVIRONMENT") with
    | null -> ".Production"
    | value -> $".{value}"

  let configuration =
    ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional = true, reloadOnChange = true)
      .AddJsonFile(
        $"appsettings{Environment}.json",
        optional = true,
        reloadOnChange = true
      )
      .Build()

  [<CompiledName "BuildAvaloniaApp">]
  let buildAvaloniaApp () =
    AppBuilder.Configure<SharedApplication>(fun () ->
      // customize initialization if needed
      let project = configuration.GetRequiredSection("Project:SERVER_URL")
      SharedApplication(ApplicationEnvironmentImpl(project.Value))
    )

  [<EntryPoint>]
  let main argv =
    task {
      do!
        buildAvaloniaApp()
          .WithInterFont()
          .UseReactiveUI()
          .StartBrowserAppAsync("out")
    }
    |> ignore

    0
