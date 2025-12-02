namespace Api.Application

open Domain

module IAuthService =
    type LoginError =
        | InvalidCode
        | ServerError of string

    type Login = string -> Async<Result<{| User: User; SessionToken: string |}, LoginError>>

    type LogoutError =
        | SessionNotFound
        | ServerError of string

    type Logout = string -> Async<Result<unit, LogoutError>>
    
type IAuthService = {
    Login: IAuthService.Login
    Logout: IAuthService.Logout
}

type AuthServiceDI =
    abstract AuthService: IAuthService
