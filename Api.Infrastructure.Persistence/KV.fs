module Api.Infrastructure.Persistence.KV

open Api.Application
open Api.Application.ISessionCache
open Domain
open Fable.Core
open Thoth.Json
open FsToolkit.ErrorHandling

module Session =
    let encoder (v: Session) =
        Encode.object [
            "user_id", Encode.string (Id.toString v.UserId)
            "access_token", Encode.string v.AccessToken
            "refresh_token", Encode.string v.RefreshToken
            "expires_at", Encode.datetime v.ExpiresAt
        ]

    let decoder: Decoder<Session> =
        Decode.object (fun get ->
            {
                UserId = get.Required.Field "user_id" (Decode.string |> Decode.map Id.fromString |> Decode.requireSome)
                AccessToken = get.Required.Field "access_token" Decode.string
                RefreshToken = get.Required.Field "refresh_token" Decode.string
                ExpiresAt = get.Required.Field "expires_at" Decode.datetimeUtc
            }
        )

// TOOD: Probably want to instead encode/decode a kv model that maps into domain model (+decoder for id?)

let putSession (di: #EnvDI): PutSession =
    fun token session ->
        di.Env.SESSION_KV.put(token, session.ToString())
        |> Async.AwaitPromise

let getSession (di: #EnvDI): GetSession =
    fun token ->
        di.Env.SESSION_KV.get(token)
        |> Async.AwaitPromise
        |> Async.map (Option.bind (Decode.fromString Session.decoder >> Result.toOption))

let deleteSession (di: #EnvDI): DeleteSession =
    fun token ->
        di.Env.SESSION_KV.delete(token)
        |> Async.AwaitPromise
