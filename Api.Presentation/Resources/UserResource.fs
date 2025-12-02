namespace Api.Presentation

open Domain
open Thoth.Json

type UserResource = {
    Id: string
}

module UserResource =
    let fromDomain (v: User): UserResource =
        {
            Id = Id.toString v.Id
        }

    let encoder (v: UserResource) =
        Encode.object [
            "id", Encode.string v.Id
        ]
