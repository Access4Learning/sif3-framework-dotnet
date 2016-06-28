using log4net;
using Sif.Framework.Model.DataModels;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service;
using Sif.Framework.Service.Functional;
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Sif.Framework.Providers
{
    public class FunctionalServiceProviderFactory
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Object locked = new Object();

        private static FunctionalServiceProviderFactory factory = null;

        private Timer eventTimer = null;
        private Timer timeoutTimer = null;

        // Active Providers for event publishing. These providers run in the background as an independent thread.
        private Dictionary<string, IFunctionalService> providers = new Dictionary<string, IFunctionalService>();

        private Dictionary<string, Thread> providerThreads = new Dictionary<string, Thread>();

        // Known providers that can be instantiated for standard request/response
        private Dictionary<string, ProviderClassInfo> providerClasses = new Dictionary<string, ProviderClassInfo>();

        public static FunctionalServiceProviderFactory CreateFactory()
        {
            lock (locked)
            {
                log.Debug("Total Threads running before initialising provider Factory: " + Process.GetCurrentProcess().Threads.Count + " threads.");
                if (factory == null)
                {
                    try
                    {
                        factory = new FunctionalServiceProviderFactory();
                    }
                    catch (Exception ex)
                    {
                        log.Error("Failed to initialise provider factory. Provider won't run.", ex);
                        factory = null;
                    }
                }
                log.Debug("Total Threads running after initialising Provider Factory: " + Process.GetCurrentProcess().Threads.Count + " threads.");
                return factory;
            }
        }

        /**
         * This will shut down each provider class that make up this provider
         */
        public static void Shutdown()
        {
            lock (locked)
            {
                if (factory == null)
                {
                    return;
                }

                log.Info("Shutting down events...");
                if (factory.eventTimer != null)
                {
                    factory.eventTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    factory.eventTimer.Dispose();
                    factory.eventTimer = null;
                }

                log.Info("Shutting job timeout task...");
                if (factory.timeoutTimer != null)
                {
                    factory.timeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    factory.timeoutTimer.Dispose();
                    factory.timeoutTimer = null;
                }

                log.Info("Shutting down providers...");
                foreach (string name in factory.providers.Keys)
                {
                    try
                    {
                        log.Info("--- " + name);
                        factory.providers[name].Shutdown();
                    }
                    catch (Exception ex)
                    {
                        log.Warn(ex.Message, ex);
                    }
                }

                log.Info("Stopping provider threads...");
                foreach (string name in factory.providerThreads.Keys)
                {
                    try
                    {
                        log.Info("--- " + name);
                        factory.providerThreads[name].Abort();
                    }
                    catch (Exception ex)
                    {
                        log.Warn(ex.Message, ex);
                    }
                }
            }
            log.Info("All providers are shut down.");
        }

        /**
         * Returns an lazy loaded instance of this provider factory.
         * 
         * @return See Desc.
         */
        public static FunctionalServiceProviderFactory GetInstance()
        {
            if (factory == null)
            {
                return CreateFactory();
            }
            return factory;
        }

        public dynamic GetProvider(string name)
        {
            if (StringUtils.IsEmpty(name))
            {
                log.Error("The provider name is either null or empty. This is required! No Provider returned.");
                return null;
            }
            
            if (!providerClasses.Keys.Contains(name))
            {
                log.Error("No known provider for " + name);
                return null;
            }
            
            try
            {
                return providerClasses[name].GetClassInstance(null);
            }
            catch (Exception ex)
            {
                log.Error("Failed to instantiate a provider for " + name + ": " + ex.Message, ex);
                return null;
            }
        }

        /*---------------------*/
        /*-- Private Methods --*/
        /*---------------------*/
        private FunctionalServiceProviderFactory()
        {
            ProviderSettings settings = SettingsManager.ProviderSettings as ProviderSettings;
            InitialiseProviders(settings);
            StartProviders(settings);
            StartEventing(settings);
            StartTimeout(settings);
        }

        private void InitialiseProviders(ProviderSettings settings)
        {
            log.Debug("Initialising ProviderFactory (currently only supports Functional Services)");
            // settings.Classes only returns functional services at the moment, but can easily be extended to other types of services.
            foreach (Type type in settings.Classes)
            {
                log.Debug("Provider class to initialse: " + type.FullName);
                try
                {
                    ProviderClassInfo providerClassInfo = new ProviderClassInfo(type, Type.EmptyTypes );
                    
                    if(providerClassInfo.GetConstructor() == null)
                    {
                        log.Error("The provider class " + type.FullName + " does not have a valid constructor. Must have a public constructor that takes no arguments.");
                        continue;
                    }

                    IFunctionalService provider = providerClassInfo.GetClassInstance() as IFunctionalService;

                    if (StringUtils.IsEmpty(provider.GetServiceName()))
                    {
                        log.Error("The provider is returning null or empty string from getServiceName(). Provider '" + provider.GetType().FullName + " not added to provider factory.");
                        continue;
                    }

                    log.Info("Adding provider for '" + provider.GetServiceName() + "', using provider class '" + provider.GetType().FullName + "'.");

                    // First add it to the standard request/response dictionary
                    providerClasses[provider.GetServiceName()] = providerClassInfo;

                    // Add it to dictionary of providers
                    providers[provider.GetServiceName()] = provider;

                    // Add it to dictionary of background threads
                    providerThreads[provider.GetServiceName()] = new Thread(new ThreadStart(provider.Startup));
                    // Each thread is essentially a global instance of a service whose responsibility is to maintain the services using timed tasks etc. - it never recieves any REST calls.
                }
                catch (Exception ex)
                {

                    log.Error("Cannot create Provider Class " + type.FullName + ": " + ex.Message, ex);
                }
            }
        }

        private void StartProviders(ProviderSettings settings)
        {
            int delay = settings.StartupDelay;  //delay between threads in seconds
            log.Debug("Start up delay between providers is: " + delay + " seconds");

            int i = 0;
            foreach (string serviceName in providerThreads.Keys)
            {
                log.Debug("Starting thread for " + serviceName);
                Timer timer = new Timer((o) => {
                    providerThreads[serviceName].Start();
                }, null, (i * delay), Timeout.Infinite);
                i += 1000;
            }
        }

        private void StartEventing(ProviderSettings settings)
        {
            log.Info("Setting up eventing...");
            if (!settings.EventsSupported)
            {
                log.Debug("Eventing disabled in settings.");
                return;
            }

            int frequencyInSec = settings.EventsFrequency;
            if (frequencyInSec == 0)
            {
                log.Info("Eventing enabled, but events currently turned off (frequency=0)");
                return;
            }

            int frequency = frequencyInSec * 1000;
            log.Info("Events will be issued every " + frequencyInSec + "s (" + frequency + "ms).");

            eventTimer = new Timer((o) =>
            {
                foreach(IService service in providers.Values)
                {
                    service.BroadcastEvents();
                }
            }, null, 0, frequency);
        }

        private void StartTimeout(ProviderSettings settings)
        {
            log.Info("Setting up job timeout...");
            if (!settings.JobTimeoutEnabled)
            {
                log.Debug("Job timeout disabled in settings.");
                return;
            }

            int frequencyInSec = settings.JobTimeoutFrequency;
            if (frequencyInSec == 0)
            {
                log.Debug("Job timeout enabled, but timeout currently turned off (frequency=0)");
                return;
            }

            int frequency = frequencyInSec * 1000;
            log.Info("Jobs timeout task will run every " + frequencyInSec + "s (" + frequency + "ms).");

            timeoutTimer = new Timer((o) =>
            {
                foreach (IFunctionalService service in providers.Values)
                {
                    service.JobTimeout();
                }
            }, null, 0, frequency);
        }
    }
}
