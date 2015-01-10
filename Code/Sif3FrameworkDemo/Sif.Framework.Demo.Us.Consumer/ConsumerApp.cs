/*
 * Copyright 2015 Systemic Pty Ltd
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

using log4net;
using Sif.Framework.Demo.Us.Consumer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sif.Framework.Demo.Us.Consumer
{

    /// <summary>
    /// 
    /// </summary>
    class ConsumerApp
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 
        /// </summary>
        void RunK12StudentConsumer()
        {
            IK12StudentConsumer studentPersonalConsumer = new K12StudentConsumer("Sif3UsDemoApp");
            studentPersonalConsumer.Register();

            try
            {
                // Retrieve all students.
                ICollection<K12Student> students = studentPersonalConsumer.Retrieve();

                foreach (K12Student student in students)
                {
                    if (log.IsInfoEnabled) log.Info("Student name is " + student.identity.name.firstName + " " + student.identity.name.lastName);
                }

                // Retrieve a single student.
                string studentId = students.ElementAt(0).Id;
                K12Student firstStudent = studentPersonalConsumer.Retrieve(studentId);
                if (log.IsInfoEnabled) log.Info("Name of first student is " + firstStudent.identity.name.firstName + " " + firstStudent.identity.name.lastName);

                // Update that student and confirm.
                firstStudent.identity.name.firstName = "Homer";
                firstStudent.identity.name.lastName = "Simpson";
                studentPersonalConsumer.Update(firstStudent);
                firstStudent = studentPersonalConsumer.Retrieve(studentId);
                if (log.IsInfoEnabled) log.Info("Name of first student has been changed to " + firstStudent.identity.name.firstName + " " + firstStudent.identity.name.lastName);

                // Delete that student and confirm.
                studentPersonalConsumer.Delete(studentId);
                students = studentPersonalConsumer.Retrieve();
                bool studentDeleted = true;

                foreach (K12Student student in students)
                {

                    if (studentId == student.Id)
                    {
                        studentDeleted = false;
                        break;
                    }

                }

                if (studentDeleted)
                {
                    if (log.IsInfoEnabled) log.Info("Student " + firstStudent.identity.name.firstName + " " + firstStudent.identity.name.lastName + " was successfully deleted.");
                }
                else
                {
                    if (log.IsInfoEnabled) log.Info("Student " + firstStudent.identity.name.firstName + " " + firstStudent.identity.name.lastName + " was NOT deleted.");
                }

            }
            finally
            {
                studentPersonalConsumer.Unregister();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ConsumerApp app = new ConsumerApp();
            app.RunK12StudentConsumer();
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

    }

}
