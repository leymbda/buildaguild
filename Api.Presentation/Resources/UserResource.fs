module Api.Presentation.UserResource

open Api.Application
open Api.Application.IUserApplicationService
open Browser
open Domain
open Fetch
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

let fetch (di: #UserApplicationServiceDI & #AuthServiceDI) (req: Request) (parts: string list) =
    async {
        let url = URL.Create(req.url)

        let token = "" // TODO: Extract session from header (TBD how it will be added)
        let! session = di.Auth.GetSession token

        match req.method, parts, session with
        | "PUT", [Id.Match userId], Some session ->
            match! di.UserApplicationService.CreateUser session userId with
            | Error CreateUserError.NotAuthorized -> return Response.forbidden()
            | Error CreateUserError.AlreadyExists -> return Response.conflict "User already exists"
            | Ok user ->
                let link = $"{url.origin}/api/users/{Id.toString user.Id}"
                return Response.created (fromDomain user) encoder link

        | "PUT", [userId], _ ->
            return Response.badRequest $"Invalid 'userId' of '{userId}' provided in route"

        | "GET", [Id.Match userId], Some session ->
            match! di.UserApplicationService.GetUser session userId with
            | Error GetUserError.DoesNotExist -> return Response.notFound "User not found"
            | Ok user -> return Response.ok (fromDomain user) encoder

        | "GET", [userId], _ ->
            return Response.badRequest $"Invalid 'userId' of '{userId}' provided in route"

        | "DELETE", [Id.Match userId], Some session ->
            match! di.UserApplicationService.DeleteUser session userId with
            | Error DeleteUserError.NotAuthorized -> return Response.forbidden()
            | Error DeleteUserError.DoesNotExist -> return Response.notFound "User not found"
            | Ok () -> return Response.noContent()

        | "DELETE", [userId], _ ->
            return Response.badRequest $"Invalid 'userId' of '{userId}' provided in route"

        | _, _, None ->
            return Response.unauthorized()

        | _ ->
            return Response.notFound ""
    }
