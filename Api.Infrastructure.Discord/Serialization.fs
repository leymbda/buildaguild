namespace Api.Infrastructure.Discord

open Api.Application
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
