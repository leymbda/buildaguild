module Api.Application.Time

open Api.Application.ITime
open System

let getCurrentTime: GetCurrentTime =
    fun () ->
        DateTime.Now

let getCurrentUnixTime: GetCurrentUnixTime =
    fun () ->
        DateTimeOffset.Now.ToUnixTimeSeconds()
