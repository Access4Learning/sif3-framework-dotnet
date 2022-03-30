# To do list

## General

- Deprecate the exceptions defined in Sif.Framework.Model.Exceptions in favour of those from Tardigrade.Framework.Exceptions.

## .NET 6 upgrade

### Tardigrade.Framework.AspNet

- Split the Provider.Get(TSingle, string, string[], string[]) method into 2 separate methods: Get(TSingle, string, string[], string[]) and Get(string, string[], string[]).