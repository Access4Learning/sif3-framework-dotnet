/*
 * Copyright 2018 Systemic Pty Ltd
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

using Sif.Framework.Demo.Hits.Consumer.Consumers;
using Sif.Framework.Demo.Hits.Consumer.Models;
using Sif.Framework.Utils;
using Sif.Specification.DataModel.Au;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Hits.Consumer
{
    internal class FQReportingConsumerApp
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private FQReporting CreateFQReporting()
        {
            FQReporting fqReporting = new FQReporting
            {
                FQYear = "2018",
                ReportingAuthority = "Ballarat Diocese",
                ReportingAuthoritySystem = "Vic Catholic",
                ReportingAuthorityCommonwealthId = "012345",
                SystemSubmission = AUCodeSetsYesOrNoCategoryType.N,
                EntityLevel = "School",
                LocalId = "01011234",
                StateProvinceId = "45645567",
                CommonwealthId = "12387",
                ACARAId = "99007",
                EntityName = "XXX Secondary College"
            };

            return fqReporting;
        }

        private void RunConsumer()
        {
            FQReportingConsumer consumer = new FQReportingConsumer(
                SettingsManager.ConsumerSettings.ApplicationKey,
                SettingsManager.ConsumerSettings.InstanceId,
                SettingsManager.ConsumerSettings.UserToken,
                SettingsManager.ConsumerSettings.SolutionId);
            consumer.Register();
            if (log.IsInfoEnabled) log.Info("Registered the Consumer.");

            try
            {
                // Create a new FQ reporting object.
                try
                {
                    if (log.IsInfoEnabled) log.Info("*** Create a new FQ reporting object.");
                    FQReporting createdObject = consumer.Create(CreateFQReporting());
                    if (log.IsInfoEnabled) log.Info($"Created new FQ reporting object with ID of {createdObject.RefId}.");
                }
                catch (UnauthorizedAccessException)
                {
                    if (log.IsInfoEnabled) log.Info("Access to create a new FQ reporting object is rejected.");
                }

                // Retrieve all FQ reporting objects.
                if (log.IsInfoEnabled) log.Info("*** Retrieve all FQ reporting objects.");
                IEnumerable<FQReporting> retrievedObjects = consumer.Query(1, 2);

                foreach (FQReporting retrievedObject in retrievedObjects)
                {
                    if (log.IsInfoEnabled) log.Info($"FQ reporting object {retrievedObject.RefId} is for {retrievedObject.EntityName}.");
                }
            }
            catch (UnauthorizedAccessException)
            {
                if (log.IsInfoEnabled) log.Info($"Access to query FQ reporting objects is rejected.");
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled) log.Error("Error running the Consumer.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
            }
            finally
            {
                consumer.Unregister();
                if (log.IsInfoEnabled) log.Info("Unregistered the Consumer.");
            }
        }

        private static void Main(string[] args)
        {
            FQReportingConsumerApp app = new FQReportingConsumerApp();

            try
            {
                app.RunConsumer();
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled) log.Error("Error running the Consumer.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
            }

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }
    }
}