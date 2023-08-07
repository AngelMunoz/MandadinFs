[<AutoOpen>]
module Library.Extensions

open System.Runtime.CompilerServices
open Avalonia
open Markdown.Avalonia

[<Extension>]
type MarkdownExtensions =

  [<Extension>]
  static member inline markdown
    (
      instance: MarkdownScrollViewer,
      markdown: string
    ) =
    instance.Markdown <- markdown
    instance

  [<Extension>]
  static member inline plugins
    (
      instance: MarkdownScrollViewer,
      ?plugins: MdAvPlugins
    ) =
    let plugins = defaultArg plugins (MdAvPlugins())
    instance.Plugins <- plugins
    instance

  [<Extension>]
  static member inline theme
    (
      instance: MarkdownScrollViewer,
      ?theme: Styling.Styles
    ) =
    let theme = defaultArg theme MarkdownStyle.FluentAvalonia
    instance.MarkdownStyle <- theme
    instance
