namespace Project.Android

open Android.App
open Android.Content.PM
open Avalonia
open Avalonia.ReactiveUI
open Avalonia.Android

open Library

module Env =
  // customize initialization if needed
  let Android = ApplicationEnvironmentImpl()

type AndroidApp() =
  inherit SharedApplication(Env.Android)

[<Activity(Label = "Project.Android",
           Theme = "@style/MyTheme.NoActionBar",
           Icon = "@drawable/icon",
           MainLauncher = true,
           ConfigurationChanges =
             (ConfigChanges.Orientation
              ||| ConfigChanges.ScreenSize
              ||| ConfigChanges.UiMode))>]
type MainActivity() =
  inherit AvaloniaMainActivity<AndroidApp>()

  override _.CustomizeAppBuilder(builder) =
    base
      .CustomizeAppBuilder(builder)
      .WithInterFont()
      .UseReactiveUI()
