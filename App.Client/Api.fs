module App.Client.Api

open Api.Presentation
open Domain
open Fable.Core
open Fetch
open FsToolkit.ErrorHandling

let getMetadata () =
    fetchUnsafe "/api/metadata" [Method HttpMethod.GET]
    |> Async.AwaitPromise
    |> Async.bind (Response.decode MetadataResource.decoder)
    |> Async.map (Result.defaultWith (fun _ -> failwith "Api error"))
    |> Async.StartAsPromise

let login (code: string) =
    fetchUnsafe $"/api/auth/login?code={code}" [Method HttpMethod.POST]
    |> Async.AwaitPromise
    |> Async.bind (Response.decode LoginResponse.decoder)
    |> Async.map (Result.defaultWith (fun _ -> failwith "Api error"))
    |> Async.StartAsPromise

let logout () =
    fetchUnsafe "/api/auth/logout" [Method HttpMethod.POST]
    |> Promise.map Response.success

let createUser (id: Id) =
    fetchUnsafe $"/api/users/{Id.toString id}" [Method HttpMethod.PUT]
    |> Async.AwaitPromise
    |> Async.bind (Response.decode UserResource.decoder)
    |> Async.map (Result.defaultWith (fun _ -> failwith "Api error"))
    |> Async.StartAsPromise

let getUserById (id: Id) =
    fetchUnsafe $"/api/users/{Id.toString id}" [Method HttpMethod.GET]
    |> Async.AwaitPromise
    |> Async.bind (Response.decode UserResource.decoder)
    |> Async.map (Result.defaultWith (fun _ -> failwith "Api error"))
    |> Async.StartAsPromise

let deleteUserById (id: Id) =
    fetchUnsafe $"/api/users/{Id.toString id}" [Method HttpMethod.DELETE]
    |> Promise.map Response.success

// TODO: Refactor into injectible service (IFetcher)
// TODO: Proper error handling
// TODO: Add cookie to requests that require auth
