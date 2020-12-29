# Reading application settings from a database

Both SIF Providers and Consumers now support the passing of SIF Framework application settings through a constructor parameter. This allows application settings to be read from any source and not just from the SifFramework.config file (which is used by default if no application settings are specified). This includes application settings stored in a database.

## SIF Consumer

The following modifications were required to allow the Demo AU Consumer to read appliations settings from a database:

> 1. Installed the System.Data.SQLite NuGet package (not to be used for a production1system).
> 1. Installed the Tardigrade.Framework.EntityFramework NuGet package.
> 1. Added an SQLite database file where the application settings are stored in the AppSettings database table. Ensure that the `Build Action` is `None` and that the `Copy to Output Directory` is `Copy if newer`. An appropriate SQL script for the AppSettings databaes table can be found in the `Scripts\SQL\Application settings table` directory.
> 1. Configured the App.config file to support SQLite with EntityFramework 6, including defining the database file to be used.
> 1. Updated the Demo AU Consumers to implement the constructors that pass an IFrameworkSettings parameter.
> 1. In the ConsumerApp, read the application settings from the database and passed them to the constructor of the SIF Consumer (in this case, StudentPersonalConsumer).

To read the application settings from a database, the following code is used:

```cs
IFrameworkSettings settings =
    new ConsumerSettings(
        new ApplicationConfiguration(
            new AppSettingsConfigurationSource("name=SettingsDb")));
```

The AppSettingsConfigurationSource constructor takes a connection string name that has been configured in the App.config file. The database file is referenced by default from the `bin\Debug` directory with the `|DataDirectory|` substitution string.

The AppSettingsConfigurationSource and ApplicationConfiguration classes are referenced from the Tardigrade.Framework.EntityFramework and Tardigrade.Framework NuGet packages respectively.


## SIF Provider

The following modifications were required to allow the Demo AU Provider to read appliations settings from a database:

> 1. Installed the System.Data.SQLite NuGet package (not to be used for a production1system).
> 1. Installed the Tardigrade.Framework.EntityFramework NuGet package.
> 1. Added an SQLite database file to the App_Data directory. The application settings are defined in the AppSettings database table. Ensure that the `Build Action` is `None` and that the `Copy to Output Directory` is `Copy if newer`. An appropriate SQL script for the AppSettings databaes table can be found in the `Scripts\SQL\Application settings table` directory.
> 1. Configured the Web.config file to support SQLite with EntityFramework 6, including defining the database file to be used.
> 1. Updated the Global.asax.cs file to read the application settings from the database and then passed them to the new `RegistrationManager.GetProviderRegistrationService(IFrameworkSettings, ISessionService)` method in the updated `Register(IFrameworkSettings)` method. All previous references to `SettingsManager.ProviderSettings` were replaced.
> 1. Updated the Demo AU Providers to implement the constructors that pass an IFrameworkSettings parameter.

To read the application settings from a database, the following code is used:

```cs
IFrameworkSettings settings =
    new ProviderSettings(
        new ApplicationConfiguration(
            new AppSettingsConfigurationSource("name=SettingsDb")));
```

The AppSettingsConfigurationSource constructor takes a connection string name that has been configured in the App.config file. The database file is referenced by default from the `App_Data` directory with the `|DataDirectory|` substitution string.

The AppSettingsConfigurationSource and ApplicationConfiguration classes are referenced from the Tardigrade.Framework.EntityFramework and Tardigrade.Framework NuGet packages respectively.
