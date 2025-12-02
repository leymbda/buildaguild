module Api.Application.UserService

open Api.Application.IUserService
open Domain
open FsToolkit.ErrorHandling

let createUser (di: #UserRepositoryDI): IUserService.CreateUser =
    fun (session: Session) (userId: Id) -> asyncResult {
        do! 
            userId = session.UserId
            |> Result.requireTrue CreateUserError.NotAuthorized

        let! user =
            di.UserRepository.CreateUser userId
            |> AsyncResult.mapError (function
                | IUserRepository.CreateUserError.DatabaseError e -> CreateUserError.ServerError e
                | IUserRepository.CreateUserError.UserAlreadyExists id -> CreateUserError.UserAlreadyExists id
            )

        return user
    }
        
let getUserById (di: #UserRepositoryDI): IUserService.GetUserById =
    fun (_session: Session) (userId: Id) -> asyncResult {
        let! user =
            di.UserRepository.GetUserById userId
            |> AsyncResult.mapError (function
                | IUserRepository.GetUserByIdError.DatabaseError e -> GetUserByIdError.ServerError e
                | IUserRepository.GetUserByIdError.UserNotFound id -> GetUserByIdError.UserNotFound id
            )

        return user
    }

let deleteUserById (di: #UserRepositoryDI): IUserService.DeleteUserById =
    fun (session: Session) (userId: Id) -> asyncResult {
        do! 
            userId = session.UserId
            |> Result.requireTrue DeleteUserByIdError.NotAuthorized
            
        let! user =
            di.UserRepository.DeleteUserById userId
            |> AsyncResult.mapError (function
                | IUserRepository.DeleteUserByIdError.DatabaseError e -> DeleteUserByIdError.ServerError e
                | IUserRepository.DeleteUserByIdError.UserNotFound id -> DeleteUserByIdError.UserNotFound id
            )

        return user
    }
