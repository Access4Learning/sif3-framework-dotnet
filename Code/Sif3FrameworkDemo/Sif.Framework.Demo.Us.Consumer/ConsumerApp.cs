/*
 * Copyright 2016 Systemic Pty Ltd
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
using Sif.Framework.Utils;
using Sif.Specification.DataModel.Us;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sif.Framework.Demo.Us.Consumer
{

    class ConsumerApp
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        void RunStudentConsumer()
        {
            XStudentConsumer studentConsumer = new XStudentConsumer("Sif3UsDemoApp");
            studentConsumer.Register();
            if (log.IsInfoEnabled) log.Info("Registered the Consumer.");

            try
            {
                // Retrieve all students.
                ICollection<XStudent> students = studentConsumer.Query();

                foreach (XStudent student in students)
                {
                    if (log.IsInfoEnabled) log.Info("Student name is " + student.name.givenName + " " + student.name.familyName);
                }

                // Retrieve a single student.
                string studentId = students.ElementAt(0).RefId;
                XStudent firstStudent = studentConsumer.Query(studentId);
                if (log.IsInfoEnabled) log.Info("Name of first student is " + firstStudent.name.givenName + " " + firstStudent.name.familyName);

                // Create and then retrieve a new student.
                xPersonNameType newStudentName = new xPersonNameType() { familyName = "Wayne", givenName = "Bruce" };
                XStudent newStudent = new XStudent() { localId = "555", name = newStudentName };
                XStudent retrievedNewStudent = studentConsumer.Create(newStudent);
                if (log.IsInfoEnabled) log.Info("Created new student " + newStudent.name.givenName + " " + newStudent.name.familyName);

                // Update that student and confirm.
                firstStudent.name.givenName = "Homer";
                firstStudent.name.familyName = "Simpson";
                studentConsumer.Update(firstStudent);
                firstStudent = studentConsumer.Query(studentId);
                if (log.IsInfoEnabled) log.Info("Name of first student has been changed to " + firstStudent.name.givenName + " " + firstStudent.name.familyName);

                // Delete that student and confirm.
                studentConsumer.Delete(studentId);
                XStudent deletedStudent = studentConsumer.Query(studentId);
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
            catch (Exception)
            {
                throw;
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
                if (log.IsErrorEnabled) log.Error("Error running the student Consumer.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
            }

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

    }

}
