module Api.Infrastructure.Discord.DiscordApi

open Api.Application
open Api.Application.IDiscord
open Browser.Types
open Fable.Core
open Fetch

/// https://discord.com/developers/docs/topics/oauth2#authorization-code-grant-access-token-exchange-example
let oauthTokenExchange (di: #EnvDI & #FetcherDI): OAuthTokenExchange =
    fun code -> async {
        let body =
            FormData.create()
            |> FormData.set "grant_type" "authorization_code"
            |> FormData.set "code" code
            |> FormData.set "redirect_uri" di.Env.REDIRECT_URI
            |> FormData.set "client_id" di.Env.CLIENT_ID
            |> FormData.set "client_secret" di.Env.CLIENT_SECRET
        
        let! res = 
            di.Fetcher.Fetch
                $"http://discord.com/api/oauth2/token"
                [
                    Method HttpMethod.POST
                    requestHeaders [
                        ContentType "application/x-www-form-urlencoded"
                    ]
                    Body (U3.Case2 body)
                ]

        return! res |> Response.decode OAuthTokenExchangeResponse.decoder
    }

// TODO: Token revocation, get current user, etc
