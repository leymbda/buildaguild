module App.Client.App

open App.Client.Common
open App.Client.Modules
open Browser
open Elmish
open Feliz
open Feliz.UseElmish

type Model = {
    Sdk: Sdk.Model
}

type Msg =
    | Sdk of Sdk.Msg

let init (origin, code) =
    let sdkModel, sdkCmd = Sdk.init (origin, code)
    { Sdk = sdkModel }, Cmd.map Msg.Sdk sdkCmd

let update (msg: Msg) (model: Model) =
    match msg, model with
    | Sdk msg, model ->
        let sdkModel, sdkCmd = Sdk.update msg model.Sdk
        { model with Sdk = sdkModel }, Cmd.map Msg.Sdk sdkCmd

let subscribe (model: Model): Sub<Msg> =
    Sub.batch [
        Sdk.subscribe model.Sdk |> Sub.map "sdk" Msg.Sdk
    ]

let program () =
    Program.mkProgram init update (fun _ _ -> ())
    |> Program.withSubscription subscribe
    |> Program.withTermination
        (function
            | Sdk (Sdk.Msg.Terminate) -> true
            | _ -> false
        )
        (function
            | { Sdk = Sdk.Model.Stopped { Reason = Sdk.StopReason.BrowserOAuthFlowInitiate clientId } } ->
                let redirectUri = window.encodeURIComponent(window.location.href)
                window.location.href <- $"https://discord.com/oauth2/authorize?response_type=code&client_id={clientId}&scope=identify&redirect_uri={redirectUri}"

            | _ ->
                failwith "Unexpected termination"
        )

[<ReactComponent>]
let App () =
    let origin = Origin.fromWindow window

    let code = React.useMemo(
        (fun () -> URLSearchParams.Create(window.location.search).get("code")),
        [| |]
    )

    if code.IsSome then
        let currentUrl = URL.Create(window.location.href)
        currentUrl.searchParams.delete("code");
        history.replaceState(null, url = currentUrl.href);

    let model, _ = React.useElmish(program, arg = (origin, code))

    match model.Sdk with
    | Sdk.Model.Active model ->
        Html.div $"Hello {model.User.DisplayName}!"

    | _ -> Html.div []

ReactDOM
    .createRoot(document.getElementById "root")
    .render(App())
    