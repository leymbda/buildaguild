namespace rec Api.Application

type ISessionCache = {
    PutSession: ISessionCache.PutSession
    GetSession: ISessionCache.GetSession
    DeleteSession: ISessionCache.DeleteSession
}

module ISessionCache =
    type PutSession = string -> Session -> Async<unit>

    type GetSession = string -> Async<Session option>

    type DeleteSession = string -> Async<unit>

type SessionCacheDI =
    abstract SessionCache: ISessionCache
