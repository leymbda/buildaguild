module Api.Application.MetadataService

open Api.Application.IMetadataService

let getMetadata (di: #EnvDI): GetMetadata =
    fun () -> async {
        return {
            ClientId = di.Env.CLIENT_ID
        }
    }
