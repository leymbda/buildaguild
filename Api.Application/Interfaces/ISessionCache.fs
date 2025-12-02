namespace rec Api.Application

type ISessionCache = {
    PutSession: ISessionCache.PutSession
    GetSession: ISessionCache.GetSession
    DeleteSession: ISessionCache.DeleteSession
}

module ISessionCache =
    type PutSessionError =
        | CacheError of string
        
    type PutSession = string -> Session -> Async<Result<Session, PutSessionError>>

    type GetSessionError =
        | SessionNotFound of string
        | CacheError of string

    type GetSession = string -> Async<Result<Session, GetSessionError>>

    type DeleteSessionError =
        | SessionNotFound of string
        | CacheError of string

    type DeleteSession = string -> Async<Result<unit, DeleteSessionError>>

type SessionCacheDI =
    abstract SessionCache: ISessionCache
