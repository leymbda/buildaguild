namespace Api.Application

open Domain
open System

type Session = {
    UserId: Id
    ExpiresAt: DateTime
}
