module Api.Application.AuthService

open Api.Application.IAuthService
open FsToolkit.ErrorHandling

let login (di: #SessionCacheDI & #UserRepositoryDI): Login =
    fun code -> asyncResult {
        return! Error (LoginError.ServerError "Not implemented")
    }

let logout (di: #SessionCacheDI): Logout =
    fun token -> asyncResult {
        return! Error (LogoutError.ServerError "Not implemented")
    }

// TODO: Sometimes the token will expire and need to be refreshed. Ideally this can be handled automatically. Maybe in here?
// TODO: Consider how to handle TTL for sessions
