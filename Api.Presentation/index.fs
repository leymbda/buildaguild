module Api.Presentation.Api

open Api.Application
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

    interface UserRepositoryDI with
        member this.UserRepository = {
            CreateUser = D1.createUser this
            GetUserById = D1.getUserById this
            DeleteUserById = D1.deleteUserById this
        }

    interface SessionCacheDI with
        member this.SessionCache = {
            PutSession = KV.putSession this
            GetSession = KV.getSession this
            DeleteSession = KV.deleteSession this
        }

    interface AuthServiceDI with
        member this.AuthService = {
            Authenticate = AuthService.authenticate this
            GetSession = AuthService.getSession this
        }

    interface UserServiceDI with
        member this.UserService = {
            CreateUser = UserService.createUser this
            GetUserById = UserService.getUserById this
            DeleteUserById = UserService.deleteUserById this
        }
    
module Program =
    let fetch (di: DI) (req: Request) (_ctx: ExecutionContext<unit>) =
        async {
            let parts =
                URL.Create(req.url).pathname
                |> _.Split([| '/' |], StringSplitOptions.RemoveEmptyEntries)
                |> Array.toList
                |> List.map (fun s -> s.Trim('/'))
                |> List.filter (fun s -> s <> "")

            // TODO: Move above into common utility
            
            match req.method, parts with
            | "POST", ["api"; "oauth2"; "token"] -> return Response.notImplemented() // TODO: Access/refresh token exchange
            
            | "POST", ["api"; "oauth2"; "token"; "revoke"] -> return Response.notImplemented() // TODO: Token revocation
        
            | "GET", ["api"; "oauth2"; "@me"] -> return Response.notImplemented() // TODO: Fetch current auth info
        
            | _, "api" :: "users" :: rest -> return! UserResource.fetch di req rest

            | _ -> return Response.notFound ""
        }

exportDefault {|
    fetch = fun (req: Request) (env: Env) (ctx: ExecutionContext<unit>) ->
        let di = DI(env)
        Program.fetch di req ctx |> Async.StartAsPromise
|}
