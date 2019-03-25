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

using Sif.Framework.Demo.Au.Consumer.Consumers;
using Sif.Framework.Utils;
using System;

namespace Sif.Framework.Demo.Au.Consumer
{
    internal class EventConsumerApp
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static bool RunDemo(string demoName)
        {
            Console.WriteLine();
            Console.Write("Would you like run the " + demoName + " demo (Y/N)? - ");
            ConsoleKeyInfo info = new ConsoleKeyInfo();

            do
            {
                info = Console.ReadKey();
            }
            while (info.Key != ConsoleKey.N && info.Key != ConsoleKey.Y && info.Key != ConsoleKey.Enter);

            Console.WriteLine();
            Console.WriteLine();

            return (info.Key == ConsoleKey.Y);
        }

        private static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("********************************************************************************");
            Console.WriteLine();
            Console.WriteLine("To run the following Event Consumer demo, the following is required:");
            Console.WriteLine();
            Console.WriteLine("  1) The consumer.environment.url app setting in the SifFramework.config file needs to reference the BROKERED environment endpoint.");
            Console.WriteLine("  2) The Sif.Framework.Demo.Broker needs to be run instead of the Sif.Framework.EnvironmentProvider.");
            Console.WriteLine();
            Console.WriteLine("********************************************************************************");

            if (RunDemo("Student Personal Event Consumer"))
            {
                try
                {
                    StudentPersonalEventConsumer studentPersonalConsumer = new StudentPersonalEventConsumer(
                        SettingsManager.ConsumerSettings.ApplicationKey,
                        SettingsManager.ConsumerSettings.InstanceId,
                        SettingsManager.ConsumerSettings.UserToken,
                        SettingsManager.ConsumerSettings.SolutionId);
                    studentPersonalConsumer.Start("Sif3DemoZone1", "DEFAULT");
                    if (log.IsInfoEnabled) log.Info("Started the Event Consumer.");

                    Console.WriteLine("Press any key to stop the Event Consumer (may take several seconds to complete) ...");
                    Console.ReadKey();

                    studentPersonalConsumer.Stop();
                    if (log.IsInfoEnabled) log.Info("Stopped the Event Consumer.");
                }
                catch (Exception e)
                {
                    if (log.IsErrorEnabled) log.Error("Error running the Student Personal Event Consumer.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
                }
            }

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }
    }
}