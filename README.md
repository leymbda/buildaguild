# Build a Guild

Build a Guild is a Discord activity built using Fable and Cloudflare Workers for the 2025 DDevs Buildathon.

## Setup and Run Locally

1. Run `npm i`, `dotnet restore`, and `dotnet tool restore` to restore necessary packages and tools solution-wide
2. Create `.env` file based on the `.env.example` and fill in necessary local secrets
3. Run `npm run build+start` to compile and run the solution

## Development Notes

- Building the solution will generate `Api.Presentation/worker-configuration.d.ts` which should be used to confirm `Api.Application/Interfaces/Env.fs` contains all necessary bindings
- To review TypeScript code gen, run `npm run build:ts`. Note that this is not used to run the worker and only exists for debugging purposes
- Database migrations are automatically run as part of the build process of the API, but may prompt confirmation if there are pending changes
- To delete all Fable artefacts from the solution in order to build fresh, run `npm run clean`
- Naming D1 migrations follows the convention of using the current time to order runs correctly. For example, a migration created at 2025-12-06 10:16 would be named with the prefix `202512061016_`
