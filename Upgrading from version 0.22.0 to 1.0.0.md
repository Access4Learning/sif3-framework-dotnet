Upgrading from version 0.22.0 to 1.0.0
--------------------------------------

1. SIF data model objects need to extend IDataModel instead of IPersistable<string>. This applies for both Consumer and Provider objects.

2. Consumers need to extend BasicConsumer<T> instead of GenericConsumer<T,PK>.

3. Provider services need to implement IBasicProviderService<T> instead of IGenericService<T,PK>.

4. Providers need to extend BasicProvider<T> instead of GenericController<T,PK>.

5. Providers need to replace the "Controller" postfix naming convention with "Provider", e.g. SchoolInfosProvider instead of SchoolInfosController.

6. The following entries need to be added to the WebApiConfig.cs file to cater for the change to the "Controller" postfix naming convention.

    ```dos
    config.Services.Replace(typeof(IHttpControllerTypeResolver), new ServiceProviderHttpControllerTypeResolver());

    FieldInfo suffix = typeof(DefaultHttpControllerSelector).GetField("ControllerSuffix", BindingFlags.Static | BindingFlags.Public);
    if (suffix != null) suffix.SetValue(null, "Provider");
    ```
7. The following Web API specific classes have changed Namespace: GlobalUnexpectedExceptionHandler.cs and MethodOverrideHandler.cs.
