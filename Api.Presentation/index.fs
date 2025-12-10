module Api.Presentation.Api

open Api.Application
open Api.Infrastructure.Discord
open Api.Infrastructure.Persistence
open Browser
open Fable.Bindings.CloudflareWorkers
open Fable.Core
open Fable.Core.JsInterop
open Fetch
open System

type DI(env) =
    interface EnvDI with
        member _.Env = env

    interface FetcherDI with
        member _.Fetcher = {
            Fetch = Fetcher.fetch
        }

    interface TimeDI with
        member _.Time = {
            GetCurrentTime = Time.getCurrentTime
            GetCurrentUnixTime = Time.getCurrentUnixTime
        }

    interface DiscordDI with
        member this.Discord = {
            OAuthTokenExchange = DiscordApi.oauthTokenExchange this
            GetCurrentUser = DiscordApi.getCurrentUser this
        }

    interface UserRepositoryDI with
        member this.UserRepository = {
            UpsertUser = D1.upsertUser this
            GetUserById = D1.getUserById this
            DeleteUserById = D1.deleteUserById this
        }

    interface SessionCacheDI with
        member this.SessionCache = {
            PutSession = KV.putSession this
            GetSession = KV.getSession this
            DeleteSession = KV.deleteSession this
        }

    interface MetadataServiceDI with
        member this.MetadataService = {
            GetMetadata = MetadataService.getMetadata this
        }

    interface AuthServiceDI with
        member this.AuthService = {
            Login = AuthService.login this
            Logout = AuthService.logout this
        }

    interface UserServiceDI with
        member this.UserService = {
            GetUserById = UserService.getUserById this
            DeleteUserById = UserService.deleteUserById this
        }
    
module Program =
    let fetch (di: #EnvDI) (req: Request) (_ctx: ExecutionContext<unit>) =
        async {
            let parts =
                URL.Create(req.url).pathname
                |> _.Split([| '/' |], StringSplitOptions.RemoveEmptyEntries)
                |> Array.toList
                |> List.map (fun s -> s.Trim('/'))
                |> List.filter (fun s -> s <> "")

            // TODO: Move above into common utility
            
            match req.method, parts with
            | _, "api" :: "metadata" :: rest -> return! MetadataController.fetch di req rest
            | _, "api" :: "auth" :: rest -> return! AuthController.fetch di req rest
            | _, "api" :: "users" :: rest -> return! UserController.fetch di req rest
            | _ -> return Response.notFound ""
        }

exportDefault {|
    fetch = fun (req: Request) (env: Env) (ctx: ExecutionContext<unit>) ->
        let di = DI(env)
        Program.fetch di req ctx |> Async.StartAsPromise
|}
