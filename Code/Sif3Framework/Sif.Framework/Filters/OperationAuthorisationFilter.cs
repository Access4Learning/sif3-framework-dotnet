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
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class OperationAuthorisationFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Service used for request authorisation.
        /// </summary>
        private readonly IOperationAuthorisationService authorisationService;

        /// <summary>
        /// Service name to check request permissions - it is defined in the ACL.
        /// </summary>
        private readonly string serviceName;

        /// <summary>
        /// >The permission requested. Any of: ADMIN, CREATE, DELETE, PROVIDE, QUERY, SUBSCRIBE, UPDATE
        /// </summary>
        private readonly RightType permission;

        /// <summary>
        /// The access level requested. Any of APPROVED, REJECTED, SUPPORTED
        /// </summary>
        private readonly RightValue privilege;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sif.Framework.Filters.OperationAuthorisationFilter" /> class with the given permission
        /// and the privilege defaulted to APPROVED.
        /// </summary>
        /// <param name="serviceName">The service name to check permissions.</param>
        /// <param name="permission">The permission requested. Any of: ADMIN, CREATE, DELETE, PROVIDE, QUERY, SUBSCRIBE, UPDATE</param>
        public OperationAuthorisationFilter(string serviceName, RightType permission)
            : this(serviceName, permission, RightValue.APPROVED) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationAuthorisationFilter" /> class.
        /// </summary>
        /// <param name="serviceName">The service name to check permissions.</param>
        /// <param name="permission">The permission requested. Any of: ADMIN, CREATE, DELETE, PROVIDE, QUERY, SUBSCRIBE, UPDATE</param>
        /// <param name="privilege">The access level requested. Any of APPROVED, REJECTED, SUPPORTED</param>
        public OperationAuthorisationFilter(string serviceName, RightType permission, RightValue privilege)
            : this(new OperationAuthorisationService(), serviceName, permission, privilege) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sif.Framework.Filters.OperationAuthorisationFilter" /> class with the given permission
        /// and the privilege defaulted to APPROVED.
        /// </summary>
        /// <param name="authService">An instance of either the <see cref="Sif.Framework.Service.Authorisation.IOperationAuthorisationService" /> class.</param>
        /// <param name="serviceName">The service name to check permissions.</param>
        /// <param name="permission">The permission requested. Any of: ADMIN, CREATE, DELETE, PROVIDE, QUERY, SUBSCRIBE, UPDATE</param>
        public OperationAuthorisationFilter(IOperationAuthorisationService authService, string serviceName, RightType permission)
            : this(authService, serviceName, permission, RightValue.APPROVED) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sif.Framework.Filters.OperationAuthorisationFilter" /> class.
        /// </summary>
        /// <param name="authService">An instance of either the <see cref="Sif.Framework.Service.Authorisation.IOperationAuthorisationService" /> class.</param>
        /// <param name="serviceName">The service name to check permissions.</param>
        /// <param name="permission">The permission requested. Any of: ADMIN, CREATE, DELETE, PROVIDE, QUERY, SUBSCRIBE, UPDATE</param>
        /// <param name="privilege">The access level requested. Any of APPROVED, REJECTED, SUPPORTED</param>
        public OperationAuthorisationFilter(IOperationAuthorisationService authService, string serviceName, RightType permission, RightValue privilege)
        {
            this.authorisationService = authService;
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
                if (!this.authorisationService.IsAuthorised(actionContext.Request.Headers, serviceName, permission, privilege))
                {
                    // it shouldn't happen, because by design the IsAuthorised method throws an exception if request is unauthorised.
                    throw new RejectedException("Request is not authorized.");
                }
            }
            catch (InvalidRequestException e)
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message, e));
            }
            catch (UnauthorisedRequestException e)
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