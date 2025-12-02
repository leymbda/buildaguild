namespace Api.Application

open Fable.Bindings.CloudflareWorkers

[<CompiledName "Env">]
type Env =
    abstract SESSION_KV: obj // TODO: KV binding
    abstract CLIENT_ID: string
    abstract CLIENT_PUBLIC_KEY: string
    abstract REDIRECT_URI: string
    abstract CLIENT_SECRET: string
    abstract BOT_TOKEN: string
    abstract D1: obj // TODO: D1 binding
        
type EnvDI =
    abstract Env: Env
