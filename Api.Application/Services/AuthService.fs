module Api.Application.AuthService

open Api.Application.IAuthService
open Domain
open FsToolkit.ErrorHandling

let login (di: #TimeDI & #DiscordDI & #SessionCacheDI & #UserRepositoryDI): Login =
    fun code -> asyncResult {
        let! oauth =
            di.Discord.OAuthTokenExchange code
            |> AsyncResult.setError LoginError.InvalidCode

        let token = "TEMP_SESSION_TOKEN" // TODO: Generate session token (arbitrary secure string)

        let! user =
            di.Discord.GetCurrentUser oauth.AccessToken
            |> AsyncResult.setError (LoginError.ServerError "Unexpectedly failed to get user using new access token")

        let session: Session = {
            UserId = Id.create user.Id
            AccessToken = oauth.AccessToken
            RefreshToken = oauth.RefreshToken
            ExpiresAt = di.Time.GetCurrentTime().AddSeconds(oauth.ExpiresIn)
        }

        do! di.SessionCache.PutSession token session

        let! user =
            di.UserRepository.GetUserById session.UserId
            |> Async.map Result.toOption

        return {|
            SessionToken = token
            User = user
            AccessToken = oauth.AccessToken
        |}
    }

let logout (di: #SessionCacheDI): Logout =
    fun token -> asyncResult {
        let! session =
            di.SessionCache.GetSession token
            |> AsyncResult.requireSome LogoutError.SessionNotFound

        do! di.SessionCache.DeleteSession token
    }

// TODO: Sometimes the token will expire and need to be refreshed. Ideally this can be handled automatically. Maybe in here?
// TODO: Consider how to handle TTL for sessions
