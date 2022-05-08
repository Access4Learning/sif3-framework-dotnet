# To do list

## General

- Deprecate the exceptions defined in Sif.Framework.Model.Exceptions in favour of those from Tardigrade.Framework.Exceptions.
- Migrate the unit tests of the Sif.Framework.Tests Project (Sif.Framework Solution) to the appropriate project in the Sif.Framework.Tests Solution.
- Convert the Sif.Framework.EntityFramework.Tests Project to an SDK style project.
- Update all AutoMapper mapping definitions to use the new Profile mechanism.
- Replace the HttpUtils class with a RestSharp implementation.
- Rename the Sif.Framework.Model directory to Sif.Framework.Models.
- Rename the Sif.Framework.Service directory to Sif.Framework.Services.
- Replace RandomNameGenerator with Bogus.
- Remove references to the Sif.Specification.Infrastructure domain models from the IProvider interface to decouple the interface from SIF Infrastructure code. Once done, deprecate the SifInputFormatter class.

## .NET 6 upgrade

### Sif.Framework.AspNetCore

- Replace the HttpUtils class with an ASP.NET Core HttpClient implementation.
- Create a Goessner Notation JSON/XML serialiser for ASP.NET Core.
- Implement Matrix Parameters for ASP.NET Core.
- Implement Service Paths for ASP.NET Core.
- Implement SIF Events for ASP.NET Core.
- Implement ASP.NET Core Middleware for Basic and HMAC SHA Authentication and Authorisation.
- Implement Dynamic Queries in the SIF Provider.
- Create BAT scripts for running ASP.NET Core apps.
- For the HEAD method, need to clear the content body (for the purpose of optimisation).
- Split the Provider.Get(TSingle, string, string[], string[]) method into 2 separate methods: Get(TSingle, string, string[], string[]) and Get(string, string[], string[]).