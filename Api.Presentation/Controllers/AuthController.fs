module Api.Presentation.AuthController

open Api.Application
open Api.Application.IAuthService
open Browser
open Fetch

let fetch (di: #AuthServiceDI) (req: Request) (parts: string list) =
    async {
        let url = URL.Create(req.url)

        match req.method, parts with
        | "POST", ["login"] ->
            let code = "" // TODO: Get code from query params/body

            match! di.AuthService.Login code with
            | Error LoginError.InvalidCode -> return Response.badRequest "Invalid authorization code"
            | Error (LoginError.ServerError e) -> return Response.internalServerError e
            | Ok data -> 
                let cookie = data.SessionToken // TODO: Make actual cookie using this token

                let res = Response.ok (UserResource.fromDomain data.User) UserResource.encoder
                res.Headers.set("Set-Cookie", cookie)
                return res

        | "DELETE", ["logout"] ->
            let token = "" // TODO: Get token from request cookie

            match! di.AuthService.Logout token with
            | Error LogoutError.SessionNotFound -> return Response.unauthorized()
            | Error (LogoutError.ServerError e) -> return Response.internalServerError e
            | Ok () -> return Response.noContent()

        | _ ->
            return Response.notFound ""
    }
