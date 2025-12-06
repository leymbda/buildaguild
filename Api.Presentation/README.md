# Api

This is the API project for **Build a Guild**, built using Fable and Cloudflare Workers.

## Setup Development Environment

1. Run `npm i`, `dotnet restore`, and `dotnet tool restore` to restore necessary packages and tools
2. Create `.env` file based on the `.env.example` and fill in necessary local secrets

## Start Running Locally

1. Run `npm run build` to compile code and regenerate `worker-configuration.d.ts`
2. Run `npm start` to start the worker

You can also use `npm run build+start` to do both steps in one command. This is particularly useful when testing quick small code changes as the single command will rebuild and restart the worker.

If you wish to delete all Fable artifacts from the solution in order to build fresh, run `npm run clean`.

If generating TS is necessary in order to inspect types being generated, run `npm run build:ts` instead. Note that the worker is setup to run using the JS output, so this TS build cannot be used for running the worker.

## Databse Migrations

Database migrations are run as part of the build script, but can also be run manually through `npm run migrate`. Naming migrations follows the convention of using the current time to order runs correctly. For example, a migration created at 2025-12-06 10:16 would be named with the prefix `202512061016_`. 

## Updating Worker Bindings

Worker bindings are bound to `Env` located in `Api.Application/Interfaces/Env.fs`. Whenever bindings get changed in `wrangler.jsonc`, the `worker-configuration.d.ts` will update to the latest bindings given the dependencies are properly exported from `index.fs`. Once this has been updated, `Env.fs` needs to be updated to reflect it.

Due to the lack of support for circular references in F# and the stubbing behaviour of the env, bindings such as durable objects need to be defined independently. These interfaces are created in `Api.Application/Bindings`. This allows the intended stub type to be properly reflected and for the bindings themselves to be defined independently.
