namespace Api.Application

open Domain

module IAuthService =
    type GetSession = string -> Async<Session option>
    type Authenticate = string -> Async<User option>

type IAuthService = {
    GetSession: IAuthService.GetSession
    Authenticate: IAuthService.Authenticate
}

type AuthServiceDI =
    abstract Auth: IAuthService
