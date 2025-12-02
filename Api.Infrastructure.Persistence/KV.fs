module Api.Infrastructure.Persistence.KV

open Api.Application
open Api.Application.ISessionCache
open FsToolkit.ErrorHandling

let putSession (di: #EnvDI): PutSession =
    fun token session -> asyncResult {
        return! Error (PutSessionError.CacheError "Not implemented")
    }

let getSession (di: #EnvDI): GetSession =
    fun token -> asyncResult {
        return! Error (GetSessionError.CacheError "Not implemented")
    }

let deleteSession (di: #EnvDI): DeleteSession =
    fun token -> asyncResult {
        return! Error (DeleteSessionError.CacheError "Not implemented")
    }
