namespace Api.Presentation

open Domain
open FsToolkit.ErrorHandling
open Thoth.Json

type UserResource = {
    Id: string
    DisplayName: string
}

module UserResource =
    let fromDomain (v: User): UserResource =
        {
            Id = Id.toString v.Id
            DisplayName = v.DisplayName
        }

    let toDomain (v: UserResource): Result<User, string> =
        result {
            let! id =
                Id.fromString v.Id
                |> Result.requireSome "Invalid id provided"

            return {
                Id = id
                DisplayName = v.DisplayName
            }
        }

    let encoder (v: UserResource) =
        Encode.object [
            "id", Encode.string v.Id
            "display_name", Encode.string v.DisplayName
        ]

    let decoder: Decoder<UserResource> =
        Decode.object (fun get -> {
            Id = get.Required.Field "id" Decode.string
            DisplayName = get.Required.Field "display_name" Decode.string
        })
