module Api.Application.Fetcher

open Api.Application.IFetcher
open Fable.Core

let fetch: Fetch =
    fun url init ->
        Fetch.fetchUnsafe url init
        |> Async.AwaitPromise
