/*
 * Copyright 2017 Systemic Pty Ltd
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
using Sif.Framework.Model.Query;
using Sif.Framework.Model.Responses;
using Sif.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sif.Framework.Demo.Hits.Consumer
{

    class ConsumerApp
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<SchoolInfo> CreateSchools()
        {
            SchoolInfo applecrossHigh = new SchoolInfo { SchoolName = "Applecross SHS" };
            SchoolInfo rossmoyneHigh = new SchoolInfo { SchoolName = "Rossmoyne SHS" };
            List<SchoolInfo> schoolCollection = new List<SchoolInfo> { applecrossHigh, rossmoyneHigh };

            return schoolCollection;
        }

        void RunSchoolInfoConsumer()
        {
            SchoolInfoConsumer schoolInfoConsumer = new SchoolInfoConsumer(SettingsManager.ConsumerSettings.ApplicationKey, SettingsManager.ConsumerSettings.InstanceId, SettingsManager.ConsumerSettings.UserToken, SettingsManager.ConsumerSettings.SolutionId);
            schoolInfoConsumer.Register();

            try
            {
                // Query all schools.
                IEnumerable<SchoolInfo> allSchools = schoolInfoConsumer.Query(1, 10);

                if (log.IsInfoEnabled) log.Info("School count is " + (allSchools == null ? 0 : allSchools.Count()));

                if (allSchools != null)
                {

                    foreach (SchoolInfo school in allSchools)
                    {
                        if (log.IsInfoEnabled) log.Info("School " + school.SchoolName + " has a RefId of " + school.RefId + ".");
                    }

                }

                // Create multiple schools.
                MultipleCreateResponse createResponse = schoolInfoConsumer.Create(CreateSchools());

                foreach (CreateStatus status in createResponse.StatusRecords)
                {
                    SchoolInfo school = schoolInfoConsumer.Query(status.Id);
                    if (log.IsInfoEnabled) log.Info("New school " + school.SchoolName + " has a RefId of " + school.RefId + ".");
                }

                // Update multiple schools.
                List<SchoolInfo> schoolsToUpdate = new List<SchoolInfo>();

                foreach (CreateStatus status in createResponse.StatusRecords)
                {
                    SchoolInfo school = schoolInfoConsumer.Query(status.Id);
                    school.SchoolName += "x";
                    schoolsToUpdate.Add(school);
                }

                MultipleUpdateResponse updateResponse = schoolInfoConsumer.Update(schoolsToUpdate);

                foreach (UpdateStatus status in updateResponse.StatusRecords)
                {
                    SchoolInfo school = schoolInfoConsumer.Query(status.Id);
                    if (log.IsInfoEnabled) log.Info("Updated school " + school.SchoolName + " has a RefId of " + school.RefId + ".");
                }

                // Delete multiple schools.
                ICollection<string> schoolsToDelete = new List<string>();

                foreach (CreateStatus status in createResponse.StatusRecords)
                {
                    schoolsToDelete.Add(status.Id);
                }

                MultipleDeleteResponse deleteResponse = schoolInfoConsumer.Delete(schoolsToDelete);

                foreach (DeleteStatus status in deleteResponse.StatusRecords)
                {
                    SchoolInfo school = schoolInfoConsumer.Query(status.Id);

                    if (school == null)
                    {
                        if (log.IsInfoEnabled) log.Info("School with RefId of " + status.Id + " has been deleted.");
                    }
                    else
                    {
                        if (log.IsInfoEnabled) log.Info("School " + school.SchoolName + " with RefId of " + school.RefId + " FAILED deletion.");
                    }

                }

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled) log.Error("Error running the SchoolInfo Consumer against HITS.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
            }
            finally
            {
                schoolInfoConsumer.Unregister();
            }

        }

        void RunStaffPersonalConsumer()
        {
            StaffPersonalConsumer staffPersonalConsumer = new StaffPersonalConsumer(SettingsManager.ConsumerSettings.ApplicationKey, SettingsManager.ConsumerSettings.InstanceId, SettingsManager.ConsumerSettings.UserToken, SettingsManager.ConsumerSettings.SolutionId);
            staffPersonalConsumer.Register();

            try
            {
                EqualCondition condition = new EqualCondition() { Left = "TeachingGroups", Right = "597ad3fe-47e7-4b2c-b919-a93c564d19d0" };
                IList<EqualCondition> conditions = new List<EqualCondition>();
                conditions.Add(condition);
                IEnumerable<StaffPersonal> staffPersonals = staffPersonalConsumer.QueryByServicePath(conditions);

                if (log.IsInfoEnabled) log.Info("Staff count is " + (staffPersonals == null ? 0 : staffPersonals.Count()));

                if (staffPersonals != null)
                {

                    foreach (StaffPersonal staffPersonal in staffPersonals)
                    {
                        if (log.IsInfoEnabled) log.Info("Staff name is " + staffPersonal.PersonInfo.Name.GivenName + " " + staffPersonal.PersonInfo.Name.FamilyName);
                    }

                }

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled) log.Error("Error running the StaffPersonal Consumer against HITS.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
            }
            finally
            {
                staffPersonalConsumer.Unregister();
            }

        }

        void RunStudentContactRelationshipConsumer()
        {
            StudentContactRelationshipConsumer consumer = new StudentContactRelationshipConsumer(SettingsManager.ConsumerSettings.ApplicationKey, SettingsManager.ConsumerSettings.InstanceId, SettingsManager.ConsumerSettings.UserToken, SettingsManager.ConsumerSettings.SolutionId);
            consumer.Register();

            try
            {
                IEnumerable<StudentContactRelationship> relationships = consumer.Query(1, 2);

                if (log.IsInfoEnabled) log.Info($"Student contact relationship count is {(relationships == null ? 0 : relationships.Count())}.");

                if (relationships != null)
                {

                    foreach (StudentContactRelationship relationship in relationships)
                    {
                        if (log.IsInfoEnabled) log.Info($"Student {relationship.StudentPersonalRefId} has {relationship.StudentContactPersonalRefId} as a registered contact.");
                    }

                }

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled) log.Error("Error running the StudentContactRelationship Consumer against HITS.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
            }
            finally
            {
                consumer.Unregister();
            }

        }

        void RunStudentPersonalConsumer()
        {
            StudentPersonalConsumer studentPersonalConsumer = new StudentPersonalConsumer(SettingsManager.ConsumerSettings.ApplicationKey, SettingsManager.ConsumerSettings.InstanceId, SettingsManager.ConsumerSettings.UserToken, SettingsManager.ConsumerSettings.SolutionId);
            studentPersonalConsumer.Register();

            try
            {
                IEnumerable<StudentPersonal> studentPersonals = studentPersonalConsumer.Query(1, 2);

                if (log.IsInfoEnabled) log.Info("Student count is " + (studentPersonals == null ? 0 : studentPersonals.Count()));

                if (studentPersonals != null)
                {

                    foreach (StudentPersonal studentPersonal in studentPersonals)
                    {
                        if (log.IsInfoEnabled) log.Info("Student name is " + studentPersonal.PersonInfo.Name.GivenName + " " + studentPersonal.PersonInfo.Name.FamilyName);
                    }

                }

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled) log.Error("Error running the StudentPersonal Consumer against HITS.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
            }
            finally
            {
                studentPersonalConsumer.Unregister();
            }

        }

        void RunWellbeingCharacteristicConsumer()
        {
            WellbeingCharacteristicConsumer consumer = new WellbeingCharacteristicConsumer(SettingsManager.ConsumerSettings.ApplicationKey, SettingsManager.ConsumerSettings.InstanceId, SettingsManager.ConsumerSettings.UserToken, SettingsManager.ConsumerSettings.SolutionId);
            consumer.Register();

            try
            {
                IEnumerable<WellbeingCharacteristic> characteristics = consumer.Query(1, 2);

                if (log.IsInfoEnabled) log.Info($"Wellbeing characteristic count is {(characteristics == null ? 0 : characteristics.Count())}.");

                if (characteristics != null)
                {

                    foreach (WellbeingCharacteristic characteristic in characteristics)
                    {
                        if (log.IsInfoEnabled) log.Info($"Wellbeing characteristic is for student {characteristic.StudentPersonalRefId}.");
                    }

                }

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled) log.Error("Error running the WellbeingCharacteristic Consumer against HITS.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
            }
            finally
            {
                consumer.Unregister();
            }

        }

        static void Main(string[] args)
        {
            ConsumerApp app = new ConsumerApp();

            try
            {
                app.RunSchoolInfoConsumer();
                app.RunStaffPersonalConsumer();
                app.RunStudentContactRelationshipConsumer();
                app.RunStudentPersonalConsumer();
                app.RunWellbeingCharacteristicConsumer();
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
