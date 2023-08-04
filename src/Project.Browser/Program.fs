open System.Runtime.Versioning
open Avalonia
open Avalonia.Browser
open Avalonia.ReactiveUI

open Library


module Program =
  [<assembly: SupportedOSPlatform("browser")>]
  do ()

  [<CompiledName "BuildAvaloniaApp">]
  let buildAvaloniaApp () =
    AppBuilder.Configure<SharedApplication>(fun () ->
      // customize initialization if needed
      SharedApplication(ApplicationEnvironmentImpl())
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
