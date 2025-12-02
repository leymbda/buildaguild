namespace Domain

open System

type Id = Id of Guid

module Id =
    let create () =
        Id (Guid.NewGuid())

    let fromString (id: string) =
        match Guid.TryParse id with
        | true, guid -> Some (Id guid)
        | false, _ -> None
        
    let (|Match|_|) (id: string) =
        fromString id

    let toString (Id id) =
        id.ToString()
