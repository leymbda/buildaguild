module App.Client.Modules.Sdk

open Api.Presentation
open App.Client
open App.Client.Common
open Domain
open Elmish
open Fable.Bindings.EmbeddedAppSdk
open Fable.Core
open System

let loginBrowser (code: string) = promise {
    return!
        Api.login(code)
        |> Promise.map (
            _.User
            >> UserResource.toDomain
            >> Result.defaultWith failwith
        )
}

let loginActivity (sdk: DiscordSdk) = promise {
    do! sdk.ready()

    let! authorize = sdk.commands.authorize(sdk.clientId, [| "identify" |])

    let! login = Api.login(authorize.code)

    let! _ = sdk.commands.authenticate(login.AccessToken) // TOOD: Is this needed?

    return
        login.User
        |> UserResource.toDomain
        |> Result.defaultWith failwith
}

type StopReason =
    | BrowserOAuthFlowInitiate of clientId: string
    
type NotStartedModel = {
    Origin: Origin
    Code: string option
    ClientId: string option
}

type StartingModel = {
    Sdk: IDiscordSdk
}

type ActiveModel = {
    Sdk: IDiscordSdk
    User: Domain.User
}

type StoppedModel = {
    Reason: StopReason
}

type Model =
    | NotStarted of NotStartedModel
    | Starting of StartingModel
    | Active of ActiveModel
    | Stopped of StoppedModel
    
type Msg =
    | Setup
    | GetClientId of clientId: string
    | GetUser of user: Domain.User
    | UpdateUser of user: Domain.User
    | Ready
    | Stop of reason: StopReason
    | Terminate

let init (origin, code) =
    Model.NotStarted {
        Origin = origin
        Code = code
        ClientId = None
    },
    Cmd.ofMsg Msg.Setup

let update (msg: Msg) (model: Model) =
    match model, msg with
    | Model.NotStarted model, Msg.Setup ->
        Model.NotStarted model,
        Cmd.OfAsync.perform (Api.getMetadata >> Async.AwaitPromise) () (_.ClientId >> Msg.GetClientId)

    | Model.NotStarted model, Msg.GetClientId clientId ->
        match model with
        | { Origin = Origin.Browser; Code = None } ->
            Model.NotStarted model, Cmd.ofMsg (Msg.Stop (StopReason.BrowserOAuthFlowInitiate clientId))

        | { Origin = Origin.Browser; Code = Some code } ->
            let sdk = DiscordSdkMock.create clientId None None None

            Model.Starting { Sdk = sdk },
            Cmd.OfAsync.perform (loginBrowser >> Async.AwaitPromise) code Msg.GetUser

        | { Origin = Origin.Activity } ->
            let sdk = DiscordSdk.create clientId None

            Model.Starting { Sdk = sdk },
            Cmd.OfAsync.perform (loginActivity >> Async.AwaitPromise) sdk Msg.GetUser

    | Model.Starting model, Msg.GetUser user ->
        Model.Active { Sdk = model.Sdk; User = user }, Cmd.ofMsg Msg.Ready

    | Model.Active _, Msg.Ready ->
        model, Cmd.none

    | Model.Active model, Msg.UpdateUser user ->
        Model.Active { model with User = user }, Cmd.none // TODO: Call api to update user details

    | model, Msg.Stop reason ->
        match model with
        | Model.NotStarted _
        | Model.Stopped _ -> ()
        | Model.Starting { Sdk = sdk }
        | Model.Active { Sdk = sdk } ->
            match reason with
            | StopReason.BrowserOAuthFlowInitiate _ -> sdk.close(RpcCloseCode.CLOSE_NORMAL, "Initiating browser oauth flow")

        Model.Stopped { Reason = reason }, Cmd.ofMsg (Msg.Terminate)

    | Model.Stopped _, Msg.Terminate ->
        model, Cmd.none

    | _, _ ->
        failwith "Unexpected state for given message"

let subscribe (model: Model): Sub<Msg> =
    Sub.batch [
        match model with
        | Model.NotStarted _
        | Model.Stopped _ -> ()
        | Model.Starting { Sdk = sdk }
        | Model.Active { Sdk = sdk } ->
            [
                ["CURRENT_USER_UPDATE"],
                fun dispatch ->
                    let callback (data: obj) =
                        let user = unbox<User> data

                        let domainUser = {
                            Id = Id.fromString user.id |> Option.get
                            DisplayName = user.global_name |> Option.defaultValue user.username
                        }
                            
                        domainUser |> Msg.UpdateUser |> dispatch

                    sdk.subscribe("CURRENT_USER_UPDATE ", callback)

                    { new IDisposable with
                        member _.Dispose () = sdk.unsubscribe("CURRENT_USER_UPDATE", callback) }
            ]
    ]

// TODO: Handle refreshing when token expires
