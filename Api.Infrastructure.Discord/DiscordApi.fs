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
            URLSearchParams.create()
            |> URLSearchParams.set "grant_type" "authorization_code"
            |> URLSearchParams.set "code" code
            |> URLSearchParams.set "redirect_uri" di.Env.REDIRECT_URI
            |> URLSearchParams.set "client_id" di.Env.CLIENT_ID
            |> URLSearchParams.set "client_secret" di.Env.CLIENT_SECRET
            |> URLSearchParams.set "scope" "identify"
            |> URLSearchParams.toString
        
        let! res = 
            di.Fetcher.Fetch
                $"https://discord.com/api/oauth2/token"
                [
                    Method HttpMethod.POST
                    requestHeaders [
                        ContentType "application/x-www-form-urlencoded"
                    ]
                    Body (U3.Case3 body)
                ]

        return! res |> Response.decode OAuthTokenExchangeResponse.decoder
    }

let getCurrentUser (di: #EnvDI & #FetcherDI): GetCurrentUser =
    fun accessToken -> async {
        let! res = 
            di.Fetcher.Fetch
                "https://discord.com/api/users/@me"
                [
                    Method HttpMethod.GET
                    requestHeaders [
                        Authorization $"Bearer {accessToken}"
                    ]
                ]
        return! res |> Response.decode UserResponse.decoder
    }

// TODO: Token revocation, etc
