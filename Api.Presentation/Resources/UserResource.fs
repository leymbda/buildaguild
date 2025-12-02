module Api.Presentation.UserResource

open Api.Application
open Api.Application.IUserService
open Browser
open Domain
open Fetch
open FsToolkit.ErrorHandling
open Thoth.Json

type UserResource = {
    Id: string
}

let fromDomain (user: User): UserResource =
    {
        Id = Id.toString user.Id
    }

let encoder (v: UserResource) =
    Encode.object [
        "id", Encode.string v.Id
    ]

let fetch (di: #UserServiceDI & #AuthServiceDI) (req: Request) (parts: string list) =
    async {
        let url = URL.Create(req.url)

        let token = "" // TODO: Extract session from header (TBD how it will be added)
        let! session =
            di.AuthService.GetSession token
            |> AsyncResult.foldResult Some (fun _ -> None)

        match req.method, parts, session with
        | "PUT", [Id.FromRoute userId], Some session ->
            match! di.UserService.CreateUser session userId with
            | Error CreateUserError.NotAuthorized -> return Response.forbidden()
            | Error (CreateUserError.UserAlreadyExists id) -> return Response.conflict $"User '{Id.toString id}' already exists"
            | Error (CreateUserError.ServerError e) -> return Response.internalServerError e
            | Ok user ->
                let link = $"{url.origin}/api/users/{Id.toString user.Id}"
                return Response.created (fromDomain user) encoder link

        | "PUT", [userId], _ ->
            return Response.badRequest $"Invalid 'userId' of '{userId}' provided in route"

        | "GET", [Id.FromRoute userId], Some session ->
            match! di.UserService.GetUserById session userId with
            | Error (GetUserByIdError.UserNotFound id) -> return Response.notFound $"User '{Id.toString id}' not found"
            | Error (GetUserByIdError.ServerError e) -> return Response.internalServerError e
            | Ok user -> return Response.ok (fromDomain user) encoder

        | "GET", [userId], _ ->
            return Response.badRequest $"Invalid 'userId' of '{userId}' provided in route"

        | "DELETE", [Id.FromRoute userId], Some session ->
            match! di.UserService.DeleteUserById session userId with
            | Error DeleteUserByIdError.NotAuthorized -> return Response.forbidden()
            | Error (DeleteUserByIdError.UserNotFound id) -> return Response.notFound $"User '{Id.toString id}' not found"
            | Error (DeleteUserByIdError.ServerError e) -> return Response.internalServerError e
            | Ok () -> return Response.noContent()

        | "DELETE", [userId], _ ->
            return Response.badRequest $"Invalid 'userId' of '{userId}' provided in route"

        | _, _, None ->
            return Response.unauthorized()

        | _ ->
            return Response.notFound ""
    }
