[<AutoOpen>]
module Extensions

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fetch
open FsToolkit.ErrorHandling
open Thoth.Json

module Async =
    let bind f v = async {
        let! v' = v
        return! f v'
    }

    let map f v = async {
        let! v' = v
        return f v'
    }

type Fetch.Types.Request with
    static member create(url: string, init: RequestProperties list): Request =
        emitJsExpr (url, requestProps init) "new Request($0, $1)"

module Response =
    let decode (decoder: Decoder<'a>) (res: Response) =
        asyncResult {
            let! json = res.text() |> Async.AwaitPromise

            match res.Status with
            | v when v >= 200 && v < 300 && v <> 204 -> return! json |> Decode.fromString decoder
            | status -> return! Error $"Request failed with status {status}"
        }

    let success (res: Response) =
        res.Status >= 200 && res.Status < 300

    // TODO: DU error for catching different scenarios and returning informative errors from proxy e.g. 500

    let json<'a> (body: 'a) (encoder: Encoder<'a>) status =
        Response.create(
            Encode.toString 4 (encoder body),
            [
                Status status
                Headers [|
                    "content-type", "application/json"
                |]
            ]
        )

    let ok<'a> (body: 'a) (encoder: Encoder<'a>) =
        json body encoder 200

    let created<'a> (body: 'a) (encoder: Encoder<'a>) (link: string) =
        let res = json body encoder 201
        res.Headers.set("link", link)
        res

    let noContent () =
        Response.create(
            "",
            [
                Status 204
            ]
        )

    let badRequest (message: string) =
        Response.create(
            message,
            [
                Status 400
            ]
        )

    let unauthorized () =
        Response.create(
            "",
            [
                Status 401
            ]
        )

    let forbidden () =
        Response.create(
            "",
            [
                Status 403
            ]
        )

    let notFound (message: string) =
        Response.create(
            message,
            [
                Status 404
            ]
        )

    let conflict (message: string) =
        Response.create(
            message,
            [
                Status 409
            ]
        )

    let notImplemented () =
        Response.create(
            "",
            [
                Status 501
            ]
        )

module Url =
    let create (url: string) =
        Browser.Url.URL.Create(url)

    let withOptionalQuery (key: string) (value: string option) (url: URL) =
        match value with
        | None -> url
        | Some v ->
            url.searchParams.append(key, v)
            url

    let withRequiredQuery (key: string) (value: string) (url: URL) =
        url.searchParams.append(key, value)
        url

    let toRequest (method: HttpMethod) (url: URL) =
        Request.create(url.toString(), [Method method])
