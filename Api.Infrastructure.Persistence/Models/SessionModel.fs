namespace Api.Infrastructure.Persistence

open Api.Application
open Domain
open FsToolkit.ErrorHandling
open System
open Thoth.Json

type SessionModel = {
    UserId: string
    AccessToken: string
    RefreshToken: string
    ExpiresAt: DateTime
}

module SessionModel =
    let encoder (v: SessionModel) =
        Encode.object [
            "user_id", Encode.string v.UserId
            "access_token", Encode.string v.AccessToken
            "refresh_token", Encode.string v.RefreshToken
            "expires_at", Encode.datetime v.ExpiresAt
        ]

    let decoder: Decoder<SessionModel> =
        Decode.object (fun get -> {
            UserId = get.Required.Field "user_id" Decode.string
            AccessToken = get.Required.Field "access_token" Decode.string
            RefreshToken = get.Required.Field "refresh_token" Decode.string
            ExpiresAt = get.Required.Field "expires_at" Decode.datetimeUtc
        })

    let fromDomain (v: Session): SessionModel =
        {
            UserId = Id.toString v.UserId
            AccessToken = v.AccessToken
            RefreshToken = v.RefreshToken
            ExpiresAt = v.ExpiresAt
        }
    
    let toDomain (v: SessionModel): Result<Session, string> =
        result {
            let! userId = 
                v.UserId 
                |> Id.fromString 
                |> Result.requireSome "Invalid 'user_id' in SessionModel"

            return {
                UserId = userId
                AccessToken = v.AccessToken
                RefreshToken = v.RefreshToken
                ExpiresAt = v.ExpiresAt
            }
        }
