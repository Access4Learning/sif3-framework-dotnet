/*
 * Crown Copyright © Department for Education (UK) 2016
 * Copyright 2020 Systemic Pty Ltd
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Sif.Framework.Model.DataModels;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service;
using Sif.Framework.Service.Functional;
using Sif.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Sif.Framework.Providers
{
    /// <summary>
    /// A factory for functional services. Manages timeouts, etc.
    /// </summary>
    public class FunctionalServiceProviderFactory
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Object locked = new Object();

        private static FunctionalServiceProviderFactory factory = null;

        private Timer eventTimer = null;
        private Timer timeoutTimer = null;

        /// <summary>
        /// Application settings associated with the Provider.
        /// </summary>
        private IFrameworkSettings ProviderSettings { get; }

        // Active Providers for event publishing. These providers run in the background as an independent thread.
        private Dictionary<string, IFunctionalService> providers = new Dictionary<string, IFunctionalService>();

        private Dictionary<string, Thread> providerThreads = new Dictionary<string, Thread>();

        // Known providers that can be instantiated for standard request/response
        private Dictionary<string, ServiceClassInfo> providerClasses = new Dictionary<string, ServiceClassInfo>();

        private Type[] classes = null;
        private Type[] Classes
        {
            get
            {
                if (classes != null)
                {
                    return classes;
                }

                string classesStr = ProviderSettings.JobClasses;

                log.Debug("Attempting to load named providers: " + classesStr);

                if (StringUtils.IsEmpty(classesStr))
                {
                    classes = Type.EmptyTypes;
                    return classes;
                }

                if (classesStr.ToLower().Equals("any"))
                {
                    classes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                               from type in assembly.GetTypes()
                               where ProviderUtils.isFunctionalService(type)
                               select type).ToArray();

                    foreach (Type t in classes)
                    {
                        log.Info("Identified service class " + t.Name + ", to specifically identify this service in your configuration file use:\n" + t.AssemblyQualifiedName);
                    }

                    return classes;
                }

                List<Type> providers = new List<Type>();
                string[] classNames = classesStr.Split('|');
                foreach (string className in classNames)
                {
                    Type provider = Type.GetType(className.Trim());
                    if (provider == null)
                    {
                        log.Error("Could not find provider with assembly qualified name " + className);
                    }
                    else
                    {
                        providers.Add(provider);
                    }
                }
                classes = providers.ToArray();
                return classes;
            }
        }

        /// <summary>
        /// Creates and configures the factory singleton instance.
        /// </summary>
        /// <returns>The factory singleton</returns>
        public static FunctionalServiceProviderFactory CreateFactory(IFrameworkSettings settings = null)
        {
            lock (locked)
            {
                log.Debug("Total Threads running before initialising provider Factory: " + Process.GetCurrentProcess().Threads.Count + " threads.");
                if (factory == null)
                {
                    try
                    {
                        factory = new FunctionalServiceProviderFactory(settings);
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
        public static FunctionalServiceProviderFactory GetInstance(IFrameworkSettings settings = null)
        {
            if (factory == null)
            {
                return CreateFactory(settings);
            }
            return factory;
        }

        /// <summary>
        /// Gets a new instance of a named provider service.
        /// </summary>
        /// <param name="name">The name of the provider service to look for</param>
        /// <returns>See description</returns>
        public IService GetProvider(string name)
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
        private FunctionalServiceProviderFactory(IFrameworkSettings settings = null)
        {
            ProviderSettings = settings ?? SettingsManager.ProviderSettings;
            InitialiseProviders();
            StartProviders(ProviderSettings);
            StartEventing();
            StartTimeout(ProviderSettings);
        }

        private void InitialiseProviders()
        {
            log.Debug("Initialising ProviderFactory (currently only supports Functional Services)");
            // settings.Classes only returns functional services at the moment, but can easily be extended to other types of services.
            foreach (Type type in Classes)
            {
                log.Debug("Provider class to initialise: " + type.FullName);
                try
                {
                    ServiceClassInfo providerClassInfo = new ServiceClassInfo(type, Type.EmptyTypes);
                    
                    if(!providerClassInfo.HasConstructor())
                    {
                        log.Error("The provider class " + type.FullName + " does not have a valid constructor. Must have a public constructor that takes no arguments.");
                        continue;
                    }

                    IFunctionalService provider = providerClassInfo.GetClassInstance() as IFunctionalService;
                    
                    string providerName = provider.GetServiceName();

                    log.Info("Adding provider for '" + providerName + "', using provider class '" + provider.GetType().FullName + "'.");

                    // First add it to the standard request/response dictionary
                    providerClasses[providerName] = providerClassInfo;

                    // Add it to dictionary of providers
                    providers[providerName] = provider;

                    // Add it to dictionary of background threads
                    providerThreads[providerName] = new Thread(new ThreadStart(provider.Startup));
                    // Each thread is essentially a global instance of a service whose responsibility is to maintain the services using timed tasks etc. - it never recieves any REST calls.
                }
                catch (Exception ex)
                {

                    log.Error("Cannot create Provider Class " + type.FullName + ": " + ex.Message, ex);
                }
            }
        }

        private void StartProviders(IFrameworkSettings settings)
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

        private void StartEventing()
        {
            // Incomplete and removed from current version of framework
            /*
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
                    // Call an event method
                }
            }, null, 0, frequency);
            */
        }

        private void StartTimeout(IFrameworkSettings settings)
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
