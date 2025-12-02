module Api.Presentation.Api

open Api.Application
open Api.Infrastructure.UserDurableObject
open Browser
open Domain
open Fable.Bindings.CloudflareWorkers
open Fable.Core
open Fable.Core.JsInterop
open Fetch
open System

let TEMPORARY_REQUEST_USER_ID = Id.Id (Guid.NewGuid())

type DI(env) =
    interface EnvDI with
        member _.Env = env

    interface AuthServiceDI with
        member this.Auth = {
            GetSession = AuthService.getSession this
            Authenticate = AuthService.authenticate this
        }

    interface UserServiceDI with
        member this.Users = {
            CreateUser = UserDurableObjectProxy.createUser this
            GetUserById = UserDurableObjectProxy.getUserById this
            DeleteUserById = UserDurableObjectProxy.deleteUserById this
        }

    interface UserApplicationServiceDI with
        member this.UserApplicationService = {
            CreateUser = UserApplicationService.createUser this
            GetUser = UserApplicationService.getUser this
            DeleteUser = UserApplicationService.deleteUser this
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
            
            match req.method, parts with
            | "POST", ["api"; "oauth2"; "token"] -> return Response.notImplemented() // TODO: Access/refresh token exchange
            
            | "POST", ["api"; "oauth2"; "token"; "revoke"] -> return Response.notImplemented() // TODO: Token revocation
        
            | "GET", ["api"; "oauth2"; "@me"] -> return Response.notImplemented() // TODO: Fetch current auth info
        
            | _, "api" :: "users" :: rest -> return! UserResource.fetch di req rest

            | _ -> return Response.notFound ""
        }
        
        // TODO: Create shared project for common utilities like above (with shared deps like Fable.Browser.Url, Thoth.Json, etc)

exportDefault {|
    fetch = fun (req: Request) (env: Env) (ctx: ExecutionContext<unit>) ->
        let di = DI(env)
        Program.fetch di req ctx |> Async.StartAsPromise
|}

type UserDurableObject(env, props) = inherit Api.Infrastructure.UserDurableObject.UserDurableObject(env, props)
