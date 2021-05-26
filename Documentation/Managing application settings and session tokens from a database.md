# Managing application settings and session tokens from a database

Both SIF Consumers and Providers now support the passing of SIF Framework application settings and session tokens through constructor parameters. This allows these values to be read from any source (including a database) and not just from the SifFramework.config file (which is used by default if no constructor parameters are specified).

## SIF Consumer

The following modifications were made to the Demo AU Consumer.

### Common configuration

> 1. Installed the System.Data.SQLite NuGet package (not to be used for a production system).
> 1. Installed the Tardigrade.Framework.EntityFramework NuGet package.
> 1. Added an SQLite database file. Ensure that the `Build Action` is `None` and that the `Copy to Output Directory` is `Copy if newer`.
> 1. Configured the App.config file to support SQLite with EntityFramework 6, including defining the database file to be used. Instructions for the use of SQLite with EntityFramework is not within the scope of this document.

### Application settings

> 1. Created an AppSettings database table. An appropriate SQL script for the AppSettings database table can be found in the `Scripts\SQL\Application settings table` directory.
> 1. Updated the Demo AU Consumer classes to implement the constructors that pass an IFrameworkSettings parameter.
> 1. In the ConsumerApp class, read the application settings from the database and passed them to the constructor of the SIF Consumer (in this case, StudentPersonalConsumer).

To read the application settings from a database, the following code is used:

```cs
IFrameworkSettings settings =
    new ConsumerSettings(
        new ApplicationConfiguration(
            new AppSettingsConfigurationSource("name=FrameworkConfigDb")));
```

The AppSettingsConfigurationSource constructor takes a connection string name that has been configured in the App.config file. The database file is referenced by default from the `bin\Debug` directory with the `|DataDirectory|` substitution string.

The AppSettingsConfigurationSource and ApplicationConfiguration classes are referenced from the Tardigrade.Framework.EntityFramework and Tardigrade.Framework NuGet packages respectively.

### Session tokens

> 1. Installed the Sif.Framework.EntityFramework NuGet package.
> 1. Created a Sessions database table. An appropriate SQL script for the Sessions database table can be found in the `Scripts\SQL\Session table` directory.
> 1. Updated the Demo AU Consumer classes to implement the constructors that pass an ISessionService parameter.
> 1. In the ConsumerApp class, created the session service and passed it to the constructor of the SIF Consumer (in this case, StudentPersonalConsumer).

To create the session service, the following code is used:

```cs
DbContext dbContext = new SessionDbContext("name=FrameworkConfigDb");
IRepository<Session, Guid> repository = new Repository<Session, Guid>(dbContext);
IObjectService<Session, Guid> service = new ObjectService<Session, Guid>(repository);
ISessionService sessionService = new SessionService(service);
```

The SessionDbContext constructor takes a connection string name that has been configured in the App.config file. The database file is referenced by default from the `bin\Debug` directory with the `|DataDirectory|` substitution string.

The Repository and ObjectService classes are referenced from the Tardigrade.Framework.EntityFramework and Tardigrade.Framework NuGet packages respectively. The SessionDbContext and SessionService classes are referenced from the Sif.Framework.EntityFramework NuGet package.

## SIF Provider

The following modifications were made to the Demo AU Provider.

### Common configuration

> 1. Installed the System.Data.SQLite NuGet package (not to be used for a production system).
> 1. Installed the Tardigrade.Framework.EntityFramework NuGet package.
> 1. Added an SQLite database file to the App_Data directory. Ensure that the `Build Action` is `None` and that the `Copy to Output Directory` is `Copy if newer`.
> 1. Configured the Web.config file to support SQLite with EntityFramework 6, including defining the database file to be used. Instructions for the use of SQLite with EntityFramework is not within the scope of this document.
> 1. Added the `demo.frameworkConfigSource` application setting to the Web.config file and set it's value to `Database`.

### Application settings

> 1. Created an AppSettings database table. An appropriate SQL script for the AppSettings databaes table can be found in the `Scripts\SQL\Application settings table` directory.
> 1. Updated the Global.asax.cs file to read the application settings from the database and pass them to the `RegistrationManager.GetProviderRegistrationService(IFrameworkSettings, ISessionService)` method. All previous references to `SettingsManager.ProviderSettings` were replaced.
> 1. Updated the Demo AU Providers to implement the constructors that pass an IFrameworkSettings parameter.

To read the application settings from a database, the following code is used:

```cs
IFrameworkSettings settings =
    new ProviderSettings(
        new ApplicationConfiguration(
            new AppSettingsConfigurationSource("name=FrameworkConfigDb")));
```

The AppSettingsConfigurationSource constructor takes a connection string name that has been configured in the Web.config file. The database file is referenced by default from the `App_Data` directory with the `|DataDirectory|` substitution string.

The AppSettingsConfigurationSource and ApplicationConfiguration classes are referenced from the Tardigrade.Framework.EntityFramework and Tardigrade.Framework NuGet packages respectively.

### Session tokens

> 1. Installed the Sif.Framework.EntityFramework NuGet package.
> 1. Created a Sessions database table. An appropriate SQL script for the Sessions database table can be found in the `Scripts\SQL\Session table` directory.
> 1. Updated the Global.asax.cs file to create a session service to pass to the `RegistrationManager.GetProviderRegistrationService(IFrameworkSettings, ISessionService)` method.
> 1. Updated the Demo AU Providers to implement the constructors that pass an ISessionService parameter.

To create the session service, the following code is used:

```cs
DbContext dbContext = new SessionDbContext("name=FrameworkConfigDb");
IRepository<Session, Guid> repository = new Repository<Session, Guid>(dbContext);
IObjectService<Session, Guid> service = new ObjectService<Session, Guid>(repository);
ISessionService sessionService = new SessionService(service);
```

The SessionDbContext constructor takes a connection string name that has been configured in the Web.config file. The database file is referenced by default from the `App_Data` directory with the `|DataDirectory|` substitution string.

The Repository and ObjectService classes are referenced from the Tardigrade.Framework.EntityFramework and Tardigrade.Framework NuGet packages respectively. The SessionDbContext and SessionService classes are referenced from the Sif.Framework.EntityFramework NuGet package.
