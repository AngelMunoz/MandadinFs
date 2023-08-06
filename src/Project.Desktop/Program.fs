namespace Project.Desktop

open System
open Avalonia
open Avalonia.ReactiveUI
open Microsoft.Extensions.Configuration

open Library

module Program =

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
    AppBuilder
      .Configure<SharedApplication>(fun _ ->
        // customize initialization if needed
        let project = configuration.GetRequiredSection("Project:SERVER_URL")

        SharedApplication(ApplicationEnvironmentImpl(project.Value))
      )
      .UsePlatformDetect()
      .WithInterFont()
      .LogToTrace(areas = Array.empty)
      .UseReactiveUI()

  [<EntryPoint; STAThread>]
  let main argv =
    buildAvaloniaApp()
      .StartWithClassicDesktopLifetime(argv)
