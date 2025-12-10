module App.Client.Modules.Sdk

open App.Client
open App.Client.Common
open Elmish
open Fable.Bindings.EmbeddedAppSdk
open Fable.Core

let loginBrowser (code: string) = promise {
    let! _ = Api.login(code)

    return { new User with
        member _.id = "1234"
        member _.username = "mock_username"
        member _.discriminator = "0"
        member _.global_name = None
        member _.avatar = None
        member _.avatar_decoration_data = None
        member _.bot = false
        member _.flags = None
        member _.premium_type = None }
        
    // TODO: Rewrite API to just use the discord user rather than a custom domain user type, then return here
}

let loginActivity (sdk: DiscordSdk) = promise {
    do! sdk.ready()

    let! authorize = sdk.commands.authorize(sdk.clientId, [| "identify" |])

    let! { AccessToken = accessToken } = Api.login(authorize.code)

    let! authenticate = sdk.commands.authenticate(accessToken) // TOOD: If api returns user, will this still be needed?

    return authenticate.user
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
    User: User
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
    | GetUser of user: User
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

    | _, Msg.Stop reason ->
        // TODO: Close sdk if present
        Model.Stopped { Reason = reason }, Cmd.ofMsg (Msg.Terminate)

    | Model.Stopped _, Msg.Terminate ->
        model, Cmd.none

    | _, _ ->
        failwith "Unexpected state for given message"

// TODO: Handle refreshing when token expires
// TODO: Subscribe to events
