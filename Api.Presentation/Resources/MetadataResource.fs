namespace Api.Presentation

open Api.Application
open Thoth.Json

type MetadataResource = {
    ClientId: string
}

module MetadataResource =
    let fromDomain (v: Metadata): MetadataResource =
        {
            ClientId = v.ClientId
        }

    let encoder (v: MetadataResource) =
        Encode.object [
            "client_id", Encode.string v.ClientId
        ]
