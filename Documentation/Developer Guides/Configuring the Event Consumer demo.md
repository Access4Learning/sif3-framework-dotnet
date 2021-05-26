Configuring the Event Consumer demo
-----------------------------------

1. In the demo Consumer project:

   + Set the consumer.environment.template.applicationKey app setting to Sif3DemoEventConsumer.
   + Configure the consumer.environment.url app setting to reference the BROKERED environment endpoint - http://localhost:59586/api/environments/environment.
   + Configure the EventConsumerApp to be the Startup object for the project.

1. In the demo Provider project:

   + Set the provider.environmentType app setting to BROKERED.
   + Configure the provider.environment.url app setting to reference the BROKERED environment endpoint - http://localhost:59586/api/environments/environment.

1. Run the demo Broker instead of the EnvironmentProvider.

1. Run the demo Provider.

1. Run the demo Consumer.
