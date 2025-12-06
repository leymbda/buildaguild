namespace rec Api.Application

open System

type ITime = {
    GetCurrentTime: ITime.GetCurrentTime
    GetCurrentUnixTime: ITime.GetCurrentUnixTime
}

module ITime =
    type GetCurrentTime = unit -> DateTime
    
    type GetCurrentUnixTime = unit -> int64

type TimeDI =
    abstract Time: ITime
