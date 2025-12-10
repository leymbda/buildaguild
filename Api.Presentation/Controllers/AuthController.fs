module Api.Presentation.AuthController

open Api.Application
open Api.Application.IAuthService
open Browser
open Fetch
open FsToolkit.ErrorHandling

let fetch (di: #AuthServiceDI) (req: Request) (parts: string list) =
    asyncResult {
        let url = URL.Create(req.url)

        match req.method, parts with
        | "POST", ["login"] ->
            let! code =
                url.searchParams.get("code")
                |> Result.requireSome (Response.badRequest "Missing 'code' query parameter")

            let! data =
                di.AuthService.Login code
                |> AsyncResult.mapError (function
                    | LoginError.InvalidCode -> Response.badRequest "Invalid authorization code"
                    | LoginError.ServerError e -> Response.internalServerError e
                )

            let res = Response.ok (LoginResponse.fromDomain data.AccessToken data.User) LoginResponse.encoder
            return res

        | "POST", ["logout"] ->
            let! token =
                Headers.tryGet "Cookie" req.headers // TODO: Get token from request cookie
                |> Result.requireSome (Response.unauthorized())

            // TODO: Above should probably be a common utility as authorization is common (needs to be in UserController too)
            
            do!
                di.AuthService.Logout token
                |> AsyncResult.mapError (function
                    | LogoutError.SessionNotFound -> Response.unauthorized()
                    | LogoutError.ServerError e -> Response.internalServerError e
                )

            // TODO: Revoke token from Discord too

            return Response.noContent()

        | _ ->
            return Response.notFound ""
    }
    |> AsyncResult.defaultWith id
