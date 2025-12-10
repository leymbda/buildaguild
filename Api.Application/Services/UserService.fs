module Api.Application.UserService

open Api.Application.IUserService
open Domain
open FsToolkit.ErrorHandling

let getUserById (di: #UserRepositoryDI): IUserService.GetUserById =
    fun (_session: Session) (userId: Id) -> asyncResult {
        let! user =
            di.UserRepository.GetUserById userId
            |> AsyncResult.requireSome (GetUserByIdError.UserNotFound userId)

        return user
    }

let deleteUserById (di: #UserRepositoryDI): IUserService.DeleteUserById =
    fun (session: Session) (userId: Id) -> asyncResult {
        do! 
            userId = session.UserId
            |> Result.requireTrue DeleteUserByIdError.NotAuthorized
            
        do!
            di.UserRepository.DeleteUserById userId
            |> AsyncResult.requireTrue (DeleteUserByIdError.UserNotFound userId)
    }
