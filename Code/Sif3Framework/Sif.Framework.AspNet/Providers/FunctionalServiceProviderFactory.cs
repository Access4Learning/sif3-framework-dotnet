/*
 * Crown Copyright © Department for Education (UK) 2016
 * Copyright 2022 Systemic Pty Ltd
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
using slf4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Sif.Framework.AspNet.Providers
{
    /// <summary>
    /// A factory for functional services. Manages timeouts, etc.
    /// </summary>
    public class FunctionalServiceProviderFactory
    {
        private static readonly object Locked = new object();

        private static readonly ILogger Log =
            LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private static FunctionalServiceProviderFactory _factory;

        private Timer _eventTimer;
        private Timer _timeoutTimer;

        /// <summary>
        /// Application settings associated with the Provider.
        /// </summary>
        private IFrameworkSettings ProviderSettings { get; }

        // Active Providers for event publishing. These providers run in the background as an independent thread.
        private readonly Dictionary<string, IFunctionalService> _providers =
            new Dictionary<string, IFunctionalService>();

        private readonly Dictionary<string, Thread> _providerThreads = new Dictionary<string, Thread>();

        // Known providers that can be instantiated for standard request/response
        private readonly Dictionary<string, ServiceClassInfo> _providerClasses =
            new Dictionary<string, ServiceClassInfo>();

        private Type[] _classes;

        private IEnumerable<Type> Classes
        {
            get
            {
                if (_classes != null) return _classes;

                string classesStr = ProviderSettings.JobClasses;

                Log.Debug($"Attempting to load named providers: {classesStr}");

                if (string.IsNullOrWhiteSpace(classesStr))
                {
                    _classes = Type.EmptyTypes;

                    return _classes;
                }

                if (classesStr.ToLower().Equals("any"))
                {
                    _classes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                from type in assembly.GetTypes()
                                where ProviderUtils.IsFunctionalService(type)
                                select type).ToArray();

                    foreach (Type t in _classes)
                    {
                        Log.Info(
                            $"Identified service class {t.Name}, to specifically identify this service in your configuration file use:\n{t.AssemblyQualifiedName}");
                    }

                    return _classes;
                }

                var providers = new List<Type>();
                string[] classNames = classesStr.Split('|');

                foreach (string className in classNames)
                {
                    var provider = Type.GetType(className.Trim());

                    if (provider == null)
                    {
                        Log.Error($"Could not find provider with assembly qualified name {className}");
                    }
                    else
                    {
                        providers.Add(provider);
                    }
                }

                _classes = providers.ToArray();

                return _classes;
            }
        }

        /// <summary>
        /// Creates and configures the factory singleton instance.
        /// </summary>
        /// <returns>The factory singleton</returns>
        public static FunctionalServiceProviderFactory CreateFactory(IFrameworkSettings settings = null)
        {
            lock (Locked)
            {
                Log.Debug(
                    $"Total Threads running before initialising provider Factory: {Process.GetCurrentProcess().Threads.Count} threads.");

                if (_factory == null)
                {
                    try
                    {
                        _factory = new FunctionalServiceProviderFactory(settings);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to initialise provider factory. Provider won't run.");
                        _factory = null;
                    }
                }

                Log.Debug(
                    $"Total Threads running after initialising Provider Factory: {Process.GetCurrentProcess().Threads.Count} threads.");

                return _factory;
            }
        }

        /**
         * This will shut down each provider class that make up this provider
         */

        public static void Shutdown()
        {
            lock (Locked)
            {
                if (_factory == null) return;

                Log.Info("Shutting down events...");

                if (_factory._eventTimer != null)
                {
                    _factory._eventTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    _factory._eventTimer.Dispose();
                    _factory._eventTimer = null;
                }

                Log.Info("Shutting job timeout task...");

                if (_factory._timeoutTimer != null)
                {
                    _factory._timeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    _factory._timeoutTimer.Dispose();
                    _factory._timeoutTimer = null;
                }

                Log.Info("Shutting down providers...");

                foreach (string name in _factory._providers.Keys)
                {
                    try
                    {
                        Log.Info($"--- {name}");
                        _factory._providers[name].Shutdown();
                    }
                    catch (Exception ex)
                    {
                        Log.Warn(ex, ex.Message);
                    }
                }

                Log.Info("Stopping provider threads...");

                foreach (string name in _factory._providerThreads.Keys)
                {
                    try
                    {
                        Log.Info($"--- {name}");
                        _factory._providerThreads[name].Abort();
                    }
                    catch (Exception ex)
                    {
                        Log.Warn(ex, ex.Message);
                    }
                }
            }

            Log.Info("All providers are shut down.");
        }

        /**
         * Returns an lazy loaded instance of this provider factory.
         *
         * @return See Desc.
         */

        public static FunctionalServiceProviderFactory GetInstance(IFrameworkSettings settings = null)
        {
            lock (Locked)
            {
                return _factory ?? CreateFactory(settings);
            }
        }

        /// <summary>
        /// Gets a new instance of a named provider service.
        /// </summary>
        /// <param name="name">The name of the provider service to look for</param>
        /// <returns>See description</returns>
        public IService GetProvider(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Log.Error("The provider name is either null or empty. This is required! No Provider returned.");

                return null;
            }

            if (!_providerClasses.Keys.Contains(name))
            {
                Log.Error($"No known provider for {name}");

                return null;
            }

            try
            {
                return _providerClasses[name].GetClassInstance();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to instantiate a provider for {name}: {ex.Message}");

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
            Log.Debug("Initialising ProviderFactory (currently only supports Functional Services)");

            // settings.Classes only returns functional services at the moment, but can easily be extended to other
            // types of services.
            foreach (Type type in Classes)
            {
                Log.Debug($"Provider class to initialise: {type.FullName}");

                try
                {
                    var providerClassInfo = new ServiceClassInfo(type, Type.EmptyTypes);

                    if (!providerClassInfo.HasConstructor())
                    {
                        Log.Error(
                            $"The provider class {type.FullName} does not have a valid constructor. Must have a public constructor that takes no arguments.");

                        continue;
                    }

                    if (providerClassInfo.GetClassInstance() is IFunctionalService provider)
                    {
                        string providerName = provider.GetServiceName();

                        Log.Info(
                            $"Adding provider for '{providerName}', using provider class 'provider?.GetType().FullName'.");

                        // First add it to the standard request/response dictionary
                        _providerClasses[providerName] = providerClassInfo;

                        // Add it to dictionary of providers
                        _providers[providerName] = provider;

                        // Add it to dictionary of background threads
                        _providerThreads[providerName] = new Thread(provider.Startup);
                        // Each thread is essentially a global instance of a service whose responsibility is to
                        // maintain the services using timed tasks etc. - it never receives any REST calls.
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Cannot create Provider Class {type.FullName}: {ex.Message}");
                }
            }
        }

        private void StartProviders(IFrameworkSettings settings)
        {
            int delay = settings.StartupDelay;  //delay between threads in seconds

            Log.Debug($"Start up delay between providers is: {delay} seconds");

            var i = 0;

            foreach (string serviceName in _providerThreads.Keys)
            {
                Log.Debug($"Starting thread for {serviceName}");

                var _ = new Timer(o =>
                {
                    _providerThreads[serviceName].Start();
                }, null, i * delay, Timeout.Infinite);

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

            _eventTimer = new Timer((o) =>
            {
                foreach(IService service in _providers.Values)
                {
                    // Call an event method
                }
            }, null, 0, frequency);
            */
        }

        private void StartTimeout(IFrameworkSettings settings)
        {
            Log.Info("Setting up job timeout...");

            if (!settings.JobTimeoutEnabled)
            {
                Log.Debug("Job timeout disabled in settings.");

                return;
            }

            int frequencyInSec = settings.JobTimeoutFrequency;

            if (frequencyInSec == 0)
            {
                Log.Debug("Job timeout enabled, but timeout currently turned off (frequency=0)");

                return;
            }

            int frequency = frequencyInSec * 1000;

            Log.Info($"Jobs timeout task will run every {frequencyInSec}s ({frequency}ms).");

            _timeoutTimer = new Timer(o =>
            {
                foreach (IFunctionalService service in _providers.Values)
                {
                    service.JobTimeout();
                }
            }, null, 0, frequency);
        }
    }
}