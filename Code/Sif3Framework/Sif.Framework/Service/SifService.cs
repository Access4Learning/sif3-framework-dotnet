/*
 * Copyright 2014 Systemic Pty Ltd
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

using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Persistence;
using Sif.Framework.Persistence;
using Sif.Framework.Service.Mapper;
using System;
using System.Collections.Generic;
using Sif.Framework.Model.Settings;
using log4net;
using System.Reflection;
using System.Threading;

namespace Sif.Framework.Service
{

    public abstract class SifService<UI, DB> : ISifService<UI, DB> where DB : IPersistable<Guid>, new()
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected IGenericRepository<DB, Guid> repository;
        private Timer eventTimer = null;

        public abstract string getServiceName();

        public virtual ServiceType getServiceType()
        {
            return ServiceType.UTILITY;
        }

        public virtual void Run()
        {
            string serviceName = getServiceName();
            ProviderSettings settings = new ProviderSettings();
            log.Debug("Start " + serviceName + " provider thread....");

            // Only if we intend to support events we will start the event manager
            if (settings.EventsSupported)
            {
                int frequency = settings.EventsFrequency;
                if (frequency == 0)
                {
                    log.Info("Intending to issue events for  " + serviceName + ", but events currently turned off (frequency=0)");
                    return;
                }

                log.Debug("Starting events thread  for " + serviceName + ".");

                frequency = frequency * 1000;
                log.Info("Event Frequency = " + frequency + " secs.");

                log.Debug("Start sending events from " + serviceName + " provider...");

                eventTimer = new Timer((o) => {
                    log.Debug("Start Event Timer Task for " + serviceName + ".");
                    broadcastEvents();
                }, null, 0, frequency);
            }
            log.Debug(serviceName + " started.");
        }

        public virtual void Finalise()
        {
            if (eventTimer != null)
            {
                log.Debug("Shut Down event timer for: " + getServiceName());
                eventTimer.Change(Timeout.Infinite, Timeout.Infinite);
                eventTimer.Dispose();
                eventTimer = null;
            }
        }

        public SifService(IGenericRepository<DB, Guid> repository)
        {
            this.repository = repository;
        }

        public virtual Guid Create(UI item, string zone = null, string context = null)
        {
            DB repoItem = MapperFactory.CreateInstance<UI, DB>(item);
            return repository.Save(repoItem);
        }

        public virtual void Create(IEnumerable<UI> items, string zone = null, string context = null)
        {
            ICollection<DB> repoItems = MapperFactory.CreateInstances<UI, DB>(items);
            repository.Save(repoItems);
        }

        public virtual void Delete(Guid id, string zone = null, string context = null)
        {
            repository.Delete(id);
        }

        public virtual void Delete(UI item, string zone = null, string context = null)
        {
            DB repoItem = MapperFactory.CreateInstance<UI, DB>(item);
            repository.Delete(repoItem);
        }

        public virtual void Delete(IEnumerable<UI> items, string zone = null, string context = null)
        {
            ICollection<DB> repoItems = MapperFactory.CreateInstances<UI, DB>(items);
            repository.Delete(repoItems);
        }

        public virtual UI Retrieve(Guid id, string zone = null, string context = null)
        {
            DB repoItem = repository.Retrieve(id);
            return MapperFactory.CreateInstance<DB, UI>(repoItem);
        }

        public virtual ICollection<UI> Retrieve(UI item, string zone = null, string context = null)
        {
            DB repoItem = MapperFactory.CreateInstance<UI, DB>(item);
            ICollection<DB> repoItems = repository.Retrieve(repoItem);
            return MapperFactory.CreateInstances<DB, UI>(repoItems);
        }

        public virtual ICollection<UI> Retrieve(string zone = null, string context = null)
        {
            ICollection<DB> repoItems = repository.Retrieve();
            return MapperFactory.CreateInstances<DB, UI>(repoItems);
        }

        public virtual void Update(UI item, string zone = null, string context = null)
        {
            DB repoItem = MapperFactory.CreateInstance<UI, DB>(item);
            repository.Save(repoItem);
        }

        public virtual void Update(IEnumerable<UI> items, string zone = null, string context = null)
        {
            ICollection<DB> repoItems = MapperFactory.CreateInstances<UI, DB>(items);
            repository.Save(repoItems);
        }

        public virtual void broadcastEvents()
        {
            log.Debug("================================ broadcastEvents() called for provider " + getServiceName() + " (" + getServiceType().ToString() + ")");
            int totalRecords = 0;
            int failedRecords = 0;
            /*
            
            int maxNumObjPerEvent = getMaxObjectsInEvent();

            SIF3Session sif3Session = getActiveSession();
            if (sif3Session == null)
            {
                return; //cannot send events. Error already logged.
            }

            List<ServiceInfo> servicesForProvider = getServicesForProvider(sif3Session);

            // If there are no services for this provider defined then we don't need to get any events at all.
            if ((servicesForProvider == null) || (servicesForProvider.size() == 0))
            {
                logger.info("This environment does not have any zones and contexts defined for the " + getMultiObjectClassInfo().getObjectName() + " service. No events can be sent.");
                return;
            }
            try
            {
                // Let's get the Event Client
                EventClient evtClient = new EventClient(getEnvironmentManager(), getRequestMediaType(), getResponseMediaType(), getServiceName(), getMarshaller(), getCompressionEnabled());
                SIFEventIterator<L> iterator = getSIFEvents();
                if (iterator != null)
                {
                    while (iterator.hasNext())
                    {
                        SIFEvent<L> sifEvents = null;
                        try
                        {
                            sifEvents = iterator.getNextEvents(maxNumObjPerEvent);
                            // This should not return null since the hasNext() returned true, but just in case we check
                            // and exit the loop if it should return null. In this case we assume that there is no more
                            // data. We also log an error to make the coder aware of the issue.
                            if (sifEvents != null)
                            {
                                logger.debug("Number of " + getMultiObjectClassInfo().getObjectName() + " Objects in this Event: " + sifEvents.getListSize());
                                for (ServiceInfo service : servicesForProvider)
                                {
                                    // keep event action. Just in case the developer changes it in modifyBeforePublishing() which would confuse
                                    // everything.
                                    EventAction eventAction = sifEvents.getEventAction();
                                    if (hasAccess(service))
                                    {
                                        HeaderProperties customHTTPHeaders = new HeaderProperties();
                                        SIFEvent<L> modifiedEvents = modifyBeforePublishing(sifEvents, service.getZone(), service.getContext(), customHTTPHeaders);
                                        if (modifiedEvents != null)
                                        {
                                            //Just in case the developer has changed it. Should not be allowed :-)
                                            modifiedEvents.setEventAction(eventAction);

                                            if (!sendEvents(evtClient, modifiedEvents, service.getZone(), service.getContext(), customHTTPHeaders))
                                            {
                                                //Report back to the caller. This should also give the event back to the caller.
                                                onEventError(modifiedEvents, service.getZone(), service.getContext());
                                                failedRecords = failedRecords + ((modifiedEvents != null) ? modifiedEvents.getListSize() : 0);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        logger.debug("The " + getProviderName() + " does not have the PROVIDE = APPROVED. No events are sent.");
                                        failedRecords = failedRecords + ((sifEvents != null) ? sifEvents.getListSize() : 0);
                                    }
                                }

                                totalRecords = totalRecords + sifEvents.getListSize();
                            }
                            else
                            {
                                logger.error("iterator.hasNext() has returned true but iterator.getNextEvent() has retrurned null => no further events are broadcasted.");
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.error("Failed to retrieve next event for provider " + getPrettyName() + ": " + ex.getMessage(), ex);
                            failedRecords = failedRecords + ((sifEvents != null) ? sifEvents.getListSize() : 0);
                        }
                    }
                    iterator.releaseResources();
                }
                else
                {
                    logger.info("getSIFEvents() for provider " + getPrettyName() + " returned null. Currently no events to be sent.");
                }
            }
            catch (Exception ex)
            {
                logger.error("Failed to retrieve events for provider " + getPrettyName() + ": " + ex.getMessage(), ex);
            }
            */
            log.Info("Total SIF Event Objects broadcasted: " + totalRecords);
            log.Info("Total SIF Event Objects failed     : " + failedRecords);
            log.Debug("================================ Finished broadcastEvents() for provider " + getServiceName());
            
        }
    }
}
