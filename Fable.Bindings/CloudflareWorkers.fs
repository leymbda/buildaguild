module Fable.Bindings.CloudflareWorkers

open Browser.Types
open Fable.Core
open Fable.Core.JS
open Fetch
open System
open System.Collections.Generic

type Any = obj
type Unknown = obj

type SqlStorageValue = U4<ArrayBuffer, string, decimal, unit>

type SqlStorageCursor<'a when 'a :> Dictionary<string, SqlStorageValue>> =
    // TODO(^): Needs to be js Record type constraint
    abstract next: unit -> U2<{| ``done``: bool option; value: 'a |}, {| ``done``: bool; value: unit |}> // TODO: Test this union
    abstract toArray: unit -> 'a[]
    abstract one: unit -> 'a
    // TODO: raw<U extends SqlStorageValue[]>(): IterableIterator<U>;
    abstract columnNames: string[] with get, set
    abstract rowsRead: int with get
    abstract rowsWritten: int with get
    // TODO: [Symbol.iterator](): IterableIterator<T>; (???)

type SqlStorage =
    abstract exec<'a>: query: string * bindings: Any[] -> SqlStorageCursor<'a> when 'a :> Dictionary<string, SqlStorageValue>
    // TODO(^): Needs to be js Record output
    abstract databaseSize: int with get
    // TODO: Cursor and Statement types? Probably just TS stuff that isn't needed here, but will need to confirm

type SyncKvStorage =
    abstract get<'a>: key: string -> 'a option
    [<ParamObject>] abstract list<'a>: ?start: string * ?startAfter: string * ?``end``: string * ?prefix: string * ?reverse: bool * ?limit: int -> obj // TODO: Iterable?
    abstract put<'a>: key: string * value: 'a -> unit
    abstract delete: key: string -> bool

// TODO: Modules for SqlStorage and SyncKvStorage methods

[<AllowNullLiteral>]
type DurableObjectId =
    abstract toString: unit -> string
    abstract equals: other: DurableObjectId -> bool
    abstract name: string option with get

[<CompiledName "DurableObjectIdModule">]
module DurableObjectId =
    let toString (id: DurableObjectId) = id.toString()
    let equals other (id: DurableObjectId) = id.equals other
    
[<AllowNullLiteral>]
type DurableObjectTransaction =
    [<ParamObject 1>] abstract get: key: string * ?allowConcurrency: bool * ?noCache: bool -> Promise<'a option>
    [<ParamObject 1>] abstract get: keys: string[] * ?allowConcurrency: bool * ?noCache: bool -> Promise<Dictionary<string, 'a>>
    // TODO(^): Needs to be js map return
    [<ParamObject>] abstract list: ?start: string * ?startAfter: string * ?``end``: string * ?prefix: string * ?reverse: bool * ?limit: int * ?allowConcurrency: bool * ?noCache: bool -> Promise<Dictionary<string, 'a>>
    // TODO(^): Needs to be js map return
    [<ParamObject 2>] abstract put: key: string * value: 'a * ?allowConcurrency: bool * ?allowUnconfirmed: bool * ?noCache: bool -> Promise<unit>
    [<ParamObject 1>] abstract put: entries: Dictionary<string, 'a> * ?allowConcurrency: bool * ?allowUnconfirmed: bool * ?noCache: bool -> Promise<unit>
    // TODO(^): Needs to be js Record entries input
    [<ParamObject 1>] abstract delete: key: string * ?allowConcurrency: bool * ?allowUnconfirmed: bool * ?noCache: bool -> Promise<bool>
    [<ParamObject 1>] abstract delete: keys: string[] * ?allowConcurrency: bool * ?allowUnconfirmed: bool * ?noCache: bool -> Promise<int>
    abstract rollback: unit -> unit
    abstract getAlarm: ?allowConcurrency: bool -> Promise<int option>
    [<ParamObject 1>] abstract setAlarm: scheduledTime: U2<int, DateTime> * ?allowConcurrency: bool * ?allowUnconfirmed: bool -> Promise<unit>
    [<ParamObject>] abstract deleteAlarm: ?allowConcurrency: bool * ?allowUnconfirmed: bool -> Promise<unit>

[<CompiledName "DurableObjectTransactionModule">]
module DurableObjectTransaction =
    let get (key: string) allowConcurrency noCache (txn: DurableObjectTransaction) = txn.get(key, ?allowConcurrency=allowConcurrency, ?noCache=noCache)
    let getMany (keys: string[]) allowConcurrency noCache (txn: DurableObjectTransaction) = txn.get(keys, ?allowConcurrency=allowConcurrency, ?noCache=noCache)
    let list start startAfter ``end`` prefix reverse limit allowConcurrency noCache (txn: DurableObjectTransaction) = txn.list(?start=start, ?startAfter=startAfter, ?``end``=``end``, ?prefix=prefix, ?reverse=reverse, ?limit=limit, ?allowConcurrency=allowConcurrency, ?noCache=noCache)
    let put (key: string) value allowConcurrency allowUnconfirmed noCache (txn: DurableObjectTransaction) = txn.put(key, value, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let putMany (entries: Dictionary<string, 'a>) allowConcurrency allowUnconfirmed noCache (txn: DurableObjectTransaction) = txn.put(entries, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let delete (key: string) allowConcurrency allowUnconfirmed noCache (txn: DurableObjectTransaction) = txn.delete(key, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let deleteMany (keys: string[]) allowConcurrency allowUnconfirmed noCache (txn: DurableObjectTransaction) = txn.delete(keys, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let rollback (txn: DurableObjectTransaction) = txn.rollback()
    let getAlarm allowConcurrency (txn: DurableObjectTransaction) = txn.getAlarm(?allowConcurrency=allowConcurrency)
    let setAlarm scheduledTime allowConcurrency allowUnconfirmed (txn: DurableObjectTransaction) = txn.setAlarm(scheduledTime, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed)
    let deleteAlarm allowConcurrency allowUnconfirmed (txn: DurableObjectTransaction) = txn.deleteAlarm(?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed)

[<AllowNullLiteral>]
type DurableObjectStorage =
    [<ParamObject 1>] abstract get: key: string * ?allowConcurrency: bool * ?noCache: bool -> Promise<'a option>
    [<ParamObject 1>] abstract get: keys: string[] * ?allowConcurrency: bool * ?noCache: bool -> Promise<Dictionary<string, 'a>>
    // TODO(^): Needs to be js map return
    [<ParamObject>] abstract list: ?start: string * ?startAfter: string * ?``end``: string * ?prefix: string * ?reverse: bool * ?limit: int * ?allowConcurrency: bool * ?noCache: bool -> Promise<Dictionary<string, 'a>>
    // TODO(^): Needs to be js map return
    [<ParamObject 2>] abstract put: key: string * value: 'a * ?allowConcurrency: bool * ?allowUnconfirmed: bool * ?noCache: bool -> Promise<unit>
    [<ParamObject 1>] abstract put: entries: Dictionary<string, 'a> * ?allowConcurrency: bool * ?allowUnconfirmed: bool * ?noCache: bool -> Promise<unit>
    // TODO(^): Needs to be js Record entries input
    [<ParamObject 1>] abstract delete: key: string * ?allowConcurrency: bool * ?allowUnconfirmed: bool * ?noCache: bool -> Promise<bool>
    [<ParamObject 1>] abstract delete: keys: string[] * ?allowConcurrency: bool * ?allowUnconfirmed: bool * ?noCache: bool -> Promise<int>
    [<ParamObject>] abstract deleteAll: ?allowConcurrency: bool * ?allowUnconfirmed: bool * ?noCache: bool -> Promise<unit>
    abstract transaction: closure: (DurableObjectTransaction -> Promise<'a>) -> Promise<'a>
    // TODO(^): Try specify DurableObjectTransaction as `txn` arg name
    abstract getAlarm: ?allowConcurrency: bool -> Promise<int option>
    [<ParamObject 1>] abstract setAlarm: scheduledTime: U2<int, DateTime> * ?allowConcurrency: bool * ?allowUnconfirmed: bool -> Promise<unit>
    [<ParamObject>] abstract deleteAlarm: ?allowConcurrency: bool * ?allowUnconfirmed: bool -> Promise<unit>
    abstract sync: unit -> Promise<unit>
    abstract sql: SqlStorage with get
    abstract kv: SyncKvStorage with get
    abstract transactionSync: closure: (unit -> 'a) -> 'a
    abstract getCurrentBookmark: unit -> Promise<string>
    abstract getBookmarkForTime: timestamp: U2<int, DateTime> -> Promise<string>
    abstract onNextSessionRestoreBookmark: bookmark: string -> Promise<string>

[<CompiledName "DurableObjectStorageModule">]
module DurableObjectStorage =
    let get (key: string) allowConcurrency noCache (storage: DurableObjectStorage) = storage.get(key, ?allowConcurrency=allowConcurrency, ?noCache=noCache)
    let getMany (keys: string[]) allowConcurrency noCache (storage: DurableObjectStorage) = storage.get(keys, ?allowConcurrency=allowConcurrency, ?noCache=noCache)
    let list start startAfter ``end`` prefix reverse limit allowConcurrency noCache (storage: DurableObjectStorage) = storage.list(?start=start, ?startAfter=startAfter, ?``end``=``end``, ?prefix=prefix, ?reverse=reverse, ?limit=limit, ?allowConcurrency=allowConcurrency, ?noCache=noCache)
    let put (key: string) value allowConcurrency allowUnconfirmed noCache (storage: DurableObjectStorage) = storage.put(key, value, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let putMany (entries: Dictionary<string, 'a>) allowConcurrency allowUnconfirmed noCache (storage: DurableObjectStorage) = storage.put(entries, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let delete (key: string) allowConcurrency allowUnconfirmed noCache (storage: DurableObjectStorage) = storage.delete(key, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let deleteMany (keys: string[]) allowConcurrency allowUnconfirmed noCache (storage: DurableObjectStorage) = storage.delete(keys, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let deleteAll allowConcurrency allowUnconfirmed noCache (storage: DurableObjectStorage) = storage.deleteAll(?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let transaction closure (storage: DurableObjectStorage) = storage.transaction closure
    let getAlarm allowConcurrency (storage: DurableObjectStorage) = storage.getAlarm(?allowConcurrency=allowConcurrency)
    let setAlarm scheduledTime allowConcurrency allowUnconfirmed (storage: DurableObjectStorage) = storage.setAlarm(scheduledTime, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed)
    let deleteAlarm allowConcurrency allowUnconfirmed (storage: DurableObjectStorage) = storage.deleteAlarm(?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed)
    let sync (storage: DurableObjectStorage) = storage.sync()
    let transactionSync closure (storage: DurableObjectStorage) = storage.transactionSync closure
    let getCurrentBookmark (storage: DurableObjectStorage) = storage.getCurrentBookmark()
    let getBookmarkForTime timestamp (storage: DurableObjectStorage) = storage.getBookmarkForTime timestamp
    let onNextSessionRestoreBookmark bookmark (storage: DurableObjectStorage) = storage.onNextSessionRestoreBookmark bookmark

[<AllowNullLiteral>]
type Container =
    abstract running: bool with get
    [<ParamObject>] abstract start: entrypoint: string[] * enableInternet: bool * ?env: Dictionary<string, string> -> unit
    // TODO(^): Needs to be js Record env input
    abstract monitor: unit -> Promise<unit>
    abstract destroy: ?error: Any -> Promise<unit>
    abstract signal: signo: int -> unit
    abstract getTcpPort: port: int -> obj
    // TODO(^): Fetcher type and everything under (rpc type stuff look extremely painful)
    
[<CompiledName "ContainerModule">]
module Container =
    let start entrypoint enableInternet env (container: Container) = container.start(entrypoint, enableInternet, ?env=env)
    let monitor (container: Container) = container.monitor()
    let destroy error (container: Container) = container.destroy error
    let signal signo (container: Container) = container.signal signo
    let getTcpPort port (container: Container) = container.getTcpPort port
    
[<AllowNullLiteral>]
type WebSocketResponsePair(request, response) =
    member val request: string = request with get
    member val response: string = response with get
    
[<AllowNullLiteral>]
type DurableObjectState<'props> =
    abstract waitUntil: promise: Promise<Any> -> unit
    abstract props: 'props with get
    abstract id: DurableObjectId with get
    abstract storage: DurableObjectStorage with get
    abstract container: Container option with get, set
    abstract blockConcurrencyWhile: callback: (unit -> Promise<'a>) -> Promise<'a>
    abstract acceptWebSocket: ws: WebSocket * ?tags: string[] -> unit
    abstract getWebSockets: ?tag: string -> WebSocket[]
    abstract setWebSocketAutoResponse: ?maybeReqResp: WebSocketResponsePair -> unit
    abstract getWebSocketAutoResponse: unit -> WebSocketResponsePair option
    abstract getWebSocketAutoResponseTimestamp: ws: WebSocket -> DateTime option
    abstract setHibernatableWebSocketEventTimeout: ?timeoutMs: int -> unit
    abstract getHibernatableWebSocketEventTimeout: unit -> int option
    abstract getTags: ws: WebSocket -> string[]
    abstract abort: reason: string -> unit
    
[<CompiledName "DurableObjectStateModule">]
module DurableObjectState =
    let waitUntil promise (state: DurableObjectState<'props>) = state.waitUntil promise
    let blockConcurrencyWhile callback (state: DurableObjectState<'props>) = state.blockConcurrencyWhile callback
    let acceptWebSocket ws tags (state: DurableObjectState<'props>) = state.acceptWebSocket(ws, ?tags=tags)
    let getWebSockets tag (state: DurableObjectState<'props>) = state.getWebSockets(?tag=tag)
    let setWebSocketAutoResponse maybeReqResp (state: DurableObjectState<'props>) = state.setWebSocketAutoResponse(?maybeReqResp=maybeReqResp)
    let getWebSocketAutoResponse (state: DurableObjectState<'props>) = state.getWebSocketAutoResponse()
    let getWebSocketAutoResponseTimestamp ws (state: DurableObjectState<'props>) = state.getWebSocketAutoResponseTimestamp ws
    let setHibernatableWebSocketEventTimeout timeoutMs (state: DurableObjectState<'props>) = state.setHibernatableWebSocketEventTimeout(?timeoutMs=timeoutMs)
    let getHibernatableWebSocketEventTimeout (state: DurableObjectState<'props>) = state.getHibernatableWebSocketEventTimeout()
    let getTags ws (state: DurableObjectState<'props>) = state.getTags ws
    let abort reason (state: DurableObjectState<'props>) = state.abort reason

[<AllowNullLiteral>]
type DurableObject =
    abstract fetch: request: Request -> Promise<Response> // U2<Response, Promise<Response>>
    [<ParamObject>] abstract alarm: isRetry: bool * retryCount: int -> Promise<Response> // U2<unit, Promise<unit>>
    abstract webSocketMessage: ws: WebSocket * message: U2<string, ArrayBuffer> -> Promise<unit> // U2<unit, Promise<unit>>
    abstract webSocketClose: ws: WebSocket * code: int * reason: string * wasClean: bool -> Promise<unit> // U2<unit, Promise<unit>>
    abstract webSocketError: ws: WebSocket * error: Unknown -> Promise<unit> // U2<unit, Promise<unit>>
    
    // Creating delegates for alarm, webSocketMessage, webSocketClose, webSocketError allow them to be correctly
    // represented as optional abstract members, but it doesn't play nicely with `[<ParamObject>]`. This may not
    // matter since alarm also expects that these params be optional too (either both or neither provided) which
    // doesn't seem possible with this attribute. Leaving as-is for now to come back to later if necessary.
    
    // type DurableObjectWebSocketClose = delegate of
    //     ws: WebSocket *
    //     code: int *
    //     reason: string *
    //     wasClean: bool ->
    //     U2<unit, Promise<unit>>
    
    // abstract webSocketClose: DurableObjectWebSocketClose option
    
[<CompiledName "DurableObjectModule">]
module DurableObject =
    let fetch request (obj: DurableObject) = obj.fetch request
    let alarm isRetry retryCount (obj: DurableObject) = obj.alarm(isRetry, retryCount)
    let webSocketMessage ws message (obj: DurableObject) = obj.webSocketMessage(ws, message)
    let webSocketClose ws code reason wasClean (obj: DurableObject) = obj.webSocketClose(ws, code, reason, wasClean)
    let webSocketError ws error (obj: DurableObject) = obj.webSocketError(ws, error)
    
[<AllowNullLiteral>]
type ExecutionContext<'props> =
    abstract waitUntil: promise: Promise<Any> -> unit
    abstract passThroughOnException: unit -> unit
    abstract props: 'props with get
    
[<CompiledName "ExecutionContextModule">]
module ExecutionContext =
    let waitUntil promise (ctx: ExecutionContext<'props>) = ctx.waitUntil promise
    let passThroughOnException (ctx: ExecutionContext<'props>) = ctx.passThroughOnException()

type ExportedHandlerFetchHandler<'env> = Request -> 'env -> ExecutionContext<unit> -> Promise<Response> // U2<Response, Promise<Response>>
type ExportedHandlerTailHandler<'env> = obj -> 'env -> ExecutionContext<unit> -> Promise<unit> // U2<unit, Promise<unit>>
// TODO(^): Implement `obj` as `TraceItem[]`
type ExportedHandlerTraceHandler<'env> = obj -> 'env -> ExecutionContext<unit> -> Promise<unit> // U2<unit, Promise<unit>>
// TODO(^): Implement `obj` as `TraceItem[]`
type ExportedHandlerTailStreamHandler<'env> = obj -> 'env -> ExecutionContext<unit> -> Promise<unit> // U2<unit, Promise<unit>>
// TODO(^): Implement `obj` as `TailStream.TailEvent<TailStream.Onset>`
type ExportedHandlerScheduledHandler<'env> = obj -> 'env -> ExecutionContext<unit> -> Promise<unit> // U2<unit, Promise<unit>>
// TODO(^): Implement `obj` as `ScheduledController`
type ExportedHandlerTestHandler<'env> = obj -> 'env -> ExecutionContext<unit> -> Promise<unit> // U2<unit, Promise<unit>>
// TODO(^): Implement `obj` as `TestController`
type [<CompiledName "EmailExportedHandler">] ExportedHandlerEmailHandler<'env> = obj -> 'env -> ExecutionContext<unit> -> Promise<unit> // U2<unit, Promise<unit>>
// TODO(^): Implement `obj` as `ForwardableEmailMessage`
type ExportedHandlerQueueHandler<'env, 'message> = obj -> 'message -> 'env -> ExecutionContext<unit> -> Promise<unit> // U2<unit, Promise<unit>>
// TODO(^): Implement `obj -> 'message` as `MessageBatch<'message>`

[<AllowNullLiteral>]
type ExportedHandler<'env, 'message> =
    abstract fetch: ExportedHandlerFetchHandler<'env> option
    abstract tail: ExportedHandlerTailHandler<'env> option
    abstract trace: ExportedHandlerTraceHandler<'env> option
    abstract tailStream: ExportedHandlerTailStreamHandler<'env> option
    abstract scheduled: ExportedHandlerScheduledHandler<'env> option
    abstract test: ExportedHandlerTestHandler<'env> option
    abstract email: ExportedHandlerEmailHandler<'env> option
    abstract queue: ExportedHandlerQueueHandler<'env, 'message> option

[<AbstractClass; AllowNullLiteral; Import("DurableObject", "cloudflare:workers")>]
type DurableObjectBase<'env, 'props>(state: DurableObjectState<'props>, env: 'env) =
    member val ctx: DurableObjectState<'props> = jsNative with get
    member val env: 'env = jsNative with get

    interface DurableObject with
        member _.fetch(req) = jsNative
        [<ParamObject>] member _.alarm(isRetry, retryCount) = jsNative
        member _.webSocketMessage(ws, message) = jsNative
        member _.webSocketClose(ws, code, reason, wasClean) = jsNative
        member _.webSocketError(ws, error) = jsNative
        
[<AllowNullLiteral>]
type DurableObjectStub<'rpc> =
    abstract member id: DurableObjectId with get
    abstract member name: string option with get // TODO: Ensure correctly handles option

    // TODO: fetch(input: RequestInfo | URL, init?: RequestInit): Promise<Response>;
    // TODO: connect(address: SocketAddress | string, options?: SocketOptions): Socket;
    // TODO: Rpc.Provider<T, Reserved | "fetch" | "connect">
    
    // IDEA: Can this be generated with emitted JS?

    // NOTE: It probably makes sense to define interfaces separately in Api.Application/Bindings for these sorts of properties.
    //       Maybe it can extend this type? Need to figure out how the stub behaviour works to ensure it creates the correct
    //       promises. Probably needs to change so the `'rpc` is a type that inherits `DurableObjectStub` rather than using a
    //       generic.

[<StringEnum; RequireQualifiedAccess>]
type DurableObjectLocationHint =
    | ``wnam``
    | ``enam``
    | ``sam``
    | ``weur``
    | ``eeur``
    | ``apac``
    | ``oc``
    | ``afr``
    | ``me``

[<StringEnum; RequireQualifiedAccess>]
type DurableObjectJurisdiction =
    | ``eu``
    | ``fedramp``
    | ``fedramp-high``

// type DurableObjectNamespace<'rpc> =
//     [<ParamObject>] abstract newUniqueId: jurisdiction: string option -> DurableObjectId
//     abstract idFromName: name: string -> DurableObjectId
//     abstract idFromString: id: string -> DurableObjectId
//     [<ParamObject 1>] abstract get: id: DurableObjectId * ?locationHint: DurableObjectLocationHint -> DurableObjectStub<'rpc>
//     [<ParamObject>] abstract getByName: name: string * ?locationHint: DurableObjectLocationHint -> DurableObjectStub<'rpc>
//     abstract jurisdiction: jurisdiction: DurableObjectJurisdiction -> DurableObjectNamespace<'rpc>

// [<CompiledName "DurableObjectNamespaceModule">]
// module DurableObjectNamespace =
//     let idFromName (dons: DurableObjectNamespace<'rpc>) name = dons.idFromName name
//     let idFromString (dons: DurableObjectNamespace<'rpc>) id = dons.idFromString id
//     let get (dons: DurableObjectNamespace<'rpc>) (id: DurableObjectId) locationHint = dons.get(id, ?locationHint=locationHint)
//     let getByName (dons: DurableObjectNamespace<'rpc>) name locationHint = dons.getByName(name, ?locationHint=locationHint)
//     let jurisdiction (dons: DurableObjectNamespace<'rpc>) jurisdiction = dons.jurisdiction jurisdiction

// TODO: Above types match the actual JS API but I think it makes sense to avoid stubs entirely and make independent
//       requests to the DO.

type DurableObjectNamespace =
     [<ParamObject>] abstract newUniqueId: jurisdiction: string option -> DurableObjectId
     abstract idFromName: name: string -> DurableObjectId
     abstract idFromString: id: string -> DurableObjectId
     [<ParamObject 1>] abstract get: id: DurableObjectId * ?locationHint: DurableObjectLocationHint -> DurableObject
     [<ParamObject>] abstract getByName: name: string * ?locationHint: DurableObjectLocationHint -> DurableObject
     abstract jurisdiction: jurisdiction: DurableObjectJurisdiction -> DurableObjectNamespace
    
[<CompiledName "DurableObjectNamespaceModule">]
module DurableObjectNamespace =
    let idFromName (dons: DurableObjectNamespace) name = dons.idFromName name
    let idFromString (dons: DurableObjectNamespace) id = dons.idFromString id
    let get (dons: DurableObjectNamespace) (id: DurableObjectId) locationHint = dons.get(id, ?locationHint=locationHint)
    let getByName (dons: DurableObjectNamespace) name locationHint = dons.getByName(name, ?locationHint=locationHint)
    let jurisdiction (dons: DurableObjectNamespace) jurisdiction = dons.jurisdiction jurisdiction
    
[<AllowNullLiteral>]
type KVNamespaceListKey<'metadata> =
    abstract name: string
    abstract expiration: int option
    abstract metadata: 'metadata option
    
[<AllowNullLiteral>]
type KVNamespaceListResult<'metadata> =
    abstract list_complete: bool
    abstract keys: KVNamespaceListKey<'metadata>[]
    abstract cursor: string
    abstract cacheStatus: string option
    
[<AllowNullLiteral>]
type KVNamespaceGetWithMetadataResult<'metadata> =
    abstract value: string option
    abstract metadata: 'metadata option
    abstract cacheStatus: string option
    
[<AllowNullLiteral>]
type KVNamespace =
    [<ParamObject 1>] abstract get: key: string * ?cacheTtl: int -> Promise<string option>
    [<ParamObject 1>] abstract get: key: string[] * ?cacheTtl: int -> Promise<Dictionary<string, string option>>
    [<ParamObject>] abstract list<'metadata>: ?limit: int * ?prefix: string option * ?cursor: string option -> Promise<KVNamespaceListResult<'metadata>>
    [<ParamObject 2>] abstract put: key: string * value: string * ?expiration: int * ?expirationTtl: int * ?metadata: Any option -> Promise<unit>
    [<ParamObject 1>] abstract getWithMetadata<'metadata>: key: string * ?cacheTtl: int -> Promise<KVNamespaceGetWithMetadataResult<'metadata>>
    abstract delete: key: string -> Promise<unit>
    
    // TODO: All variations of return types and whatnot (just doing simple string currently)

[<CompiledName "KVNamespaceModule">]
module KVNamespace =
    let get (key: string) cacheTtl (kv: KVNamespace) = kv.get(key, ?cacheTtl=cacheTtl)
    let getMany (key: string[]) cacheTtl (kv: KVNamespace) = kv.get(key, ?cacheTtl=cacheTtl)
    let list<'metadata> limit prefix cursor (kv: KVNamespace) = kv.list<'metadata>(?limit=limit, ?prefix=prefix, ?cursor=cursor)
    let put key value expiration expirationTtl metadata (kv: KVNamespace) = kv.put(key, value, ?expiration=expiration, ?expirationTtl=expirationTtl, ?metadata=metadata)
    let getWithMetadata<'metadata> key cacheTtl (kv: KVNamespace) = kv.getWithMetadata<'metadata>(key, ?cacheTtl=cacheTtl)
    let delete key (kv: KVNamespace) = kv.delete key
    
[<AllowNullLiteral>]
type D1MetaTimings =
    abstract sql_duration_ms: int
    
[<AllowNullLiteral>]
type D1Meta =
    abstract duration: int
    abstract size_after: int
    abstract rows_read: int
    abstract rows_written: int
    abstract last_row_id: int
    abstract changed_db: bool
    abstract changes: int
    abstract served_by_region: string option
    abstract served_by_primary: bool option
    abstract timings: D1MetaTimings option
    abstract total_attempts: int option
    
[<AllowNullLiteral>]
type D1Result<'a> =
    abstract meta: D1Meta
    abstract results: 'a[]
    
    // NOTE: D1Response is only seemingly used by this so I have merged it and D1Result
    // NOTE: success and error properties appear to expect to never actually be a failure so haven't defined
    
[<AllowNullLiteral>]
type D1ExecResult =
    abstract count: int
    abstract duration: int
    
[<AllowNullLiteral>]
type D1PreparedStatement =
    [<Emit "$0.bind(...$1)">] abstract bind: values: Unknown[] -> D1PreparedStatement
    abstract first<'a>: colName: string -> Promise<'a option>
    abstract first<'a>: unit -> Promise<'a option>
    abstract run<'a>: unit -> Promise<D1Result<'a>>
    abstract all<'a>: unit -> Promise<D1Result<'a>>
    [<Emit "$0.raw({ columnNames: true })">] abstract rawWithCols<'a>: unit -> Promise<U2<string, 'a>[]>
    abstract raw<'a>: unit -> Promise<'a[]>

[<CompiledName "D1PreparedStatementModule">]
module D1PreparedStatement =
    let bind values (statement: D1PreparedStatement) = statement.bind values
    let firstOfCol<'a> colName (statement: D1PreparedStatement) = statement.first<'a> colName
    let first<'a> (statement: D1PreparedStatement) = statement.first<'a> ()
    let run<'a> (statement: D1PreparedStatement) = statement.run<'a>()
    let all<'a> (statement: D1PreparedStatement) = statement.all<'a>()
    let rawWithCols<'a> (statement: D1PreparedStatement) = statement.rawWithCols<'a>()
    let raw<'a> (statement: D1PreparedStatement) = statement.raw<'a>()

[<RequireQualifiedAccess; StringEnum>]
type D1SessionConstraint =
    | ``first-primary``
    | ``first-unconstrained``
    
[<AllowNullLiteral>]
type D1DatabaseSession =
    abstract prepare: query: string -> D1PreparedStatement
    abstract batch<'a>: statements: D1PreparedStatement[] -> Promise<D1Result<'a>[]>
    abstract getBookmark: unit -> string option

[<CompiledName "D1DatabaseSessionModule">]
module D1DatabaseSession =
    let prepare query (session: D1DatabaseSession) = session.prepare query
    let batch<'a> statements (session: D1DatabaseSession) = session.batch<'a> statements
    let getBookmark (session: D1DatabaseSession) = session.getBookmark()
    
[<AllowNullLiteral>]
type D1Database =
    abstract prepare: query: string -> D1PreparedStatement
    abstract batch<'a>: statements: D1PreparedStatement[] -> Promise<D1Result<'a>[]>
    abstract exec: query: string -> Promise<D1ExecResult>
    abstract withSession: ?constraintOrBookmark: U2<string, D1SessionConstraint> -> D1DatabaseSession

[<CompiledName "D1DatabaseModule">]
module D1Database =
    let prepare query (db: D1Database) = db.prepare query
    let batch<'a> statements (db: D1Database) = db.batch<'a> statements
    let exec query (db: D1Database) = db.exec query
    let withSession constraintOrBookmark (db: D1Database) = db.withSession(?constraintOrBookmark=constraintOrBookmark)

// TODO: KV and D1 "number" types are all set to int but may need to be something else
// TODO: How to handle globals given these are all globally defined interfaces? [<Global>] + the .d.ts might be enough?
// TODO: Revert U2 types if possible/reasonable (currently changed to all expect promises)
