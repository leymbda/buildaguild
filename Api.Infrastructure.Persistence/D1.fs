module Api.Infrastructure.Persistence.D1

open Api.Application
open Api.Application.IUserRepository
open Domain
open Fable.Bindings.CloudflareWorkers
open FsToolkit.ErrorHandling

let createUser (di: #EnvDI): CreateUser =
    fun id -> asyncResult {
        let! res =
            di.Env.D1.prepare("INSERT INTO users (id) VALUES (?) RETURNING *")
            |> D1PreparedStatement.bind [| Id.toString id |]
            |> D1PreparedStatement.query UserModel.decoder

        let! model =
            res.results
            |> Array.tryHead
            |> Result.requireSome (CreateUserError.UserAlreadyExists id)

        return!
            model
            |> UserModel.toDomain
            |> Result.mapError CreateUserError.DatabaseError
    }

let getUserById (di: #EnvDI): GetUserById =
    fun id -> asyncResult {
        let! res =
            di.Env.D1.prepare("SELECT * FROM users WHERE id = ?")
            |> D1PreparedStatement.bind [| Id.toString id |]
            |> D1PreparedStatement.query (UserModel.decoder)

        let! model =
            res.results
            |> Array.tryHead
            |> Result.requireSome (GetUserByIdError.UserNotFound id)
            
        return!
            model
            |> UserModel.toDomain
            |> Result.mapError GetUserByIdError.DatabaseError
    }

let deleteUserById (di: #EnvDI): DeleteUserById =
    fun id ->
        di.Env.D1.prepare("DELETE FROM users WHERE id = ?")
        |> D1PreparedStatement.bind [| Id.toString id |]
        |> D1PreparedStatement.meta
        |> Async.map (_.rows_written >> (<>) 0)
