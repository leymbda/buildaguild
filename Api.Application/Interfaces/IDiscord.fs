namespace rec Api.Application

type IDiscord = {
    OAuthTokenExchange: IDiscord.OAuthTokenExchange
}

module IDiscord =
    type OAuthTokenExchange = string -> Async<Result<OAuthTokenExchangeResponse, DecoderError>>

type DiscordDI =
    abstract Discord: IDiscord

type OAuthTokenExchangeResponse = {
    AccessToken: string
    TokenType: string
    ExpiresIn: int
    RefreshToken: string
    Scope: string list
}
