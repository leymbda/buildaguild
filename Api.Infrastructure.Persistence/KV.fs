module Api.Infrastructure.Persistence.KV

open Api.Application
open Api.Application.ISessionCache
open Fable.Core
open Thoth.Json

let putSession (di: #EnvDI): PutSession =
    fun token session ->
        let json =
            session
            |> SessionModel.fromDomain
            |> SessionModel.encoder
            |> Encode.toString 0

        di.Env.SESSION_KV.put(token, json)
        |> Async.AwaitPromise

let getSession (di: #EnvDI): GetSession =
    fun token ->
        di.Env.SESSION_KV.get(token)
        |> Promise.map (Option.bind (
            Decode.fromString SessionModel.decoder
            >> Result.bind SessionModel.toDomain
            >> Result.toOption
        ))
        |> Async.AwaitPromise

let deleteSession (di: #EnvDI): DeleteSession =
    fun token ->
        di.Env.SESSION_KV.delete(token)
        |> Async.AwaitPromise
