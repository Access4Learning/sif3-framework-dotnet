/*
 * Copyright 2019 Systemic Pty Ltd
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

using Sif.Framework.Demo.Au.Consumer.Consumers;
using Sif.Framework.Demo.Au.Consumer.Models;
using Sif.Framework.Utils;
using Sif.Specification.DataModel.Au;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Au.Consumer
{
    internal class FinancialQuestionnaireSubmissionConsumerApp
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private FinancialQuestionnaireSubmission CreateSubmission()
        {
            FinancialQuestionnaireSubmission submission = new FinancialQuestionnaireSubmission
            {
                FQYear = "2018",
                ReportingAuthority = "Ballarat Diocese",
                ReportingAuthoritySystem = "Vic Catholic",
                ReportingAuthorityCommonwealthId = "012345",
                SystemSubmission = AUCodeSetsYesOrNoCategoryType.N
            };

            return submission;
        }

        private void RunConsumer()
        {
            FinancialQuestionnaireSubmissionConsumer consumer = new FinancialQuestionnaireSubmissionConsumer(
                SettingsManager.ConsumerSettings.ApplicationKey,
                SettingsManager.ConsumerSettings.InstanceId,
                SettingsManager.ConsumerSettings.UserToken,
                SettingsManager.ConsumerSettings.SolutionId);
            consumer.Register();
            if (log.IsInfoEnabled) log.Info("Registered the Consumer.");

            try
            {
                // Create a new submission.
                try
                {
                    if (log.IsInfoEnabled) log.Info("*** Create a new submission.");
                    FinancialQuestionnaireSubmission createdObject = consumer.Create(CreateSubmission());
                    if (log.IsInfoEnabled) log.Info($"Created new submission with ID of {createdObject.RefId}.");
                }
                catch (UnauthorizedAccessException)
                {
                    if (log.IsInfoEnabled) log.Info("Access to create a new submission is rejected.");
                }

                // Retrieve all submissions.
                if (log.IsInfoEnabled) log.Info("*** Retrieve all submissions.");
                IEnumerable<FinancialQuestionnaireSubmission> retrievedObjects = consumer.Query(0, 10);

                foreach (FinancialQuestionnaireSubmission retrievedObject in retrievedObjects)
                {
                    if (log.IsInfoEnabled) log.Info($"Submission {retrievedObject.RefId} is for {retrievedObject.ReportingAuthority}.");
                }
            }
            catch (UnauthorizedAccessException)
            {
                if (log.IsInfoEnabled) log.Info($"Access to query submissions is rejected.");
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
            FinancialQuestionnaireSubmissionConsumerApp app = new FinancialQuestionnaireSubmissionConsumerApp();

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