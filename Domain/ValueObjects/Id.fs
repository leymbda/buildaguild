namespace Domain

open System

type Id = Id of int64

module Id =
    let create (snowflake: int64) =
        Id snowflake

    let fromString (id: string) =
        match Int64.TryParse id with
        | true, snowflake -> Some (Id snowflake)
        | false, _ -> None
        
    let (|FromRoute|_|) (id: string) =
        fromString id

    let toString (Id id) =
        id.ToString()

    let toInt64 (Id id) =
        id
