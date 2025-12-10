module Api.Infrastructure.Persistence.D1

open Api.Application
open Api.Application.IUserRepository
open Domain
open Fable.Bindings.CloudflareWorkers
open FsToolkit.ErrorHandling

let upsertUser (di: #EnvDI): UpsertUser =
    fun id displayName -> async {
        let! res =
            di.Env.D1.prepare("INSERT OR REPLACE INTO users (id, display_name) VALUES (?, ?) RETURNING *")
            |> D1PreparedStatement.bind [| Id.toString id; displayName |]
            |> D1PreparedStatement.query UserModel.decoder

        Fable.Core.JS.console.log(res)

        return res.results
            |> Array.head
            |> UserModel.toDomain
    }

let getUserById (di: #EnvDI): GetUserById =
    fun id -> async {
        let! res =
            di.Env.D1.prepare("SELECT * FROM users WHERE id = ?")
            |> D1PreparedStatement.bind [| Id.toString id |]
            |> D1PreparedStatement.query (UserModel.decoder)

        Fable.Core.JS.console.log(res)

        return
            res.results
            |> Array.tryHead
            |> Option.map UserModel.toDomain
    }

let deleteUserById (di: #EnvDI): DeleteUserById =
    fun id ->
        di.Env.D1.prepare("DELETE FROM users WHERE id = ?")
        |> D1PreparedStatement.bind [| Id.toString id |]
        |> D1PreparedStatement.meta
        |> Async.map (_.rows_written >> (<>) 0)
