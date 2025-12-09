namespace App.Client.Common

open Browser.Types

type Origin =
    | Activity
    | Browser
    
module Origin =
    let fromWindow (window: Window) =
        match window.top <> window.self with
        | true -> Origin.Activity
        | false -> Origin.Browser
