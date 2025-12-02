module Api.Application.AuthService

open Api.Application.IAuthService
open FsToolkit.ErrorHandling

let authenticate (di: #UserRepositoryDI): Authenticate =
    fun (token: string) -> asyncResult {
        return! Error (AuthenticateError.ServerError "Not implemented")
    }

let getSession (di: #SessionCacheDI): GetSession =
    fun (token: string) -> asyncResult {
        return! Error (GetSessionError.ServerError "Not implemented")
    }

// TODO: createSession, etc(?)
