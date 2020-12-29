/*
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

using Sif.Framework.Demo.Au.Consumer.Consumers;
using Sif.Framework.Demo.Au.Consumer.Models;
using Sif.Framework.Utils;
using Sif.Specification.DataModel.Au;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Au.Consumer
{
    internal class EnrollmentConsumerApp
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private void RunConsumer()
        {
            StudentSchoolEnrollmentConsumer consumer = new StudentSchoolEnrollmentConsumer(
                SettingsManager.ConsumerSettings.ApplicationKey,
                SettingsManager.ConsumerSettings.InstanceId,
                SettingsManager.ConsumerSettings.UserToken,
                SettingsManager.ConsumerSettings.SolutionId,
                SettingsManager.ConsumerSettings);
            consumer.Register();
            if (log.IsInfoEnabled) log.Info("Registered the Consumer.");

            try
            {
                // Retrieve object using QBE.
                if (log.IsInfoEnabled) log.Info("*** Retrieve object using QBE.");
                StudentSchoolEnrollment enrollmentExample = new StudentSchoolEnrollment
                {
                    YearLevel = new YearLevelType

                    {
                        Code = AUCodeSetsYearLevelCodeType.Item10
                    }
                };

                IEnumerable<StudentSchoolEnrollment> filteredEnrollments = consumer.QueryByExample(enrollmentExample);

                foreach (StudentSchoolEnrollment enrollment in filteredEnrollments)
                {
                    if (log.IsInfoEnabled) log.Info($"Filtered year level is {enrollment?.YearLevel.Code}");
                }
            }
            catch (UnauthorizedAccessException)
            {
                if (log.IsInfoEnabled) log.Info($"Access to query objects is rejected.");
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
            EnrollmentConsumerApp app = new EnrollmentConsumerApp();

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