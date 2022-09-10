/*
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

using Sif.Framework.Demo.Au.Consumer.Consumers;
using Sif.Framework.Models.Settings;
using Sif.Framework.Services.Sessions;
using Sif.Framework.Utils;
using System;

namespace Sif.Framework.Demo.Au.Consumer
{
    internal class EventConsumerApp : ConsoleApp
    {
        private static readonly slf4net.ILogger Log =
            slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public static void Main()
        {
            Console.WriteLine();
            Console.WriteLine("********************************************************************************");
            Console.WriteLine();
            Console.WriteLine("To run this Event Consumer demo, the following steps are required.");
            Console.WriteLine();
            Console.WriteLine("  1) In the demo Consumer project:");
            Console.WriteLine("    1.1) Set the consumer.environment.template.applicationKey app setting to Sif3DemoEventConsumer.");
            Console.WriteLine("    1.2) Configure the consumer.environment.url app setting to reference the BROKERED environment endpoint - http://localhost:59586/api/environments/environment.");
            Console.WriteLine("    1.3) Configure the EventConsumerApp to be the Startup object for the project.");
            Console.WriteLine();
            Console.WriteLine("  2) In the demo Consumer project:");
            Console.WriteLine("    2.1) Set the provider.environmentType app setting to BROKERED.");
            Console.WriteLine("    2.2) Configure the provider.environment.url app setting to reference the BROKERED environment endpoint - http://localhost:59586/api/environments/environment.");
            Console.WriteLine("    2.3) This EventConsumerApp needs to be configured to be the Startup object for this project.");
            Console.WriteLine();
            Console.WriteLine("  3) Run the demo Broker instead of the EnvironmentProvider.");
            Console.WriteLine();
            Console.WriteLine("  4) Run the demo Provider.");
            Console.WriteLine();
            Console.WriteLine("  5) Run the demo Consumer.");
            Console.WriteLine();
            Console.WriteLine("********************************************************************************");

            try
            {
                SettingsSource source = SelectFrameworkConfigSource();
                IFrameworkSettings settings = GetSettings(source);
                ISessionService sessionService = GetSessionService(source);
                var consumer = new StudentPersonalEventConsumer(
                    settings.ApplicationKey,
                    settings.InstanceId,
                    settings.UserToken,
                    settings.SolutionId,
                    settings,
                    sessionService);
                consumer.Start("Sif3DemoZone1", "DEFAULT");

                if (Log.IsInfoEnabled) Log.Info("Started the Event Consumer.");

                Console.WriteLine(
                    "Press any key to stop the Event Consumer (may take several seconds to complete) ...");
                Console.ReadKey();

                consumer.Stop();

                if (Log.IsInfoEnabled) Log.Info("Stopped the Event Consumer.");
            }
            catch (Exception e)
            {
                if (Log.IsErrorEnabled)
                    Log.Error($"Error running the Event Consumer.\n{ExceptionUtils.InferErrorResponseMessage(e)}", e);
            }

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }
    }
}