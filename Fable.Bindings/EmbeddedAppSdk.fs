module rec Fable.Bindings.EmbeddedAppSdk

open Browser.Types
open Fable.Core
open Fable.Core.JS

/// https://discord.com/developers/docs/developer-tools/embedded-app-sdk#rpcclosecodes
[<Import("RPCCloseCodes", "@discord/embedded-app-sdk")>]
type RpcCloseCode =
    | CLOSE_NORMAL = 1000
    | CLOSE_UNSUPPORTED = 1003
    | CLOSE_ABNORMAL = 1006
    | INVALID_CLIENTID = 4000
    | INVALID_ORIGIN = 4001
    | RATELIMITED = 4002
    | TOKEN_REVOKED = 4003
    | INVALID_VERSION = 4004
    | INVALID_ENCODING = 4005

[<Import("Platform", "@discord/embedded-app-sdk"); RequireQualifiedAccess; StringEnum>]
type Platform =
    | ``mobile``
    | ``desktop``
    
[<AllowNullLiteral; Import("IDiscordSDK", "@discord/embedded-app-sdk")>]
type IDiscordSdk =
    abstract clientId: string
    abstract instanceId: string
    abstract customId: string option
    abstract referrerId: string option
    abstract platform: Platform
    abstract mobileAppVersion: string option
    abstract sdkVersion: string
    abstract commands: DiscordSdkCommands
    abstract configuration: SdkConfiguration
    abstract channelId: string option
    abstract guildId: string option
    abstract source: Window option
    abstract sourceOrigin: string
    abstract close: code: RpcCloseCode * reason: string -> unit
    // TODO: subscribe
    // TODO: unsubscribe
    abstract ready: unit -> Promise<unit>

[<AllowNullLiteral; Import("DiscordSDK", "@discord/embedded-app-sdk")>]
type DiscordSdk =
    inherit IDiscordSdk
    abstract locationId: string option

[<CompiledName "DiscordSdkModule">]
module DiscordSdk =
    [<Import("DiscordSDK", "@discord/embedded-app-sdk"); Emit("new $0($1...)")>]
    let create (clientId: string) (configuration: SdkConfiguration option): DiscordSdk = jsNative
    
[<AllowNullLiteral; Erase>]
type SdkConfiguration =
    abstract disableConsoleLogOverride: bool
    
[<AllowNullLiteral; Erase>]
type DiscordSdkCommands =
    [<ParamObject>] abstract authenticate: access_token: string -> Promise<AuthenticateResponse>
    [<ParamObject>] abstract authorize: client_id: string * scope: string[] * ?code_challenge: string * ?state: string -> Promise<AuthorizeResponse>
    // TODO: Other commands
    
[<AllowNullLiteral; Import("DiscordSDKMock", "@discord/embedded-app-sdk")>]
type DiscordSdkMock =
    inherit IDiscordSdk
    abstract locationId: string option
    abstract _updateCommandMocks: DiscordSdkCommands -> DiscordSdkCommands
    abstract emitReady: unit -> unit
    // TODO: emitEvent
    
[<CompiledName "DiscordSdkMockModule">]
module DiscordSdkMock =
    [<Import("DiscordSDKMock", "@discord/embedded-app-sdk"); Emit("new $0($1...)")>]
    let create (clientId: string) (guildId: string option) (channelId: string option) (locationId: string option): DiscordSdkMock = jsNative
    // TODO: Above is returning undefined not null for options... how to fix?
    
    let updateCommandMocks (newMocks: DiscordSdkCommands) (sdk: DiscordSdkMock) =
        sdk._updateCommandMocks(newMocks) |> ignore
        sdk

/// https://discord.com/developers/docs/developer-tools/embedded-app-sdk#application
[<AllowNullLiteral; Erase>]
type Application =
    abstract description: string
    abstract icon: string option
    abstract id: string
    abstract rpc_origins: string[] option
    abstract name: string

/// https://discord.com/developers/docs/developer-tools/embedded-app-sdk#authenticateresponse
[<AllowNullLiteral; Erase>]
type AuthenticateResponse =
    abstract access_token: string
    abstract user: User
    abstract scopes: string array
    abstract expires: string
    abstract application: Application

/// https://discord.com/developers/docs/developer-tools/embedded-app-sdk#authorizeresponse
[<AllowNullLiteral; Erase>]
type AuthorizeResponse =
    abstract code: string

/// https://discord.com/developers/docs/developer-tools/embedded-app-sdk#avatardecorationdata
[<AllowNullLiteral; Erase>]
type AvatarDecorationData =
    abstract asset: string
    abstract sku_id: string option

/// https://discord.com/developers/docs/developer-tools/embedded-app-sdk#user
[<AllowNullLiteral; Erase>]
type User =
    abstract id: string
    abstract username: string
    abstract discriminator: string
    abstract global_name: string option
    abstract avatar: string option
    abstract avatar_decoration_data: AvatarDecorationData option
    abstract bot: bool
    abstract flags: int option
    abstract premium_type: int option
