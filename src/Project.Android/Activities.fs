namespace Project.Android

open Android.App
open Android.Content.PM
open Avalonia
open Avalonia.ReactiveUI
open Avalonia.Android

open System
open Microsoft.Extensions.Configuration

open Library

module Env =

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

  // customize initialization if needed
  let Android =
    let project = configuration.GetRequiredSection("Project:SERVER_URL")
    ApplicationEnvironmentImpl(project.Value)

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
