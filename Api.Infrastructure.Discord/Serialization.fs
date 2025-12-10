namespace Api.Infrastructure.Discord

open Api.Application
open System
open Thoth.Json

module OAuthTokenExchangeResponse =
    let decoder: Decoder<OAuthTokenExchangeResponse> =
        Decode.object (fun get -> {
            AccessToken = get.Required.Field "access_token" Decode.string
            TokenType = get.Required.Field "token_type" Decode.string
            ExpiresIn = get.Required.Field "expires_in" Decode.int
            RefreshToken = get.Required.Field "refresh_token" Decode.string
            Scope = get.Required.Field "scope" Decode.string |> _.Split(' ') |> Array.toList
        })

module UserResponse =
    let decoder: Decoder<UserResponse> =
        Decode.object (fun get -> {
            Id = get.Required.Field "id" (Decode.string |> Decode.map Int64.Parse)
            GlobalName = get.Required.Field "global_name" (Decode.option Decode.string)
            Username = get.Required.Field "username" Decode.string
        })
