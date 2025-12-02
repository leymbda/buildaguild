module Api.Application.UserApplicationService

open Api.Application.IUserApplicationService
open Domain
open FsToolkit.ErrorHandling

let createUser (di: #UserServiceDI): IUserApplicationService.CreateUser =
    fun (session: Session) (userId: Id) -> asyncResult {
        do! 
            userId = session.UserId
            |> Result.requireTrue CreateUserError.NotAuthorized

        return!
            di.Users.CreateUser userId
            |> AsyncResult.requireSome CreateUserError.AlreadyExists
    }
        
let getUser (di: #UserServiceDI): IUserApplicationService.GetUser =
    fun (_session: Session) (userId: Id) -> asyncResult {
        // TODO: Consider if/how fetching users should be restricted (based on team membership, etc)

        return!
            di.Users.GetUserById userId
            |> AsyncResult.requireSome GetUserError.DoesNotExist
    }

let deleteUser (di: #UserServiceDI): IUserApplicationService.DeleteUser =
    fun (session: Session) (userId: Id) -> asyncResult {
        do! 
            userId = session.UserId
            |> Result.requireTrue DeleteUserError.NotAuthorized

        do! 
            di.Users.DeleteUserById userId
            |> AsyncResult.requireTrue DeleteUserError.DoesNotExist
    }
