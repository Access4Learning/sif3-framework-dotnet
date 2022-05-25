# To do list

## General

- Deprecate the exceptions defined in Sif.Framework.Model.Exceptions in favour of those from Tardigrade.Framework.Exceptions.
- Migrate the unit tests of the Sif.Framework.Tests Project (Sif.Framework Solution) to the appropriate project in the Sif.Framework.Tests Solution.
- Convert the Sif.Framework.EntityFramework.Tests Project to an SDK-style project.
- Update all AutoMapper mapping definitions to use the new Profile mechanism.
- Replace the HttpUtils class with a RestSharp implementation.
- Rename the Sif.Framework.Model directory to Sif.Framework.Models.
- Rename the Sif.Framework.Service directory to Sif.Framework.Services.
- Replace RandomNameGenerator with Bogus.
- Remove references to the Sif.Specification.Infrastructure domain models from the IProvider interface to decouple the interface from SIF Infrastructure code. Once done, deprecate the SifInputFormatter class.
- Support PESC JSON notation.
- Convert the UK and US data model projects (from the SifSpecification Solution) from .NET Framework to .NET Standard SDK-style projects.

## .NET 6 upgrade

### Sif.Framework.AspNetCore

- **SDTP** Implement Query by Example for ASP.NET Core.
- **SDTP** Implement Matrix Parameters for ASP.NET Core.
- **SDTP** Implement Service Paths for ASP.NET Core.
- **SDTP** Create a .NET 6 version of SIF Consumers. Deprecate the HttpUtils class with an ASP.NET Core HttpClient implementation.
- **SDTP** Complete implementation of Functional Services in ASP.NET Core.
- **SDTP** Create a Goessner Notation JSON/XML serialiser for ASP.NET Core.
- **SDTP** Implement SIF Events for ASP.NET Core.
- Implement Basic and HMAC SHA Authentication and Authorisation using ASP.NET Core Middleware in-lieu of current implementation.
- Implement Dynamic Queries in the SIF Provider.
- For the HEAD method, need to clear the content body (for the purpose of optimisation).
- Split the Provider.Get(TSingle, string, string[], string[]) method into 2 separate methods: Get(TSingle, string, string[], string[]) and Get(string, string[], string[]).