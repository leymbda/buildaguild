namespace Api.Presentation

open Domain
open Thoth.Json

type LoginResponse = {
    AccessToken: string
    User: UserResource option
}

module LoginResponse =
    let fromDomain (accessToken: string) (user: User option): LoginResponse =
        {
            AccessToken = accessToken
            User = Option.map UserResource.fromDomain user
        }

    let encoder (v: LoginResponse) =
        Encode.object [
            "access_token", Encode.string v.AccessToken
            "user", Encode.option UserResource.encoder v.User
        ]
