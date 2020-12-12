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

using Sif.Framework.Demo.Broker.Models;
using Sif.Framework.Demo.Broker.Services;
using Sif.Framework.Demo.Broker.Utils;
using Sif.Framework.Model.Exceptions;
using Sif.Framework.Providers;
using Sif.Framework.Utils;
using Sif.Framework.WebApi.ActionResults;
using Sif.Framework.WebApi.ModelBinders;
using Sif.Specification.DataModel.Au;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Sif.Framework.Demo.Broker.Controllers
{
    public class QueuesProvider : BasicProvider<Queue>
    {
        private static readonly string[] EventActionType = { "CREATE", "DELETE", "UPDATE" };
        private static readonly Random Random = new Random();
        private static readonly string[] ReplacementType = { "FULL", "PARTIAL" };

        private static int availableMessageBatches = 5;

        public QueuesProvider() : base(new QueueService(), SettingsManager.ProviderSettings)
        {
        }

        [NonAction]
        public override IHttpActionResult BroadcastEvents(string zoneId = null, string contextId = null)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        /// <summary>
        /// Add a custom header to an action result and return it.
        /// </summary>
        /// <param name="result">Action result.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValue">Value associated with the header.</param>
        /// <returns>Action result with a custom header.</returns>
        private static IHttpActionResult CreateCustomActionResult(
            IHttpActionResult result,
            string headerName,
            string headerValue)
        {
            return new CustomHeaderResult(result, headerName, new[] { headerValue });
        }

        private static StudentPersonal CreateStudent()
        {
            var name = new NameOfRecordType
            {
                Type = NameOfRecordTypeType.LGL,
                FamilyName = RandomNameGenerator.FamilyName,
                GivenName = RandomNameGenerator.GivenName
            };

            var personInfo = new PersonInfoType { Name = name };

            var studentPersonal = new StudentPersonal
            {
                RefId = Guid.NewGuid().ToString(),
                LocalId = Random.Next(10000, 99999).ToString(),
                PersonInfo = personInfo
            };

            return studentPersonal;
        }

        private static List<StudentPersonal> CreateStudents(int count)
        {
            var students = new List<StudentPersonal>();

            for (var i = 0; i < count; i++)
            {
                StudentPersonal studentPersonal = CreateStudent();
                students.Add(studentPersonal);
            }

            return students;
        }

        public override IHttpActionResult Delete(
            deleteRequestType deleteRequest,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        public override IHttpActionResult Get(
            string object1,
            [FromUri(Name = "id1")] string refId1,
            string object2 = null,
            [FromUri(Name = "id2")] string refId2 = null,
            string object3 = null,
            [FromUri(Name = "id3")] string refId3 = null,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [Route("~/api/Queues/{queueId}/messages")]
        public IHttpActionResult Get(string queueId)
        {
            if (availableMessageBatches == 0)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            availableMessageBatches--;
            List<StudentPersonal> students = CreateStudents(Random.Next(1, 5));
            IHttpActionResult result = Ok(students);
            string eventActionValue = EventActionType[Random.Next(EventActionType.Length)];
            result = CreateCustomActionResult(result, "eventAction", eventActionValue);
            result = CreateCustomActionResult(result, "messageId", Guid.NewGuid().ToString());
            result = CreateCustomActionResult(result, "minWaitTime", "10");

            if ("UPDATE".Equals(eventActionValue))
            {
                string replacementValue = ReplacementType[Random.Next(ReplacementType.Length)];
                result = CreateCustomActionResult(result, "Replacement", replacementValue);
            }

            return result;
        }

        [Route("~/api/Queues/{queueId}/messages;deleteMessageId={deleteMessageId}")]
        public IHttpActionResult Get(string queueId, string deleteMessageId)
        {
            if (availableMessageBatches == 0)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            availableMessageBatches--;
            List<StudentPersonal> students = CreateStudents(Random.Next(1, 5));
            IHttpActionResult result = Ok(students);
            string eventActionValue = EventActionType[Random.Next(EventActionType.Length)];
            result = CreateCustomActionResult(result, "eventAction", eventActionValue);
            result = CreateCustomActionResult(result, "messageId", Guid.NewGuid().ToString());
            result = CreateCustomActionResult(result, "minWaitTime", "10");

            if ("UPDATE".Equals(eventActionValue))
            {
                string replacementValue = ReplacementType[Random.Next(ReplacementType.Length)];
                result = CreateCustomActionResult(result, "Replacement", replacementValue);
            }

            return result;
        }

        public override IHttpActionResult Get(
            [FromUri(Name = "id")] string refId,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string _))
            {
                return Unauthorized();
            }

            if (HttpUtils.HasPagingHeaders(Request.Headers))
            {
                return StatusCode(HttpStatusCode.MethodNotAllowed);
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest($"Request failed for object {TypeName} as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;

            try
            {
                Queue obj = Service.Retrieve(refId, zoneId?[0], contextId?[0]);

                if (obj == null)
                {
                    result = NotFound();
                }
                else
                {
                    result = Ok(obj);
                }
            }
            catch (ArgumentException e)
            {
                result = BadRequest($"Invalid argument: id={refId}.\n{e.Message}");
            }
            catch (QueryException e)
            {
                result = BadRequest($"Request failed for object {TypeName} with ID of {refId}.\n{e.Message}");
            }
            catch (Exception e)
            {
                result = InternalServerError(e);
            }

            return result;
        }

        public override IHttpActionResult Head(
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [NonAction]
        public override IHttpActionResult Post(
            List<Queue> objs,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        public override IHttpActionResult Post(
            Queue obj,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string _))
            {
                return Unauthorized();
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest($"Request failed for object {TypeName} as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;

            try
            {
                bool hasAdvisoryId = !string.IsNullOrWhiteSpace(obj.RefId);
                bool? mustUseAdvisory = HttpUtils.GetMustUseAdvisory(Request.Headers);

                if (mustUseAdvisory.HasValue)
                {
                    if (mustUseAdvisory.Value && !hasAdvisoryId)
                    {
                        result = BadRequest(
                            $"Request failed for object {TypeName} as object ID is not provided, but mustUseAdvisory is true.");
                    }
                    else
                    {
                        Queue createdObject = Service.Create(obj, mustUseAdvisory, zoneId?[0], contextId?[0]);
                        string uri = Url.Link("DefaultApi", new { controller = TypeName, id = createdObject.RefId });
                        result = Created(uri, createdObject);
                    }
                }
                else
                {
                    Queue createdObject = Service.Create(obj, null, zoneId?[0], contextId?[0]);
                    string uri =
                        Url.Link("DefaultApi", new { controller = TypeName, id = createdObject.RefId });
                    result = Created(uri, createdObject);
                }
            }
            catch (AlreadyExistsException)
            {
                result = Conflict();
            }
            catch (ArgumentException e)
            {
                result = BadRequest($"Object to create of type {TypeName} is invalid.\n{e.Message}");
            }
            catch (CreateException e)
            {
                result = BadRequest($"Request failed for object {TypeName}.\n{e.Message}");
            }
            catch (RejectedException)
            {
                result = NotFound();
            }
            catch (QueryException e)
            {
                result = BadRequest($"Request failed for object {TypeName}.\n{e.Message}");
            }
            catch (Exception e)
            {
                result = InternalServerError(e);
            }

            return result;
        }

        public override IHttpActionResult Put(
            List<Queue> objs,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        public override IHttpActionResult Put(
            [FromUri(Name = "id")] string refId,
            Queue obj,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }
    }
}