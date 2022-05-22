# Troubleshooting Guide

### Parameterless public constructor

> An error occurred when trying to create a controller of type 'StudentPersonalsProvider'. Make sure that the controller has a parameterless public constructor.

If you have an ASP.NET (Web API) Controller whose default constructor defers to the constructor of a base class, this message may occur if an exception is raised in the base class constructor. In the following sample code, for instance, if the database referenced by "name=SettingsDb" cannot be found, the underlying exception may be hidden by the aforementioned error message.

    public StudentPersonalsProvider() : base(
        new StudentPersonalService(),
        new ProviderSettings(
            new ApplicationConfiguration(
                new AppSettingsConfigurationSource("name=SettingsDb"))))
    {
    }

