namespace Api.Presentation

open Domain
open Thoth.Json

type LoginResponse = {
    AccessToken: string
    User: UserResource
}

module LoginResponse =
    let fromDomain (accessToken: string) (user: User): LoginResponse =
        {
            AccessToken = accessToken
            User = UserResource.fromDomain user
        }

    let encoder (v: LoginResponse) =
        Encode.object [
            "access_token", Encode.string v.AccessToken
            "user", UserResource.encoder v.User
        ]

    let decoder: Decoder<LoginResponse> =
        Decode.object (fun get -> {
            AccessToken = get.Required.Field "access_token" Decode.string
            User = get.Required.Field "user" UserResource.decoder
        })
