module Api.Infrastructure.Persistence.D1

open Api.Application
open Api.Application.IUserRepository
open FsToolkit.ErrorHandling

let createUser (di: #EnvDI): CreateUser =
    fun id -> asyncResult {
        return! Error (CreateUserError.DatabaseError "Not implemented")
    }

let getUserById (di: #EnvDI): GetUserById =
    fun id -> asyncResult {
        return! Error (GetUserByIdError.DatabaseError "Not implemented")
    }

let deleteUserById (di: #EnvDI): DeleteUserById =
    fun id -> asyncResult {
        return! Error (DeleteUserByIdError.DatabaseError "Not implemented")
    }

// NOTE: D1 queries should return a simple record matching the type that the js would return, which should then be
//       mapped into the domain type with some sort of validation. Could potentially stringify then deserialize, might
//       want to do this always as part of a helper util function? Should these helpers be made for KV too?
