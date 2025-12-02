namespace Api.Application

open Domain

module IUserApplicationService =
    type CreateUserError =
        | NotAuthorized
        | AlreadyExists

    type CreateUser = Session -> Id -> Async<Result<User, CreateUserError>>

    type GetUserError =
        | DoesNotExist

    type GetUser = Session -> Id -> Async<Result<User, GetUserError>>

    type DeleteUserError =
        | NotAuthorized
        | DoesNotExist

    type DeleteUser = Session -> Id -> Async<Result<unit, DeleteUserError>>

type IUserApplicationService = {
    CreateUser: IUserApplicationService.CreateUser
    GetUser: IUserApplicationService.GetUser
    DeleteUser: IUserApplicationService.DeleteUser
}

type UserApplicationServiceDI =
    abstract UserApplicationService: IUserApplicationService
