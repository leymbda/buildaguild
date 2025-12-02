module Api.Application.AuthService

open Domain
open FsToolkit.ErrorHandling

let authenticate (di: #UserServiceDI): IAuthService.Authenticate =
    fun (token: string) ->
        asyncResult {
            let id = Id.create() // TODO: Validate token then extract ID

            return! di.Users.GetUserById(id)

            // TODO: Attempt to cache user in KV rather than hitting DO every time
        }
        |> AsyncResult.defaultValue None

let getSession (di: #UserServiceDI): IAuthService.GetSession =
    fun (token: string) ->
        asyncResult {
            let session = { UserId = Id.create(); ExpiresAt = System.DateTime.MaxValue } // TODO: Get session from KV

            return Some session
        }
        |> AsyncResult.defaultValue None

// TODO: createSession, etc(?)
// TODO: Sessions should probably also be stored in DO and cached in KV
