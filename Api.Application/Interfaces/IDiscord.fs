namespace rec Api.Application

type IDiscord = {
    OAuthTokenExchange: IDiscord.OAuthTokenExchange
    GetCurrentUser: IDiscord.GetCurrentUser
}

module IDiscord =
    type OAuthTokenExchange = string -> Async<Result<OAuthTokenExchangeResponse, DecoderError>>

    type GetCurrentUser = string -> Async<Result<UserResponse, DecoderError>>

type DiscordDI =
    abstract Discord: IDiscord

type OAuthTokenExchangeResponse = {
    AccessToken: string
    TokenType: string
    ExpiresIn: int
    RefreshToken: string
    Scope: string list
}

type UserResponse = {
    Id: int64
}
