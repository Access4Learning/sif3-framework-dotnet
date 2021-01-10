using Sif.Framework.Controllers;
using Sif.Framework.Extensions;
using Sif.Framework.Providers;
using Sif.Framework.Service;
using Sif.Framework.Service.Functional;
using Sif.Specification.Infrastructure;
using System;
using System.Net;
using System.Web.Http.Controllers;
using Tardigrade.Framework.Exceptions;

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
                if (IsFunctionalService(service.GetType()))
                {
                    return ((IFunctionalService)service).GetServiceName();
                }

                if (IsDataModelService(service.GetType()))
                {
                    return service.GetType().GenericTypeArguments[0].Name;
                }
            }
            catch (Exception e)
            {
                throw new NotFoundException(
                    "Could not infer the name of the service with type " + service.GetType().Name,
                    e);
            }

            throw new NotFoundException("Could not infer the name of the service with type " + service.GetType().Name);
        }

        /// <summary>
        /// Returns true if the given type is a functional service, false otherwise.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>See def.</returns>
        public static bool IsFunctionalService(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            bool isFService = type.IsClass
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
        public static bool IsDataModelService(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
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
        public static bool IsController(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            bool isController = type.IsClass &&
                type.IsVisible &&
                !type.IsAbstract &&
                (type.IsAssignableToGenericType(typeof(IProvider<,,>)) ||
                type.IsAssignableToGenericType(typeof(SifController<,>)) ||
                typeof(FunctionalServiceProvider).IsAssignableFrom(type)) &&
                typeof(IHttpController).IsAssignableFrom(type) &&
                type.Name.EndsWith("Provider");

            return isController;
        }

        /// <summary>
        /// Returns true if the given Guid instance is not null/empty or an empty Guid, false otherwise.
        /// </summary>
        /// <param name="id">The Guid instance to check.</param>
        /// <returns>See def.</returns>
        public static bool IsAdvisoryId(Guid id)
        {
            return StringUtils.NotEmpty(id) && Guid.Empty != id;
        }

        /// <summary>
        /// Returns true if the given string instance is not null/empty or an empty Guid, false otherwise.
        /// </summary>
        /// <param name="id">The string representation of a Guid to check.</param>
        /// <returns>See def.</returns>
        public static bool IsAdvisoryId(string id)
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
            var error = new errorType { id = Guid.NewGuid().ToString(), code = (uint)statusCode, scope = scope };

            if (!string.IsNullOrWhiteSpace(message))
            {
                error.message = message.Trim();
            }

            return error;
        }

        public static createType CreateCreate(
            HttpStatusCode statusCode,
            string id,
            string advisoryId = null,
            errorType error = null)
        {
            var create = new createType { statusCode = ((int)statusCode).ToString(), id = id };

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
            var createResponse = new createResponseType { creates = creates };

            return createResponse;
        }

        public static createResponseType CreateCreateResponse(createType create)
        {
            var createResponse = new createResponseType { creates = new[] { create } };

            return createResponse;
        }

        public static deleteStatus CreateDelete(HttpStatusCode statusCode, string id, errorType error = null)
        {
            var status = new deleteStatus { statusCode = ((int)statusCode).ToString(), id = id };

            if (error != null)
            {
                status.error = error;
            }

            return status;
        }

        public static deleteResponseType CreateDeleteResponse(deleteStatus[] statuses)
        {
            var deleteResponse = new deleteResponseType { deletes = statuses };

            return deleteResponse;
        }

        public static deleteResponseType CreateDeleteResponse(deleteStatus status)
        {
            var deleteResponse = new deleteResponseType { deletes = new[] { status } };

            return deleteResponse;
        }
    }
}