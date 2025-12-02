namespace Api.Application

open Fable.Bindings.CloudflareWorkers

[<CompiledName "Env">]
type Env =
    abstract CLIENT_ID: string
    abstract CLIENT_PUBLIC_KEY: string
    abstract REDIRECT_URI: string
    abstract D1: obj // TODO: D1 binding
    abstract SESSION_KV: obj // TODO: KV binding
        
type EnvDI =
    abstract Env: Env
