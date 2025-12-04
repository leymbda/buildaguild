module Api.Presentation.UserController

open Api.Application
open Api.Application.IUserService
open Browser
open Domain
open Fetch

let fetch (di: #UserServiceDI & #SessionCacheDI) (req: Request) (parts: string list) =
    async {
        let url = URL.Create(req.url)

        let token = "" // TODO: Extract session from header (TBD how it will be added)
        let! session = di.SessionCache.GetSession token

        match req.method, parts, session with
        | "PUT", [Id.FromRoute userId], Some session ->
            match! di.UserService.CreateUser session userId with
            | Error CreateUserError.NotAuthorized -> return Response.forbidden()
            | Error (CreateUserError.UserAlreadyExists id) -> return Response.conflict $"User '{Id.toString id}' already exists"
            | Error (CreateUserError.ServerError e) -> return Response.internalServerError e
            | Ok user ->
                let link = $"{url.origin}/api/users/{Id.toString user.Id}"
                return Response.created (UserResource.fromDomain user) UserResource.encoder link

        | "PUT", [userId], Some _ ->
            return Response.badRequest $"Invalid 'userId' of '{userId}' provided in route"

        | "PUT", [_], None ->
            return Response.unauthorized()

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
