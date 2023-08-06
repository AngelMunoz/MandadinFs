namespace Library.Types

open System
open Thoth.Json.Net

[<Struct>]
type Todo = {
  Id: Guid
  Title: string
  Completed: bool
}


type MkInstance = {
  name: string
  softwareName: string
  softwareVersion: string
  iconUrl: string option
  faviconUrl: string option
  themeColor: string
} with

  static member Decoder: Decoder<MkInstance> =
    Decode.object(fun get -> {
      name = get.Required.Field "name" Decode.string
      softwareName = get.Required.Field "softwareName" Decode.string
      softwareVersion = get.Required.Field "softwareVersion" Decode.string
      iconUrl = get.Optional.Field "iconUrl" Decode.string
      faviconUrl = get.Optional.Field "faviconUrl" Decode.string
      themeColor = get.Required.Field "themeColor" Decode.string
    })

type MkUser = {
  username: string
  host: string option
  instance: MkInstance option
} with

  static member Decoder: Decoder<MkUser> =
    Decode.object(fun get -> {
      username = get.Required.Field "username" Decode.string
      host = get.Optional.Field "host" Decode.string
      instance = get.Optional.Field "instance" MkInstance.Decoder
    })


type MkNote = {
  id: string
  createdAt: DateTime
  url: string option
  text: string
  user: MkUser
  reply: MkNote option
} with

  static member Decoder: Decoder<MkNote> =
    Decode.object(fun get -> {
      id = get.Required.Field "id" Decode.string
      createdAt = get.Required.Field "createdAt" Decode.datetimeLocal
      url = get.Optional.Field "url" Decode.string
      text = get.Required.Field "text" Decode.string
      user = get.Required.Field "user" MkUser.Decoder
      reply = get.Optional.Field "reply" MkNote.Decoder
    })
