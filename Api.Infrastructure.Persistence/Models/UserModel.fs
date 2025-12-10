namespace Api.Infrastructure.Persistence

open Domain
open Thoth.Json

type UserModel = {
    Id: int64
    DisplayName: string
}

module UserModel =
    let encoder (v: UserModel) =
        Encode.object [
            "id", Encode.int64 v.Id
            "display_name", Encode.string v.DisplayName
        ]

    let decoder: Decoder<UserModel> =
        Decode.object (fun get -> {
            Id = get.Required.Field "id" Decode.int64
            DisplayName = get.Required.Field "display_name" Decode.string
        })

    let fromDomain (v: User): UserModel =
        {
            Id = Id.toInt64 v.Id
            DisplayName = v.DisplayName
        }
    
    let toDomain (v: UserModel): User =
        {
            Id = Id.create v.Id
            DisplayName = v.DisplayName
        }
