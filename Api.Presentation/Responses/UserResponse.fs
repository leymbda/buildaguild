namespace Api.Presentation

open Domain
open Thoth.Json

type UserResponse = {
    Id: string
}

module UserResponse =
    let fromDomain (user: User): UserResponse =
        {
            Id = Id.toString user.Id
        }

    let encoder (v: UserResponse) =
        Encode.object [
            "id", Encode.string v.Id
        ]

    let encode (user: User): string =
        fromDomain user
        |> encoder
        |> Encode.toString 4
