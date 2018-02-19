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

using Sif.Framework.Demo.Us.Consumer.Consumers;
using Sif.Framework.Demo.Us.Consumer.Models;
using Sif.Framework.Utils;
using Sif.Specification.DataModel.Us;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sif.Framework.Demo.Us.Consumer
{

    class ConsumerApp
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        void RunStudentConsumer()
        {
            XStudentConsumer studentConsumer = new XStudentConsumer(SettingsManager.ConsumerSettings.ApplicationKey);
            studentConsumer.Register();
            if (log.IsInfoEnabled) log.Info("Registered the Consumer.");

            try
            {
                // Retrieve all students.
                ICollection<xStudent> students = studentConsumer.Query();

                foreach (xStudent student in students)
                {
                    if (log.IsInfoEnabled) log.Info("Student name is " + student.name.givenName + " " + student.name.familyName);
                }

                // Retrieve a single student.
                string studentId = students.ElementAt(0).RefId;
                xStudent firstStudent = studentConsumer.Query(studentId);
                if (log.IsInfoEnabled) log.Info("Name of first student is " + firstStudent.name.givenName + " " + firstStudent.name.familyName);

                // Create and then retrieve a new student.
                xPersonNameType newStudentName = new xPersonNameType() { familyName = "Wayne", givenName = "Bruce" };
                xStudent newStudent = new xStudent() { localId = "555", name = newStudentName };
                xStudent retrievedNewStudent = studentConsumer.Create(newStudent);
                if (log.IsInfoEnabled) log.Info("Created new student " + newStudent.name.givenName + " " + newStudent.name.familyName);

                // Update that student and confirm.
                firstStudent.name.givenName = "Homer";
                firstStudent.name.familyName = "Simpson";
                studentConsumer.Update(firstStudent);
                firstStudent = studentConsumer.Query(studentId);
                if (log.IsInfoEnabled) log.Info("Name of first student has been changed to " + firstStudent.name.givenName + " " + firstStudent.name.familyName);

                // Delete that student and confirm.
                studentConsumer.Delete(studentId);
                xStudent deletedStudent = studentConsumer.Query(studentId);
                bool studentDeleted = (deletedStudent == null ? true : false);

                if (studentDeleted)
                {
                    if (log.IsInfoEnabled) log.Info("Student " + firstStudent.name.givenName + " " + firstStudent.name.familyName + " was successfully deleted.");
                }
                else
                {
                    if (log.IsInfoEnabled) log.Info("Student " + firstStudent.name.givenName + " " + firstStudent.name.familyName + " was NOT deleted.");
                }

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled) log.Error("Error running the Student Consumer.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
            }
            finally
            {
                studentConsumer.Unregister();
                if (log.IsInfoEnabled) log.Info("Unregistered the Consumer.");
            }

        }

        static void Main(string[] args)
        {
            ConsumerApp app = new ConsumerApp();

            try
            {
                app.RunStudentConsumer();
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled) log.Error("Error running the ConsumerApp.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
            }

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

    }

}
