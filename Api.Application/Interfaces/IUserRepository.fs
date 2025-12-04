namespace rec Api.Application

open Domain

type IUserRepository = {
    CreateUser: IUserRepository.CreateUser
    GetUserById: IUserRepository.GetUserById
    DeleteUserById: IUserRepository.DeleteUserById
}

module IUserRepository =
    type CreateUserError =
        | UserAlreadyExists of Id
        | DatabaseError of string
        
    type CreateUser = Id -> Async<Result<User, CreateUserError>>

    type GetUserByIdError =
        | UserNotFound of Id
        | DatabaseError of string

    type GetUserById = Id -> Async<Result<User, GetUserByIdError>>
    
    type DeleteUserById = Id -> Async<bool>

type UserRepositoryDI =
    abstract UserRepository: IUserRepository
