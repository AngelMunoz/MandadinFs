namespace Library.Services

open Thoth.Json.Net
open Library.Types
open FsHttp

type FediverseNotesService =
  abstract member find:
    ?page: int * ?limit: int -> Async<Result<MkNote list, string>>

  abstract member findOne: note: string -> Async<Result<MkNote, string>>

[<RequireQualifiedAccess>]
module FediverseNotesService =

  let Default (baseUrl: string) =

    { new FediverseNotesService with
        member _.find(?page: int, ?limit: int) = async {
          let page = defaultArg page 1
          let limit = defaultArg limit 10

          let req = http {
            GET baseUrl
            query [ "page", page; "limit", limit ]
          }

          let! response = req |> Request.sendAsync
          let! response = response |> Response.toTextAsync

          return Decode.fromString (Decode.list MkNote.Decoder) response
        }

        member _.findOne(note: string) = async {
          let req = http {
            GET baseUrl
            query [ ("note", note) ]
          }

          let! response = req |> Request.sendAsync
          let! response = response |> Response.toTextAsync

          return Decode.fromString MkNote.Decoder response
        }
    }
