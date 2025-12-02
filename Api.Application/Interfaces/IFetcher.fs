namespace rec Api.Application

open Fetch

type IFetcher = {
    Fetch: IFetcher.Fetch
}

module IFetcher =
    type Fetch = string -> RequestProperties list -> Async<Response>

type FetcherDI =
    abstract Fetcher: IFetcher
