namespace Project.Desktop

open System
open Avalonia
open Avalonia.ReactiveUI

open Library

module Program =

  [<CompiledName "BuildAvaloniaApp">]
  let buildAvaloniaApp () =
    AppBuilder
      .Configure<SharedApplication>(fun _ ->
        // customize initialization if needed
        SharedApplication(ApplicationEnvironmentImpl())
      )
      .UsePlatformDetect()
      .WithInterFont()
      .LogToTrace(areas = Array.empty)
      .UseReactiveUI()

  [<EntryPoint; STAThread>]
  let main argv =
    buildAvaloniaApp()
      .StartWithClassicDesktopLifetime(argv)
