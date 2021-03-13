/*
 * Copyright 2021 Systemic Pty Ltd
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
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Sessions;
using Sif.Framework.Utils;
using Sif.Specification.DataModel.Au;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Au.Consumer
{
    internal class EnrollmentConsumerApp : ConsoleApp
    {
        private static readonly slf4net.ILogger Log =
            slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static void RunConsumer(IFrameworkSettings settings, ISessionService sessionService)
        {
            var consumer = new StudentSchoolEnrollmentConsumer(
                settings.ApplicationKey,
                settings.InstanceId,
                settings.UserToken,
                settings.SolutionId,
                settings,
                sessionService);
            consumer.Register();

            if (Log.IsInfoEnabled) Log.Info("Registered the Consumer.");

            try
            {
                // Retrieve object using QBE.

                if (Log.IsInfoEnabled) Log.Info("*** Retrieve object using QBE.");

                var enrollmentExample = new StudentSchoolEnrollment
                {
                    YearLevel = new YearLevelType

                    {
                        Code = AUCodeSetsYearLevelCodeType.Item10
                    }
                };

                IEnumerable<StudentSchoolEnrollment> filteredEnrollments = consumer.QueryByExample(enrollmentExample);

                foreach (StudentSchoolEnrollment enrollment in filteredEnrollments)
                {
                    if (Log.IsInfoEnabled) Log.Info($"Filtered year level is {enrollment?.YearLevel.Code}");
                }
            }
            catch (UnauthorizedAccessException)
            {
                if (Log.IsInfoEnabled) Log.Info("Access to query objects is rejected.");
            }
            catch (Exception e)
            {
                if (Log.IsErrorEnabled)
                    Log.Error($"Error running the Consumer.\n{ExceptionUtils.InferErrorResponseMessage(e)}", e);
            }
            finally
            {
                consumer.Unregister();
                if (Log.IsInfoEnabled) Log.Info("Unregistered the Consumer.");
            }
        }

        public static void Main()
        {
            try
            {
                SettingsSource source = SelectSettingsSource();
                RunConsumer(GetSettings(source), GetSessionService(source));
            }
            catch (Exception e)
            {
                if (Log.IsErrorEnabled)
                    Log.Error($"Error running the Consumer.\n{ExceptionUtils.InferErrorResponseMessage(e)}", e);
            }

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }
    }
}