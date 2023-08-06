namespace Project.iOS

open System
open Foundation
open Avalonia
open Avalonia.iOS
open Avalonia.ReactiveUI
open Microsoft.Extensions.Configuration

open Library

module Env =

  // customize initialization if needed
  let iOS = ApplicationEnvironmentImpl()

type iOSApp() =
  inherit SharedApplication(Env.iOS)

// The UIApplicationDelegate for the application. This class is responsible for launching the
// User Interface of the application, as well as listening (and optionally responding) to
// application events from iOS.
[<Register("AppDelegate")>]
type AppDelegate() =
  inherit AvaloniaAppDelegate<iOSApp>()

  override _.CustomizeAppBuilder(builder) =
    base
      .CustomizeAppBuilder(builder)
      .WithInterFont()
      .UseReactiveUI()
