namespace rec Api.Application

open Domain

type IUserRepository = {
    UpsertUser: IUserRepository.UpsertUser
    GetUserById: IUserRepository.GetUserById
    DeleteUserById: IUserRepository.DeleteUserById
}

module IUserRepository =
    type UpsertUser = Id -> string -> Async<User>

    type GetUserById = Id -> Async<User option>
    
    type DeleteUserById = Id -> Async<bool>

type UserRepositoryDI =
    abstract UserRepository: IUserRepository
