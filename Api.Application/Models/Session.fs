namespace Api.Application

open Domain
open System

type Session = {
    UserId: Id
    AccessToken: string
    RefreshToken: string
    ExpiresAt: DateTime
}
