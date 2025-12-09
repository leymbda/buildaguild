module Api.Presentation.MetadataController

open Api.Application
open Browser
open Fetch
open FsToolkit.ErrorHandling

let fetch (di: #MetadataServiceDI) (req: Request) (parts: string list) =
    asyncResult {
        let url = URL.Create(req.url)

        match req.method, parts with
        | "GET", [] ->
            let! metadata = di.MetadataService.GetMetadata()
            return Response.ok (MetadataResource.fromDomain metadata) MetadataResource.encoder

        | _ ->
            return Response.notFound ""
    }
    |> AsyncResult.defaultWith id
