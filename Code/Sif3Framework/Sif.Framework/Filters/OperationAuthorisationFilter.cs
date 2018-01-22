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

using Sif.Framework.Model.Exceptions;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Service.Authorisation;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Sif.Framework.Filters
{

    /// <summary>
    /// Action filter used to verify if a resource / action can be accessed by the requester.
    /// </summary>
    [Obsolete]
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class OperationAuthorisationFilter : ActionFilterAttribute
    {
        private readonly IAuthorisationService authorisationService;
        private readonly RightType permission;
        private readonly RightValue privilege;
        private readonly string serviceName;
        private readonly string sessionToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationAuthorisationFilter" /> class.
        /// </summary>
        /// <param name="authorisationService">An instance of either the <see cref="IAuthorisationService" /> class.</param>
        /// <param name="sessionToken">Session token.</param>
        /// <param name="serviceName">The service name to check permissions.</param>
        /// <param name="permission">The permission requested. Any of: ADMIN, CREATE, DELETE, PROVIDE, QUERY, SUBSCRIBE, UPDATE</param>
        /// <param name="privilege">The access level requested. Any of APPROVED, REJECTED, SUPPORTED</param>
        public OperationAuthorisationFilter(IAuthorisationService authorisationService, string sessionToken, string serviceName, RightType permission, RightValue privilege)
        {
            this.authorisationService = authorisationService;
            this.sessionToken = sessionToken;
            this.serviceName = serviceName;
            this.permission = permission;
            this.privilege = privilege;
        }

        /// <summary>
        /// Checks if the action requester is authorised to access the enquired resource.
        /// </summary>
        /// <param name="actionContext">The action context, which encapsulates information for using the filter.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            try
            {

                if (!authorisationService.IsAuthorised(actionContext.Request.Headers, sessionToken, serviceName, permission, privilege))
                {
                    throw new RejectedException("Request is not authorized.");
                }

            }
            catch (InvalidSessionException e)
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message, e));
            }
            catch (UnauthorizedAccessException e)
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, e.Message, e));
            }
            catch (RejectedException e)
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, e.Message, e));
            }

        }

    }

}
