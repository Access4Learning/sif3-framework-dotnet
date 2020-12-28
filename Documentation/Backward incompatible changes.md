# Backward incompatible changes


### **3.2.1.14 -> 4.0.0**

- BasicConsumer constructors

  - Renamed the LegacyConfigurationProvider class to LegacySettingsConfigurationProvider.
  - Renamed the LegacyConfigurationSource class to LegacySettingsConfigurationSource.
  - Updated the signature of the ApplicationConfiguration constructor to accept multiple configuration sources.
- Tardigrade.Framework.AspNetCore
  - Updated the project from supporting both .NET Core 2.2 and 3.0 to just 3.1.
- Tardigrade.Framework.EntityFrameworkCore
  - Updated the project from .NET Core 3.0 to 3.1.
