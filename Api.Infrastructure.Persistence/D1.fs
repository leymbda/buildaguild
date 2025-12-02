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
