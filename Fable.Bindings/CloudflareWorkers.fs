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

[<AllowNullLiteral>]
type DurableObjectId =
    abstract toString: unit -> string
    abstract equals: other: DurableObjectId -> bool
    abstract name: string option with get

[<CompiledName "DurableObjectIdModule">]
module DurableObjectId =
    let toString (id: DurableObjectId) = id.toString()
    let equals (id: DurableObjectId) other = id.equals other
    
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
    let get (txn: DurableObjectTransaction) (key: string) allowConcurrency noCache = txn.get(key, ?allowConcurrency=allowConcurrency, ?noCache=noCache)
    let getMany (txn: DurableObjectTransaction) (keys: string[]) allowConcurrency noCache = txn.get(keys, ?allowConcurrency=allowConcurrency, ?noCache=noCache)
    let list (txn: DurableObjectTransaction) start startAfter ``end`` prefix reverse limit allowConcurrency noCache = txn.list(?start=start, ?startAfter=startAfter, ?``end``=``end``, ?prefix=prefix, ?reverse=reverse, ?limit=limit, ?allowConcurrency=allowConcurrency, ?noCache=noCache)
    let put (txn: DurableObjectTransaction) (key: string) value allowConcurrency allowUnconfirmed noCache = txn.put(key, value, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let putMany (txn: DurableObjectTransaction) (entries: Dictionary<string, 'a>) allowConcurrency allowUnconfirmed noCache = txn.put(entries, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let delete (txn: DurableObjectTransaction) (key: string) allowConcurrency allowUnconfirmed noCache = txn.delete(key, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let deleteMany (txn: DurableObjectTransaction) (keys: string[]) allowConcurrency allowUnconfirmed noCache = txn.delete(keys, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let rollback (txn: DurableObjectTransaction) = txn.rollback()
    let getAlarm (txn: DurableObjectTransaction) allowConcurrency = txn.getAlarm(?allowConcurrency=allowConcurrency)
    let setAlarm (txn: DurableObjectTransaction) scheduledTime allowConcurrency allowUnconfirmed = txn.setAlarm(scheduledTime, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed)
    let deleteAlarm (txn: DurableObjectTransaction) allowConcurrency allowUnconfirmed = txn.deleteAlarm(?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed)

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
    let get (storage: DurableObjectStorage) (key: string) allowConcurrency noCache = storage.get(key, ?allowConcurrency=allowConcurrency, ?noCache=noCache)
    let getMany (storage: DurableObjectStorage) (keys: string[]) allowConcurrency noCache = storage.get(keys, ?allowConcurrency=allowConcurrency, ?noCache=noCache)
    let list (storage: DurableObjectStorage) start startAfter ``end`` prefix reverse limit allowConcurrency noCache = storage.list(?start=start, ?startAfter=startAfter, ?``end``=``end``, ?prefix=prefix, ?reverse=reverse, ?limit=limit, ?allowConcurrency=allowConcurrency, ?noCache=noCache)
    let put (storage: DurableObjectStorage) (key: string) value allowConcurrency allowUnconfirmed noCache = storage.put(key, value, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let putMany (storage: DurableObjectStorage) (entries: Dictionary<string, 'a>) allowConcurrency allowUnconfirmed noCache = storage.put(entries, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let delete (storage: DurableObjectStorage) (key: string) allowConcurrency allowUnconfirmed noCache = storage.delete(key, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let deleteMany (storage: DurableObjectStorage) (keys: string[]) allowConcurrency allowUnconfirmed noCache = storage.delete(keys, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let deleteAll (storage: DurableObjectStorage) allowConcurrency allowUnconfirmed noCache = storage.deleteAll(?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed, ?noCache=noCache)
    let transaction (storage: DurableObjectStorage) closure = storage.transaction closure
    let getAlarm (storage: DurableObjectStorage) allowConcurrency = storage.getAlarm(?allowConcurrency=allowConcurrency)
    let setAlarm (storage: DurableObjectStorage) scheduledTime allowConcurrency allowUnconfirmed = storage.setAlarm(scheduledTime, ?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed)
    let deleteAlarm (storage: DurableObjectStorage) allowConcurrency allowUnconfirmed = storage.deleteAlarm(?allowConcurrency=allowConcurrency, ?allowUnconfirmed=allowUnconfirmed)
    let sync (storage: DurableObjectStorage) = storage.sync()
    let transactionSync (storage: DurableObjectStorage) closure = storage.transactionSync closure
    let getCurrentBookmark (storage: DurableObjectStorage) = storage.getCurrentBookmark()
    let getBookmarkForTime (storage: DurableObjectStorage) timestamp = storage.getBookmarkForTime timestamp
    let onNextSessionRestoreBookmark (storage: DurableObjectStorage) bookmark = storage.onNextSessionRestoreBookmark bookmark

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
    let start (container: Container) entrypoint enableInternet env = container.start(entrypoint, enableInternet, ?env=env)
    let monitor (container: Container) = container.monitor()
    let destroy (container: Container) error = container.destroy error
    let signal (container: Container) signo = container.signal signo
    let getTcpPort (container: Container) port = container.getTcpPort port
    
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
    let waitUntil (state: DurableObjectState<'props>) promise = state.waitUntil promise
    let blockConcurrencyWhile (state: DurableObjectState<'props>) callback = state.blockConcurrencyWhile callback
    let acceptWebSocket (state: DurableObjectState<'props>) ws tags = state.acceptWebSocket(ws, ?tags=tags)
    let getWebSockets (state: DurableObjectState<'props>) tag = state.getWebSockets(?tag=tag)
    let setWebSocketAutoResponse (state: DurableObjectState<'props>) maybeReqResp = state.setWebSocketAutoResponse(?maybeReqResp=maybeReqResp)
    let getWebSocketAutoResponse (state: DurableObjectState<'props>) = state.getWebSocketAutoResponse()
    let getWebSocketAutoResponseTimestamp (state: DurableObjectState<'props>) ws = state.getWebSocketAutoResponseTimestamp ws
    let setHibernatableWebSocketEventTimeout (state: DurableObjectState<'props>) timeoutMs = state.setHibernatableWebSocketEventTimeout(?timeoutMs=timeoutMs)
    let getHibernatableWebSocketEventTimeout (state: DurableObjectState<'props>) = state.getHibernatableWebSocketEventTimeout()
    let getTags (state: DurableObjectState<'props>) ws = state.getTags ws
    let abort (state: DurableObjectState<'props>) reason = state.abort reason

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
    let fetch (obj: DurableObject) request = obj.fetch request
    let alarm (obj: DurableObject) isRetry retryCount = obj.alarm(isRetry, retryCount)
    let webSocketMessage (obj: DurableObject) ws message = obj.webSocketMessage(ws, message)
    let webSocketClose (obj: DurableObject) ws code reason wasClean = obj.webSocketClose(ws, code, reason, wasClean)
    let webSocketError (obj: DurableObject) ws error = obj.webSocketError(ws, error)
    
[<AllowNullLiteral>]
type ExecutionContext<'props> =
    abstract waitUntil: promise: Promise<Any> -> unit
    abstract passThroughOnException: unit -> unit
    abstract props: 'props with get
    
[<CompiledName "ExecutionContextModule">]
module ExecutionContext =
    let waitUntil (ctx: ExecutionContext<'props>) promise = ctx.waitUntil promise
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

// TODO: How to handle globals given these are all globally defined interfaces? [<Global>] + the .d.ts might be enough?
// TODO: Revert U2 types if possible/reasonable (currently changed to all expect promises)
