namespace Api.Application

open Domain

module IUserService =
    type CreateUser = Id -> Async<User option>

    type GetUserById = Id -> Async<User option>

    type DeleteUserById = Id -> Async<bool>

type IUserService = {
    CreateUser: IUserService.CreateUser
    GetUserById: IUserService.GetUserById
    DeleteUserById: IUserService.DeleteUserById
}

type UserServiceDI =
    abstract Users: IUserService
