using Sif.Framework.Controllers;
using Sif.Framework.Extensions;
using Sif.Framework.Model.Exceptions;
using Sif.Framework.Providers;
using Sif.Framework.Service;
using Sif.Framework.Service.Functional;
using Sif.Specification.Infrastructure;
using System;
using System.Net;
using System.Web.Http.Controllers;

namespace Sif.Framework.Utils
{

    public class ProviderUtils
    {

        /// <summary>
        /// Infer the name of the Object/Functional Service type
        /// </summary>
        public static string GetServiceName(IService service)
        {
            try
            {
                if (isFunctionalService(service.GetType()))
                {
                    return ((IFunctionalService)service).GetServiceName();
                }

                if (isDataModelService(service.GetType()))
                {
                    return service.GetType().GenericTypeArguments[0].Name;
                }
            }
            catch(Exception e) {
                throw new NotFoundException("Could not infer the name of the service with type " + service.GetType().Name, e);
            }

            throw new NotFoundException("Could not infer the name of the service with type " + service.GetType().Name);
        }

        /// <summary>
        /// Returns true if the given type is a functional service, false otherwise.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>See def.</returns>
        public static Boolean isFunctionalService(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("Argument type cannot be null");
            }

            Boolean isFService = type.IsClass
                && type.IsVisible
                && !type.IsAbstract
                && typeof(IFunctionalService).IsAssignableFrom(type);

            return isFService;
        }

        /// <summary>
        /// Returns true if the given type is a data service, false otherwise.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>See def.</returns>
        public static Boolean isDataModelService(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("Argument type cannot be null");
            }

            return type.IsClass
                && type.IsVisible 
                && !type.IsAbstract
                && type.IsAssignableToGenericType(typeof(IObjectService<,,>));
        }

        /// <summary>
        /// Returns true if the given type is a controller, false otherwise.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>See def.</returns>
        public static Boolean isController(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("Argument type cannot be null");
            }

            Boolean iscontroller = type.IsClass &&
                type.IsVisible &&
                !type.IsAbstract &&
                (type.IsAssignableToGenericType(typeof(IProvider<,,>)) ||
                type.IsAssignableToGenericType(typeof(SifController<,>)) ||
                typeof(FunctionalServiceProvider).IsAssignableFrom(type)) &&
                typeof(IHttpController).IsAssignableFrom(type) &&
                type.Name.EndsWith("Provider");
            
            return iscontroller;
        }

        /// <summary>
        /// Returns true if the given Guid instance is not null/empty or an empty Guid, false otherwise.
        /// </summary>
        /// <param name="id">The Guid instance to check.</param>
        /// <returns>See def.</returns>
        public static Boolean IsAdvisoryId(Guid id)
        {
            return StringUtils.NotEmpty(id) && Guid.Empty != id;
        }

        /// <summary>
        /// Returns true if the given string instance is not null/empty or an empty Guid, false otherwise.
        /// </summary>
        /// <param name="id">The string representation of a Guid to check.</param>
        /// <returns>See def.</returns>
        public static Boolean IsAdvisoryId(string id)
        {
            return StringUtils.NotEmpty(id) && Guid.Empty != Guid.Parse(id);
        }

        /// <summary>
        /// Convenience method for creating an error record.
        /// </summary>
        /// <param name="statusCode">HTTP status code.</param>
        /// <param name="scope">The scope of the error.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Error record.</returns>
        public static errorType CreateError(HttpStatusCode statusCode, string scope, string message = null)
        {
            errorType error = new errorType();
            error.id = Guid.NewGuid().ToString();
            error.code = (uint)statusCode;
            error.scope = scope;

            if (!string.IsNullOrWhiteSpace(message))
            {
                error.message = message.Trim();
            }

            return error;
        }

        public static createType CreateCreate(HttpStatusCode statusCode, string id, string advisoryId = null, errorType error = null)
        {
            createType create = new createType();
            create.statusCode = ((int)statusCode).ToString();
            create.id = id;
            if (StringUtils.NotEmpty(advisoryId))
            {
                create.advisoryId = advisoryId;
            }
            if (error != null)
            {
                create.error = error;
            }
            return create;
        }

        public static createResponseType CreateCreateResponse(createType[] creates)
        {
            createResponseType createResponse = new createResponseType();
            createResponse.creates = creates;
            return createResponse;
        }

        public static createResponseType CreateCreateResponse(createType create)
        {
            createResponseType createResponse = new createResponseType();
            createResponse.creates = new createType[] { create } ;
            return createResponse;
        }

        public static deleteStatus CreateDelete(HttpStatusCode statusCode, string id, errorType error = null)
        {
            deleteStatus status = new deleteStatus();
            status.statusCode = ((int)statusCode).ToString();
            status.id = id;
            if (error != null)
            {
                status.error = error;
            }
            return status;
        }

        public static deleteResponseType CreateDeleteResponse(deleteStatus[] statuses)
        {
            deleteResponseType deleteResponse = new deleteResponseType();
            deleteResponse.deletes = statuses;
            return deleteResponse;
        }

        public static deleteResponseType CreateDeleteResponse(deleteStatus status)
        {
            deleteResponseType deleteResponse = new deleteResponseType();
            deleteResponse.deletes = new deleteStatus[] { status };
            return deleteResponse;
        }
    }
}
