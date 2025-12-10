namespace rec Api.Application

open Domain

type IUserService = {
    GetUserById: IUserService.GetUserById
    DeleteUserById: IUserService.DeleteUserById
}

module IUserService =
    type GetUserByIdError =
        | UserNotFound of Id
        | ServerError of string

    type GetUserById = Session -> Id -> Async<Result<User, GetUserByIdError>>

    type DeleteUserByIdError =
        | NotAuthorized
        | UserNotFound of Id
        | ServerError of string

    type DeleteUserById = Session -> Id -> Async<Result<unit, DeleteUserByIdError>>

type UserServiceDI =
    abstract UserService: IUserService
