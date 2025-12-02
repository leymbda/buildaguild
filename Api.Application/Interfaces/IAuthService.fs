namespace Api.Application

open Domain

module IAuthService =
    type AuthenticateError =
        | InvalidToken
        | ServerError of string

    type Authenticate = string -> Async<Result<User, AuthenticateError>>

    type GetSessionError =
        | InvalidToken
        | ServerError of string

    type GetSession = string -> Async<Result<Session, GetSessionError>>

type IAuthService = {
    Authenticate: IAuthService.Authenticate
    GetSession: IAuthService.GetSession
}

type AuthServiceDI =
    abstract AuthService: IAuthService
