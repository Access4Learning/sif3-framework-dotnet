/*
 * Crown Copyright © Department for Education (UK) 2016
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
using Newtonsoft.Json;
using Sif.Framework.Consumers;
using Sif.Framework.Demo.Uk.Consumer.Consumers;
using Sif.Framework.Demo.Uk.Consumer.Models;
using Sif.Framework.Demo.Uk.Consumer.Utils;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Query;
using Sif.Framework.Model.Responses;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Utils;
using Sif.Specification.DataModel.Uk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sif.Framework.Demo.Uk.Consumer
{

    class ConsumerApp
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Random random = new Random();

        private static LearnerPersonal CreateBruceWayne()
        {
            return new LearnerPersonal
            {
                LocalId = "555",
                PersonalInformation = new PersonalInformationType
                {
                    Name = new NameType
                    {
                        Type = NameTypeType.C,
                        FamilyName = "Wayne",
                        GivenName = "Bruce"
                    }
                }
            };
        }

        private static LearnerPersonal CreateLearner()
        {
            return new LearnerPersonal
            {
                LocalId = random.Next(10000, 99999).ToString(),
                PersonalInformation = new PersonalInformationType
                {
                    Name = new NameType
                    {
                        Type = NameTypeType.C,
                        FamilyName = RandomNameGenerator.FamilyName,
                        GivenName = RandomNameGenerator.GivenName
                    }
                }
            };
        }

        private static List<LearnerPersonal> CreateLearners(int count)
        {
            List<LearnerPersonal> learnerPersonalsCache = new List<LearnerPersonal>();

            for (int i = 1; i <= count; i++)
            {
                learnerPersonalsCache.Add(CreateLearner());
            }

            return learnerPersonalsCache;
        }

        private static IList<Guid> ProcessCreates(MultipleCreateResponse creates)
        {
            if(creates == null)
            {
                throw new ArgumentNullException("creates");
            }

            List<Guid> ids = new List<Guid>();

            foreach(CreateStatus s in creates.StatusRecords)
            {
                if(s.StatusCode.StartsWith("2"))
                {
                    log.Info("Created a job with id " + s.Id + " (" + s.StatusCode + ").");
                    ids.Add(Guid.Parse(s.Id));
                } else
                {
                    log.Info("Failed to create a job with id " + s.Id + " (" + s.StatusCode + "/" + s.Error.Code + ").\nDescription: " + s.Error.Description + "\nMessage: " + s.Error.Message);
                }
            }
            return ids;
        }

        private static void ProcessDeletes(MultipleDeleteResponse deletes)
        {
            if (deletes == null)
            {
                throw new ArgumentNullException("creates");
            }

            foreach (DeleteStatus d in deletes.StatusRecords)
            {
                if (d.StatusCode.StartsWith("2"))
                {
                    log.Info("Deleted a job with id " + d.Id + " (" + d.StatusCode + ").");
                }
                else
                {
                    log.Info("Failed to delete a job with id " + d.Id + " (" + d.StatusCode + "/" + d.Error.Code + ".\nDescription: " + d.Error.Description + "\nMessage: " + d.Error.Message);
                }
            }
        }


        void RunLearnerPersonalConsumer()
        {
            LearnerPersonalConsumer learnerPersonalConsumer = new LearnerPersonalConsumer("Sif3DemoApp");
            learnerPersonalConsumer.Register();
            if (log.IsInfoEnabled) log.Info("Registered the Consumer.");

            try
            {
                // Retrieve Bart Simpson using QBE.
                if (log.IsInfoEnabled) log.Info("*** Retrieve Bart Simpson using QBE.");
                LearnerPersonal exampleLearner = new LearnerPersonal {
                    PersonalInformation = new PersonalInformationType
                    {
                        Name = new NameType
                        {
                            FamilyName = "Simpson",
                            GivenName = "Bart"
                        }
                    }
                };
                IEnumerable<LearnerPersonal> filteredLearners = learnerPersonalConsumer.QueryByExample(exampleLearner);

                foreach (LearnerPersonal learner in filteredLearners)
                {
                    if (log.IsInfoEnabled) log.Info("Filtered learner name is " + learner.PersonalInformation.Name.GivenName + " " + learner.PersonalInformation.Name.FamilyName);
                }

                // Create a new learner.
                if (log.IsInfoEnabled) log.Info("*** Create a new learner.");
                LearnerPersonal newLearner = ConsumerApp.CreateBruceWayne();
                LearnerPersonal retrievedNewLearner = learnerPersonalConsumer.Create(newLearner);
                if (log.IsInfoEnabled) log.Info("Created new learner " + newLearner.PersonalInformation.Name.GivenName + " " + newLearner.PersonalInformation.Name.FamilyName);

                // Create multiple new learners.
                if (log.IsInfoEnabled) log.Info("*** Create multiple new learners.");
                List<LearnerPersonal> newLearners = CreateLearners(5);
                MultipleCreateResponse multipleCreateResponse = learnerPersonalConsumer.Create(newLearners);
                int count = 0;

                foreach (CreateStatus status in multipleCreateResponse.StatusRecords)
                {
                    if (log.IsInfoEnabled) log.Info("Create status code is " + status.StatusCode);
                    newLearners[count++].RefId = status.Id;
                }

                // Update multiple learners.
                if (log.IsInfoEnabled) log.Info("*** Update multiple learners.");
                foreach (LearnerPersonal learner in newLearners)
                {
                    learner.PersonalInformation.Name.GivenName += "o";
                }

                MultipleUpdateResponse multipleUpdateResponse = learnerPersonalConsumer.Update(newLearners);

                foreach (UpdateStatus status in multipleUpdateResponse.StatusRecords)
                {
                    if (log.IsInfoEnabled) log.Info("Update status code is " + status.StatusCode);
                }

                // Delete multiple learners.
                if (log.IsInfoEnabled) log.Info("*** Delete multiple learners.");
                ICollection<string> refIds = new List<string>();

                foreach (CreateStatus status in multipleCreateResponse.StatusRecords)
                {
                    refIds.Add(status.Id);
                }

                MultipleDeleteResponse multipleDeleteResponse = learnerPersonalConsumer.Delete(refIds);

                foreach (DeleteStatus status in multipleDeleteResponse.StatusRecords)
                {
                    if (log.IsInfoEnabled) log.Info("Delete status code is " + status.StatusCode);
                }

                // Retrieve all learners from zone "Gov" and context "Curr".
                if (log.IsInfoEnabled) log.Info("*** Retrieve all learners from zone \"Gov\" and context \"Curr\".");
                IEnumerable<LearnerPersonal> learners = learnerPersonalConsumer.Query(zone: "Gov", context: "Curr");

                foreach (LearnerPersonal learner in learners)
                {
                    if (log.IsInfoEnabled) log.Info("Learner name is " + learner.PersonalInformation.Name.GivenName + " " + learner.PersonalInformation.Name.FamilyName);
                }

                if (learners.Count() > 1)
                {

                    // Retrieve a single learner.
                    if (log.IsInfoEnabled) log.Info("*** Retrieve a single learner.");
                    string learnerId = learners.ElementAt(1).RefId;
                    LearnerPersonal secondLearner = learnerPersonalConsumer.Query(learnerId);
                    if (log.IsInfoEnabled) log.Info("Name of second learner is " + secondLearner.PersonalInformation.Name.GivenName + " " + secondLearner.PersonalInformation.Name.FamilyName);

                    // Update that learner and confirm.
                    if (log.IsInfoEnabled) log.Info("*** Update that learner and confirm.");
                    secondLearner.PersonalInformation.Name.GivenName = "Homer";
                    secondLearner.PersonalInformation.Name.FamilyName = "Simpson";
                    learnerPersonalConsumer.Update(secondLearner);
                    secondLearner = learnerPersonalConsumer.Query(learnerId);
                    if (log.IsInfoEnabled) log.Info("Name of second learner has been changed to " + secondLearner.PersonalInformation.Name.GivenName + " " + secondLearner.PersonalInformation.Name.FamilyName);

                    // Delete that learner and confirm.
                    if (log.IsInfoEnabled) log.Info("*** Delete that learner and confirm.");
                    learnerPersonalConsumer.Delete(learnerId);
                    LearnerPersonal deletedLearner = learnerPersonalConsumer.Query(learnerId);
                    bool learnerDeleted = (deletedLearner == null ? true : false);

                    if (learnerDeleted)
                    {
                        if (log.IsInfoEnabled) log.Info("Learner " + secondLearner.PersonalInformation.Name.GivenName + " " + secondLearner.PersonalInformation.Name.FamilyName + " was successfully deleted.");
                    }
                    else
                    {
                        if (log.IsInfoEnabled) log.Info("Learner " + secondLearner.PersonalInformation.Name.GivenName + " " + secondLearner.PersonalInformation.Name.FamilyName + " was NOT deleted.");
                    }

                }

                // Retrieve learners based on Teaching Group using Service Paths.
                if (log.IsInfoEnabled) log.Info("*** Retrieve learners based on Teaching Group using Service Paths.");
                EqualCondition condition = new EqualCondition() { Left = "TeachingGroups", Right = "597ad3fe-47e7-4b2c-b919-a93c564d19d0" };
                IList<EqualCondition> conditions = new List<EqualCondition>();
                conditions.Add(condition);
                IEnumerable<LearnerPersonal> teachingGroupLearners = learnerPersonalConsumer.QueryByServicePath(conditions);

                foreach (LearnerPersonal learner in teachingGroupLearners)
                {
                    if (log.IsInfoEnabled) log.Info("Learner name is " + learner.PersonalInformation.Name.GivenName + " " + learner.PersonalInformation.Name.FamilyName);
                }

            }
            catch (Exception e)
            {
                //if (log.IsInfoEnabled) log.Fatal(e.StackTrace);
                throw new Exception(this.GetType().FullName, e);
            }
            finally
            {
                learnerPersonalConsumer.Unregister();
                if (log.IsInfoEnabled) log.Info("Unregistered the Consumer.");
            }

        }

        void RunPayloadConsumer()
        {
            FunctionalServiceConsumer consumer = new FunctionalServiceConsumer("Sif3DemoApp");
            consumer.Register();
            if (log.IsInfoEnabled) log.Info("Registered the Consumer.");

            try
            {
                // Create a new payload job.
                if (log.IsInfoEnabled) log.Info("*** Create a job.");
                Job job = consumer.Create(new Job("Payload", "Testing"));
                if (log.IsInfoEnabled) log.Info("Created new job " + job.Name + " (" + job.Id + ")");

                // Query phase "default".
                if (log.IsInfoEnabled) log.Info("*** Check state of phase 'default', expecting NOTSTARTED.");
                job = consumer.Query(job);
                PhaseState state = job.Phases["default"].GetCurrentState();
                if (state.Type == PhaseStateType.NOTSTARTED)
                {
                    if (log.IsInfoEnabled) log.Info("Got EXPECTED result, last modified at " + state.LastModified);
                }
                else
                {
                    if (log.IsInfoEnabled) log.Info("Got UNEXPECTED result " + state.Type + ", last modified at " + state.LastModified);
                }

                // Execute CREATE to phase "default".
                if (log.IsInfoEnabled) log.Info("*** Executing CREATE to phase 'default'.");
                consumer.CreateToPhase(job, "default", "Sending CREATE", contentTypeOverride: "text/plain", acceptOverride: "text/plain");

                // Query phase "default".
                if (log.IsInfoEnabled) log.Info("*** Check state of phase 'default', expecting INPROGRESS.");
                job = consumer.Query(job);
                state = job.Phases["default"].GetCurrentState();
                if (state.Type == PhaseStateType.INPROGRESS)
                {
                    if (log.IsInfoEnabled) log.Info("Got EXPECTED result, last modified at " + state.LastModified);
                }
                else
                {
                    if (log.IsInfoEnabled) log.Info("Got UNEXPECTED result " + state.Type + ", last modified at " + state.LastModified);
                }

                // Execute UPDATE to phase "default".
                if (log.IsInfoEnabled) log.Info("*** Executing UPDATE to phase 'default'.");
                consumer.UpdateToPhase(job, "default", "Sending UPDATE", contentTypeOverride: "text/plain", acceptOverride: "text/plain");

                // Query phase "default".
                if (log.IsInfoEnabled) log.Info("*** Check state of phase 'default', expecting COMPLETE.");
                job = consumer.Query(job);
                state = job.Phases["default"].GetCurrentState();
                if (state.Type == PhaseStateType.COMPLETED)
                {
                    if (log.IsInfoEnabled) log.Info("Got EXPECTED result, last modified at " + state.LastModified);
                }
                else
                {
                    if (log.IsInfoEnabled) log.Info("Got UNEXPECTED result " + state.Type + ", last modified at " + state.LastModified);
                }

                // Execute DELETE to phase "default".
                if (log.IsInfoEnabled) log.Info("*** Executing DELETE to phase 'default'.");
                try {
                    consumer.DeleteToPhase(job, "default", "Sending DELETE", contentTypeOverride: "text/plain", acceptOverride: "text/plain");
                } catch (Exception e)
                {
                    if (log.IsInfoEnabled) log.Info("EXPECTED exception due to access rights: " + e.Message);
                }

                // Execute UPDATE to phase "xml".
                if (log.IsInfoEnabled) log.Info("*** Executing UPDATE to phase 'xml'.");
                string xml = "";
                try
                {
                    xml = SerialiserFactory.GetXmlSerialiser<LearnerPersonal>().Serialise(CreateBruceWayne());
                } catch (Exception e)
                {
                    if (log.IsFatalEnabled) log.Info("***** Error serializing to xml: " + e.Message, e);
                }
                consumer.UpdateToPhase(job, "xml", xml, contentTypeOverride: "application/xml", acceptOverride: "text/plain");

                // Execute UPDATE to phase "json".
                if (log.IsInfoEnabled) log.Info("*** Executing UPDATE to phase 'json'.");
                string json = "";
                try
                {
                    json = JsonConvert.SerializeObject(CreateBruceWayne());
                } catch (Exception e)
                {
                    if (log.IsFatalEnabled) log.Info("***** Error serializing to json: " + e.Message, e);
                }

                consumer.UpdateToPhase(job, "json", json, contentTypeOverride: "application/json", acceptOverride: "text/plain");

                // Change state of phase "json".
                if (log.IsInfoEnabled) log.Info("*** Executing CREATE to the state of phase 'json'.");
                state = consumer.CreateToState(job, "json", new PhaseState(PhaseStateType.FAILED, "Because I want it to"));
                if (state.Type == PhaseStateType.FAILED)
                {
                    if (log.IsInfoEnabled) log.Info("Got EXPECTED result, last modified at " + state.LastModified);
                }
                else
                {
                    if (log.IsInfoEnabled) log.Info("Got UNEXPECTED result " + state.Type + ", last modified at " + state.LastModified);
                }

                // Query phase "json".
                if (log.IsInfoEnabled) log.Info("*** Check state of phase 'json', expecting FAILED.");
                job = consumer.Query(job);
                state = job.Phases["json"].GetCurrentState();
                if (state.Type == PhaseStateType.FAILED)
                {
                    if (log.IsInfoEnabled) log.Info("Got EXPECTED result, last modified at " + state.LastModified);
                }
                else
                {
                    if (log.IsInfoEnabled) log.Info("Got UNEXPECTED result " + state.Type + ", last modified at " + state.LastModified);
                }

                // Delete the job.
                if (log.IsInfoEnabled) log.Info("*** Delete a job.");
                consumer.Delete(job);
                try {
                    Job deleted = consumer.Query(job);

                    if (log.IsInfoEnabled) log.Info("Shouldn't get here, provider should have caused an expection. Retrieving job with " + job.Id + " returned " + (job == null ? "null value" : "job object with ID " + deleted.Id) + ".");
                }
                catch (Exception e)
                {
                    if (log.IsInfoEnabled) log.Info("Job " + job.Id + " was successfully deleted. Provider responded with " + e.Message);
                }

                /*------------------------------------------*/
                /* Check multiple job creation and deletion */
                /*------------------------------------------*/
                List<Job> jobs = new List<Job>();
                for(int i = 0; i < 5; i++)
                {
                    jobs.Add(new Job("Payload"));
                }
                MultipleCreateResponse creates = consumer.Create(jobs);

                IList<Guid> ids = ProcessCreates(creates);
                jobs.Clear();
                foreach(Guid id in ids)
                {
                    jobs.Add(new Job("Payload") { Id = id });
                }

                // Change a random job's id, whihc will result in:
                // 1) The missing job will be removed by the timeout
                // 2) We expect a single job to have an error when being removed
                jobs.ElementAt(random.Next(0, jobs.Count)).Id = Guid.NewGuid();

                MultipleDeleteResponse deletes = consumer.Delete(jobs);
            }
            catch (Exception e)
            {
                log.Fatal(e.StackTrace);
                throw new Exception(this.GetType().FullName, e);
            }
            finally
            {
                consumer.Unregister();
                if (log.IsInfoEnabled) log.Info("Unregistered the Consumer.");
            }
        }

        private static Boolean RunDemo(string demoName)
        {
            Console.Write("\n\nWould you like run the " + demoName + " demo? (Y/n)");
            ConsoleKeyInfo info = new ConsoleKeyInfo();
            do
            {
                info = Console.ReadKey();
            }
            while (!(info.Key == ConsoleKey.N || info.Key == ConsoleKey.Y || info.Key == ConsoleKey.Enter));

            Console.Write("\n\n");

            return info.Key == ConsoleKey.Y || info.Key == ConsoleKey.Enter;
        }

        static void Main(string[] args)
        {
            ConsumerApp app = new ConsumerApp();

            if (RunDemo("Learner Personal (Object Service)"))
            {
                try
                {
                    app.RunLearnerPersonalConsumer();
                }
                catch (Exception e)
                {
                    if (log.IsErrorEnabled) log.Error("Error running the SIF3 demo.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
                }
            }
            if (RunDemo("Payload (Functional Service)"))
            {
                try
                {
                    app.RunPayloadConsumer();
                }
                catch (Exception e)
                {
                    if (log.IsErrorEnabled) log.Error("Error running the SIF3 demo.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
                }
            }

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

    }

}
