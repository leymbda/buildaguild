namespace Api.Infrastructure.Persistence

open Domain
open FsToolkit.ErrorHandling
open Thoth.Json

type UserModel = {
    Id: string
}

module UserModel =
    let encoder (v: UserModel) =
        Encode.object [
            "id", Encode.string v.Id
        ]

    let decoder: Decoder<UserModel> =
        Decode.object (fun get ->{
            Id = get.Required.Field "id" Decode.string
        })

    let fromDomain (v: User): UserModel =
        {
            Id = Id.toString v.Id
        }
    
    let toDomain (v: UserModel): Result<User, string> =
        result {
            let! id = 
                v.Id 
                |> Id.fromString 
                |> Result.requireSome "Invalid 'id' in UserModel"

            return {
                Id = id
            }
        }
