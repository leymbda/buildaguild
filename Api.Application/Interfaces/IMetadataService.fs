namespace Api.Application

module IMetadataService =
    type GetMetadata = unit -> Async<Metadata>
    
type IMetadataService = {
    GetMetadata: IMetadataService.GetMetadata
}

type MetadataServiceDI =
    abstract MetadataService: IMetadataService
