module App.Client.App

open Browser
open Elmish
open Fable.Bindings.EmbeddedAppSdk
open Fable.Core
open Feliz
open Feliz.UseElmish

type Origin =
    | Activity
    | Browser

type Model = {
    Origin: Origin
    ClientId: string option
    Sdk: IDiscordSdk option
    User: User option
}

type Msg =
    | Connect
    | Ready of ClientId: string * Sdk: IDiscordSdk * User: User
    | Error of exn

let init origin =
    {
        Origin = origin
        ClientId = None
        Sdk = None
        User = None
    },
    Cmd.ofMsg Connect

let private connect (model: Model) = promise {
    let clientId = "" // TODO: Get client ID from config request to API

    let sdk: IDiscordSdk =
        match model.Origin with
        | Origin.Activity -> DiscordSdk.create clientId None
        | Origin.Browser -> DiscordSdkMock.create clientId None None None

    do! sdk.ready()

    let! authorize = sdk.commands.authorize(clientId, [| "identify" |])

    let accessToken = "" // TODO: Get access token from API (using authorize.code)
    let sessionToken = "" // TODO: Get session token from API

    let! authenticate = sdk.commands.authenticate(accessToken)

    return {| ClientId = clientId; Sdk = sdk; User = authenticate.user |}
}

let update (msg: Msg) (model: Model) =
    match msg, model with
    | Connect, _ ->
        model,
        Cmd.OfAsync.either
            (connect >> Async.AwaitPromise)
            model
            (fun res -> Ready (res.ClientId, res.Sdk, res.User))
            Error

    | Ready (clientId, sdk, user), _ ->
        { model with ClientId = Some clientId; Sdk = Some sdk; User = Some user }, Cmd.none

    | (Error exn), { Sdk = Some sdk } ->
        sdk.close(RpcCloseCode.CLOSE_ABNORMAL, exn.Message)
        raise exn

    | (Error exn), { Sdk = None } ->
        raise exn

[<ReactComponent>]
let App () =
    let origin = Origin.Browser // TODO: Figure out how to check

    let model, _ = React.useElmish(init, update, origin)

    match model.User, model.Sdk with
    | Some user, Some sdk ->
        console.log(sdk.guildId)
        Html.div $"Hello, {user.global_name |> Option.defaultValue user.username}!"

    | _, _ ->
        Html.div "Connecting to Discord SDK..."

ReactDOM
    .createRoot(document.getElementById "root")
    .render(App())
    
// TODO: Removed vite build from ts build because it still tries to use App.fs.js... How to make TS build work? tsc?
