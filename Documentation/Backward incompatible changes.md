# Backward incompatible changes


### **3.2.1.14 -> 4.0.0**

- The public Consumer(Environment) constructor has been made protected.
- The public BasicConsumer(Environment) constructor has been made protected.
- The public BasicEventConsumer(Environment) constructor has been made protected.
- The IBasicProvider<T> interface has been removed.
- Extend the ObjectProvider class rather than the Provider class when creating SIF Providers whose TMultiple generic type is not a list of the TSingle generic type.
- Implement the IObjectProviderService interface rather than the IProviderService interface when creating SIF Provider services whose TMultiple generic type is not a list of the TSingle generic type.
- The empty default constructor of the abstract Provider class has been removed.
- The empty default constructor of the AuthorisationService class has been removed.
- The Sif.Framework project only targets .NET Standard 2.0, .NET Framework 4.7.2 and .NET Framework 4.6.1. It no longer targets .NET Frameworks 4.6.2, 4.7 and 4.7.1.
- The ConsumerRegistrationService  and ProviderRegistrationService static properties of the static RegistrationManager class have been made obsolete.
