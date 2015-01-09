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
using Sbp.Framework.Service.Consumer;
using Sif.Framework.Demo.Au.DataModel;
using Sif.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sif.Framework.Demo.Au.Consumer
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
        void RunSchoolInfoConsumer()
        {
            ISchoolInfoConsumer schoolInfoConsumer = new SchoolInfoConsumer("Sif3DemoApp");
            schoolInfoConsumer.Register();

            try
            {
                ICollection<Sbp.Framework.Model.SchoolInfo> schools = schoolInfoConsumer.Retrieve();
            }
            finally
            {
                schoolInfoConsumer.Unregister();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        void RunStudentPersonalConsumer()
        {
            IStudentPersonalConsumer studentPersonalConsumer = new StudentPersonalConsumer("Sif3DemoApp");
            studentPersonalConsumer.Register();
            if (log.IsInfoEnabled) log.Info("Registered the Consumer.");

            try
            {
                // Retrieve all students.
                ICollection<StudentPersonal> students = studentPersonalConsumer.Retrieve();

                foreach (StudentPersonal student in students)
                {
                    if (log.IsInfoEnabled) log.Info("Student name is " + student.PersonInfo.Name.GivenName + " " + student.PersonInfo.Name.FamilyName);
                }

                // Retrieve a single student.
                Guid studentId = students.ElementAt(0).Id;
                StudentPersonal firstStudent = studentPersonalConsumer.Retrieve(studentId);
                if (log.IsInfoEnabled) log.Info("Name of first student is " + firstStudent.PersonInfo.Name.GivenName + " " + firstStudent.PersonInfo.Name.FamilyName);

                // Update that student and confirm.
                firstStudent.PersonInfo.Name.GivenName = "Homer";
                firstStudent.PersonInfo.Name.FamilyName = "Simpson";
                studentPersonalConsumer.Update(firstStudent);
                firstStudent = studentPersonalConsumer.Retrieve(studentId);
                if (log.IsInfoEnabled) log.Info("Name of first student has been changed to " + firstStudent.PersonInfo.Name.GivenName + " " + firstStudent.PersonInfo.Name.FamilyName);

                // Delete that student and confirm.
                studentPersonalConsumer.Delete(studentId);
                students = studentPersonalConsumer.Retrieve();
                bool studentDeleted = true;

                foreach (StudentPersonal student in students)
                {

                    if (studentId == student.Id)
                    {
                        studentDeleted = false;
                        break;
                    }

                }

                if (studentDeleted)
                {
                    if (log.IsInfoEnabled) log.Info("Student " + firstStudent.PersonInfo.Name.GivenName + " " + firstStudent.PersonInfo.Name.FamilyName + " was successfully deleted.");
                }
                else
                {
                    if (log.IsInfoEnabled) log.Info("Student " + firstStudent.PersonInfo.Name.GivenName + " " + firstStudent.PersonInfo.Name.FamilyName + " was NOT deleted.");
                }

            }
            finally
            {
                studentPersonalConsumer.Unregister();
                if (log.IsInfoEnabled) log.Info("Unregistered the Consumer.");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ConsumerApp app = new ConsumerApp();

            try
            {
                app.RunStudentPersonalConsumer();
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled) log.Error("Error running the StudentPersonal Consumer.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
            }
            
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

    }

}
