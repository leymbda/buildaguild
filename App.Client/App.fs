module App.Client.App

open Browser.Dom
open Feliz
open Feliz.Router

[<ReactComponent>]
let Router () =
    let currentUrl, updateUrl = React.useState (Router.currentPath())

    React.router [
        router.hashMode
        router.onUrlChanged updateUrl
        router.children [
            match currentUrl with
            | _ -> Html.div "Hello world"
        ]
    ]

ReactDOM
    .createRoot(document.getElementById "root")
    .render(Router())
