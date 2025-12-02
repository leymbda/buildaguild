namespace Api.Application

open Fable.Bindings.CloudflareWorkers

[<CompiledName "Env">]
type Env =
    abstract CLIENT_ID: string
    abstract CLIENT_PUBLIC_KEY: string
    abstract REDIRECT_URI: string
        
type EnvDI =
    abstract Env: Env
