module Api.Presentation.UserController

open Api.Application
open Api.Application.IUserService
open Browser
open Domain
open Fetch

let fetch (di: #UserServiceDI & #SessionCacheDI) (req: Request) (parts: string list) =
    async {
        let url = URL.Create(req.url)

        let token = "TEMP_SESSION_TOKEN" // TODO: Extract session from header (TBD how it will be added)
        let! session = di.SessionCache.GetSession token

        match req.method, parts, session with
        | "GET", [Id.FromRoute userId], Some session ->
            match! di.UserService.GetUserById session userId with
            | Error (GetUserByIdError.UserNotFound id) -> return Response.notFound $"User '{Id.toString id}' not found"
            | Error (GetUserByIdError.ServerError e) -> return Response.internalServerError e
            | Ok user -> return Response.ok (UserResource.fromDomain user) UserResource.encoder

        | "GET", [userId], Some _ ->
            return Response.badRequest $"Invalid 'userId' of '{userId}' provided in route"
            
        | "GET", [_], None ->
            return Response.unauthorized()

        | "DELETE", [Id.FromRoute userId], Some session ->
            match! di.UserService.DeleteUserById session userId with
            | Error DeleteUserByIdError.NotAuthorized -> return Response.forbidden()
            | Error (DeleteUserByIdError.UserNotFound id) -> return Response.notFound $"User '{Id.toString id}' not found"
            | Error (DeleteUserByIdError.ServerError e) -> return Response.internalServerError e
            | Ok () -> return Response.noContent()

        | "DELETE", [userId], Some _ ->
            return Response.badRequest $"Invalid 'userId' of '{userId}' provided in route"
            
        | "DELETE", [_], None ->
            return Response.unauthorized()

        | _ ->
            return Response.notFound ""

        // TODO: Can auth be cleaned up in a way that only returns 401 on known endpoints?
    }
