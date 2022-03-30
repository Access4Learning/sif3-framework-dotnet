/*
 * Copyright 2022 Systemic Pty Ltd
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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sif.Framework.AspNetCore.Extensions;
using Sif.Framework.AspNetCore.Services.Authentication;
using Sif.Framework.AspNetCore.Services.Authorisation;
using Sif.Framework.Extensions;
using Sif.Framework.Model.Authentication;
using Sif.Framework.Model.DataModels;
using Sif.Framework.Model.Events;
using Sif.Framework.Model.Exceptions;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Parameters;
using Sif.Framework.Model.Requests;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Authentication;
using Sif.Framework.Service.Authorisation;
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.Service.Providers;
using Sif.Framework.Service.Registration;
using Sif.Framework.Service.Sessions;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System.Collections.Specialized;
using System.Net;
using Tardigrade.Framework.Exceptions;

namespace Sif.Framework.AspNetCore.Providers;

/// <summary>
/// This class defines a Provider of SIF data model objects.
/// </summary>
/// <typeparam name="TSingle">Type that defines a single object entity.</typeparam>
/// <typeparam name="TMultiple">Type that defines a multiple objects entity.</typeparam>
[ApiController]
public abstract class Provider<TSingle, TMultiple>
    : ControllerBase, IProvider<TSingle, TMultiple, string>, IEventPayloadSerialisable<TMultiple>
    where TSingle : ISifRefId<string>
{
    private readonly ISessionService _sessionService;

    /// <summary>
    /// Accepted content type (XML or JSON) for a message payload.
    /// </summary>
    protected Accept Accept => ProviderSettings.Accept;

    /// <summary>
    /// Service used for request authentication.
    /// </summary>
    protected IAuthenticationService<IHeaderDictionary> AuthenticationService { get; }

    /// <summary>
    /// Service used for request authorisation.
    /// </summary>
    protected IAuthorisationService<IHeaderDictionary> AuthorisationService { get; }

    /// <summary>
    /// Content type (XML or JSON) of the message payload.
    /// </summary>
    protected ContentType ContentType => ProviderSettings.ContentType;

    /// <summary>
    /// Application settings associated with the Provider.
    /// </summary>
    protected IFrameworkSettings ProviderSettings { get; }

    /// <summary>
    /// Object service associated with this Provider.
    /// </summary>
    protected IProviderService<TSingle, TMultiple> Service { get; }

    /// <summary>
    /// Name of the SIF data model that the Provider is based on, e.g. SchoolInfo, StudentPersonal, etc.
    /// </summary>
    protected virtual string TypeName => typeof(TSingle).Name;

    /// <summary>
    /// Create an instance based on the specified service.
    /// </summary>
    /// <param name="service">Service used for managing the object type.</param>
    /// <param name="applicationRegisterService">Application register service.</param>
    /// <param name="environmentService">Environment service.</param>
    /// <param name="settings">Provider settings. If null, Provider settings will be read from the SifFramework.config file.</param>
    /// <param name="sessionService">Provider session service. If null, the Provider session will be stored in the SifFramework.config file.</param>
    /// <exception cref="ArgumentNullException">service is null.</exception>
    protected Provider(
        IProviderService<TSingle, TMultiple> service,
        IApplicationRegisterService applicationRegisterService,
        IEnvironmentService environmentService,
        IFrameworkSettings? settings = null,
        ISessionService? sessionService = null)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
        ProviderSettings = settings ?? SettingsManager.ProviderSettings;
        _sessionService = sessionService ?? SessionsManager.ProviderSessionService;

        AuthenticationService = ProviderSettings.EnvironmentType switch
        {
            EnvironmentType.BROKERED =>
                new BrokeredAuthenticationService(
                    applicationRegisterService,
                    environmentService,
                    ProviderSettings,
                    _sessionService),
            EnvironmentType.DIRECT => new DirectAuthenticationService(applicationRegisterService, environmentService),
            _ => new DirectAuthenticationService(applicationRegisterService, environmentService)
        };

        AuthorisationService = new AuthorisationService(AuthenticationService);
    }

    /// <inheritdoc cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Post(TTSingle, string, string)" />
    [HttpPost]
    public virtual IActionResult Post(
        TSingle obj,
        [FromQuery] string? zoneId = null,
        [FromQuery] string? contextId = null)
    {
        if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
        {
            return Unauthorized();
        }

        // Check ACLs and return StatusCode(StatusCodes.Status403Forbidden) if appropriate.
        if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.CREATE))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        IActionResult result;

        try
        {
            bool hasAdvisoryId = !string.IsNullOrWhiteSpace(obj.RefId);
            bool? mustUseAdvisory = Request.Headers.GetMustUseAdvisory();

            if (mustUseAdvisory.HasValue)
            {
                if (mustUseAdvisory.Value && !hasAdvisoryId)
                {
                    result = BadRequest(
                        $"Request failed for object {TypeName} as object ID is not provided, but mustUseAdvisory is true.");
                }
                else
                {
                    RequestParameter[] requestParameters = Request.GetQueryParameters().ToArray();
                    TSingle createdObject =
                        Service.Create(obj, mustUseAdvisory, zoneId, contextId, requestParameters);
                    result = CreatedAtAction(nameof(GetById), new { id = createdObject.RefId }, createdObject);
                }
            }
            else
            {
                RequestParameter[] requestParameters = Request.GetQueryParameters().ToArray();
                TSingle createdObject = Service.Create(obj, null, zoneId, contextId, requestParameters);
                result = CreatedAtAction(nameof(GetById), new { id = createdObject.RefId }, createdObject);
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
        catch (RejectedException e)
        {
            result = NotFound(
                $"Create request rejected for object {TypeName} with ID of {obj.RefId}.\n{e.Message}");
        }
        catch (QueryException e)
        {
            result = BadRequest($"Request failed for object {TypeName}.\n{e.Message}");
        }
        catch (Exception e)
        {
            result = StatusCode(StatusCodes.Status500InternalServerError, e);
        }

        return result;
    }

    /// <inheritdoc cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Post(TMultiple, string, string)" />
    [HttpPost]
    public abstract IActionResult Post(TMultiple obj, string? zoneId = null, string? contextId = null);

    /// <inheritdoc cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.GetById(TPrimaryKey, string, string)" />
    [HttpGet("{id}")]
    public virtual IActionResult GetById(
        [FromRoute(Name = "id")] string refId,
        [FromQuery] string? zoneId = null,
        [FromQuery] string? contextId = null)
    {
        if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
        {
            return Unauthorized();
        }

        // Check ACLs and return StatusCode(StatusCodes.Status403Forbidden) if appropriate.
        if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.QUERY))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        if (Request.Headers.HasPagingHeaders())
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);
        }

        IActionResult result;

        try
        {
            RequestParameter[] requestParameters = Request.GetQueryParameters().ToArray();
            TSingle obj = Service.Retrieve(refId, zoneId, contextId, requestParameters);

            if (obj == null)
            {
                result = StatusCode(StatusCodes.Status204NoContent);
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
            result = StatusCode(StatusCodes.Status500InternalServerError, e);
        }

        return result;
    }

    /// <summary>
    /// Retrieve all objects.
    /// </summary>
    /// <param name="zoneId">Zone associated with the request.</param>
    /// <param name="contextId">Zone context.</param>
    /// <exception cref="ArgumentException">One or more parameters of the Provider service call are invalid.</exception>
    /// <exception cref="ContentTooLargeException">Too many objects to return.</exception>
    /// <exception cref="QueryException">Error retrieving objects.</exception>
    /// <exception cref="Exception">Catch all for exceptions thrown by the implementation of the Provider service interface.</exception>
    /// <returns>All objects.</returns>
    private IActionResult GetAll(string? zoneId, string? contextId)
    {
        if (Request.Headers.HasMethodOverrideHeader())
        {
            return BadRequest("GET (Query by Example) request failed due to missing payload.");
        }

        uint? navigationPage = Request.Headers.GetNavigationPage();
        uint? navigationPageSize = Request.Headers.GetNavigationPageSize();
        RequestParameter[] requestParameters = Request.GetQueryParameters().ToArray();
        TMultiple items =
            Service.Retrieve(navigationPage, navigationPageSize, zoneId, contextId, requestParameters);
        IActionResult result;

        if (items == null)
        {
            result = StatusCode(StatusCodes.Status204NoContent);
        }
        else
        {
            result = Ok(items);
        }

        return result;
    }

    /// <summary>
    /// Retrieve objects based on the Changes Since marker.
    /// </summary>
    /// <param name="changesSinceMarker">Changes Since marker.</param>
    /// <param name="zoneId">Zone associated with the request.</param>
    /// <param name="contextId">Zone context.</param>
    /// <exception cref="ArgumentException">One or more parameters of the Provider service call are invalid.</exception>
    /// <exception cref="ContentTooLargeException">Too many objects to return.</exception>
    /// <exception cref="QueryException">Error retrieving objects.</exception>
    /// <exception cref="Exception">Catch all for exceptions thrown by the implementation of the Provider service interface.</exception>
    /// <returns>Objects associated with the Changes Since marker.</returns>
    private IActionResult GetChangesSince(string changesSinceMarker, string? zoneId, string? contextId)
    {
        if (Request.Headers.HasMethodOverrideHeader())
        {
            return BadRequest("The Changes Since marker is not applicable for a GET (Query by Example) request.");
        }

        bool changesSinceRequested = !string.IsNullOrWhiteSpace(changesSinceMarker);
        var changesSinceService = Service as IChangesSinceService<TMultiple>;
        bool changesSinceSupported = changesSinceService != null;

        if (changesSinceRequested && !changesSinceSupported)
        {
            return BadRequest("The Changes Since request is not supported.");
        }

        uint? navigationPage = Request.Headers.GetNavigationPage();
        uint? navigationPageSize = Request.Headers.GetNavigationPageSize();
        RequestParameter[] requestParameters = Request.GetQueryParameters().ToArray();
        TMultiple items = changesSinceService.RetrieveChangesSince(
            changesSinceMarker,
            navigationPage,
            navigationPageSize,
            zoneId,
            contextId,
            requestParameters);
        IActionResult result;

        if (items == null)
        {
            result = StatusCode(StatusCodes.Status204NoContent);
        }
        else
        {
            result = Ok(items);
        }

        bool pagedRequest = navigationPage.HasValue && navigationPageSize.HasValue;
        bool firstPage = navigationPage == 1;

        if (pagedRequest && !firstPage) return result;

        // Changes Since marker is only returned for non-paged requests or the first page of a paged request.
        try
        {
            Response.Headers.Add(
                "changesSinceMarker",
                changesSinceService.NextChangesSinceMarker ?? string.Empty);
        }
        catch (Exception)
        {
            throw new QueryException(
                "Implementation to retrieve the next Changes Since marker returned an error.");
        }

        return result;
    }

    /// <summary>
    /// Retrieve objects using Query by Example.
    /// </summary>
    /// <param name="obj">Example object.</param>
    /// <param name="zoneId">Zone associated with the request.</param>
    /// <param name="contextId">Zone context.</param>
    /// <exception cref="ArgumentException">One or more parameters of the Provider service call are invalid.</exception>
    /// <exception cref="ContentTooLargeException">Too many objects to return.</exception>
    /// <exception cref="QueryException">Error retrieving objects.</exception>
    /// <exception cref="Exception">Catch all for exceptions thrown by the implementation of the Provider service interface.</exception>
    /// <returns>Objects which match the Query by Example.</returns>
    private IActionResult GetQueryByExample(TSingle obj, string? zoneId, string? contextId)
    {
        if (!Request.Headers.HasMethodOverrideHeader())
        {
            return BadRequest("GET (Query by Example) request failed due to a missing method override header.");
        }

        uint? navigationPage = Request.Headers.GetNavigationPage();
        uint? navigationPageSize = Request.Headers.GetNavigationPageSize();
        RequestParameter[] requestParameters = Request.GetQueryParameters().ToArray();
        TMultiple items =
            Service.Retrieve(obj, navigationPage, navigationPageSize, zoneId, contextId, requestParameters);
        IActionResult result;

        if (items == null)
        {
            result = StatusCode(StatusCodes.Status204NoContent);
        }
        else
        {
            result = Ok(items);
        }

        return result;
    }

    /// <inheritdoc cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Get(string, string, string)" />
    [HttpGet]
    public virtual IActionResult Get(
        string? changesSinceMarker = null,
        [FromQuery] string? zoneId = null,
        [FromQuery] string? contextId = null)
    {
        if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
        {
            return Unauthorized();
        }

        // Check ACLs and return StatusCode(StatusCodes.Status403Forbidden) if appropriate.
        if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.QUERY))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        if (!Request.Headers.ValidatePagingParameters(out string? errorMessage))
        {
            return BadRequest(errorMessage);
        }

        IActionResult result;

        try
        {
            result = changesSinceMarker == null
                ? GetAll(zoneId, contextId)
                : GetChangesSince(changesSinceMarker, zoneId, contextId);
        }
        catch (ArgumentException e)
        {
            result = BadRequest($"One or more parameters of the GET request are invalid.\n{e.Message}");
        }
        catch (QueryException e)
        {
            result = BadRequest($"GET request failed for object {TypeName}.\n{e.Message}");
        }
        catch (ContentTooLargeException)
        {
            result = StatusCode(StatusCodes.Status413RequestEntityTooLarge);
        }
        catch (Exception e)
        {
            result = StatusCode(StatusCodes.Status500InternalServerError, e);
        }

        return result;
    }

    ///// <inheritdoc cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Get(TSingle, string, string, string)" />
    //[HttpPost]
    //public virtual IActionResult Get(
    //    [FromBody] TSingle? obj,
    //    string? changesSinceMarker = null,
    //    [FromQuery] string? zoneId = null,
    //    [FromQuery] string? contextId = null)
    //{
    //    if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
    //    {
    //        return Unauthorized();
    //    }

    //    // Check ACLs and return StatusCode(StatusCodes.Status403Forbidden) if appropriate.
    //    if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.QUERY))
    //    {
    //        return StatusCode(StatusCodes.Status403Forbidden);
    //    }

    //    if (!Request.Headers.ValidatePagingParameters(out string? errorMessage))
    //    {
    //        return BadRequest(errorMessage);
    //    }

    //    if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
    //    {
    //        return BadRequest($"Request failed for object {TypeName} as Zone and/or Context are invalid.");
    //    }

    //    IActionResult result;

    //    try
    //    {
    //        if (obj == null)
    //        {
    //            result = changesSinceMarker == null
    //                ? GetAll(zoneId, contextId)
    //                : GetChangesSince(changesSinceMarker, zoneId, contextId);
    //        }
    //        else
    //        {
    //            result = GetQueryByExample(obj, zoneId, contextId);
    //        }
    //    }
    //    catch (ArgumentException e)
    //    {
    //        result = BadRequest($"One or more parameters of the GET request are invalid.\n{e.Message}");
    //    }
    //    catch (QueryException e)
    //    {
    //        result = BadRequest($"GET request failed for object {TypeName}.\n{e.Message}");
    //    }
    //    catch (ContentTooLargeException)
    //    {
    //        result = StatusCode(StatusCodes.Status413RequestEntityTooLarge);
    //    }
    //    catch (Exception e)
    //    {
    //        result = StatusCode(StatusCodes.Status500InternalServerError, e);
    //    }

    //    return result;
    //}

    ///// <inheritdoc cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Get(string, string, string, string, string, string, string[], string[])" />
    //[HttpGet("~/api/{object1}/{id1}/{object2}/{id2}/{object3}/{id3}/[controller]")]
    //public virtual IActionResult Get(
    //    string object1,
    //    [FromRoute(Name = "id1")] string refId1,
    //    string? object2 = null,
    //    [FromRoute(Name = "id2")] string? refId2 = null,
    //    string? object3 = null,
    //    [FromRoute(Name = "id3")] string? refId3 = null,
    //    [FromQuery] string[]? zoneId = null,
    //    [FromQuery] string[]? contextId = null)
    //{
    //    if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
    //    {
    //        return Unauthorized();
    //    }

    //    string serviceName;

    //    if (object3 != null)
    //    {
    //        serviceName = $"{object1}/{{}}/{object2}/{{}}/{object3}/{{}}/{TypeName}s";
    //    }
    //    else if (object2 != null)
    //    {
    //        serviceName = $"{object1}/{{}}/{object2}/{{}}/{TypeName}s";
    //    }
    //    else
    //    {
    //        serviceName = $"{object1}/{{}}/{TypeName}s";
    //    }

    //    // Check ACLs and return StatusCode(StatusCodes.Status403Forbidden) if appropriate.
    //    if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, serviceName, RightType.QUERY))
    //    {
    //        return StatusCode(StatusCodes.Status403Forbidden);
    //    }

    //    if (!Request.Headers.ValidatePagingParameters(out string? errorMessage))
    //    {
    //        return BadRequest(errorMessage);
    //    }

    //    if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
    //    {
    //        return BadRequest($"Request failed for object {TypeName} as Zone and/or Context are invalid.");
    //    }

    //    IActionResult result;

    //    try
    //    {
    //        IList<EqualCondition> conditions =
    //            new List<EqualCondition> { new() { Left = object1, Right = refId1 } };

    //        if (!string.IsNullOrWhiteSpace(object2))
    //        {
    //            conditions.Add(new EqualCondition { Left = object2, Right = refId2 });

    //            if (!string.IsNullOrWhiteSpace(object3))
    //            {
    //                conditions.Add(new EqualCondition { Left = object3, Right = refId3 });
    //            }
    //        }

    //        uint? navigationPage = Request.Headers.GetNavigationPage();
    //        uint? navigationPageSize = Request.Headers.GetNavigationPageSize();
    //        RequestParameter[] requestParameters = Request.GetQueryParameters().ToArray();
    //        TMultiple items = Service.Retrieve(
    //            conditions,
    //            navigationPage,
    //            navigationPageSize,
    //            zoneId?[0],
    //            contextId?[0],
    //            requestParameters);

    //        if (items == null)
    //        {
    //            result = StatusCode(StatusCodes.Status204NoContent);
    //        }
    //        else
    //        {
    //            result = Ok(items);
    //        }
    //    }
    //    catch (ArgumentException e)
    //    {
    //        result = BadRequest($"One or more conditions are invalid.\n{e.Message}");
    //    }
    //    catch (QueryException e)
    //    {
    //        result = BadRequest($"Service Path GET request failed for object {TypeName}.\n{e.Message}");
    //    }
    //    catch (ContentTooLargeException)
    //    {
    //        result = StatusCode(StatusCodes.Status413RequestEntityTooLarge);
    //    }
    //    catch (Exception e)
    //    {
    //        result = StatusCode(StatusCodes.Status500InternalServerError, e);
    //    }

    //    return result;
    //}

    /// <inheritdoc cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Put(TPrimaryKey, TTSingle, string, string)" />
    [HttpPut("{id}")]
    public virtual IActionResult Put(
        [FromRoute(Name = "id")] string refId,
        [FromBody] TSingle obj,
        [FromQuery] string? zoneId = null,
        [FromQuery] string? contextId = null)
    {
        if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
        {
            return Unauthorized();
        }

        // Check ACLs and return StatusCode(StatusCodes.Status403Forbidden) if appropriate.
        if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.UPDATE))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        if (string.IsNullOrWhiteSpace(refId) || !refId.Equals(obj.RefId))
        {
            return BadRequest(
                "The refId in the update request does not match the SIF identifier of the object provided.");
        }

        IActionResult result;

        try
        {
            RequestParameter[] requestParameters = Request.GetQueryParameters().ToArray();
            Service.Update(obj, zoneId, contextId, requestParameters);
            result = StatusCode(StatusCodes.Status204NoContent);
        }
        catch (ArgumentException e)
        {
            result = BadRequest($"Object to update of type {TypeName} is invalid.\n{e.Message}");
        }
        catch (NotFoundException e)
        {
            result = NotFound($"Object {TypeName} with ID of {refId} not found.\n{e.Message}");
        }
        catch (UpdateException e)
        {
            result = BadRequest($"Request failed for object {TypeName} with ID of {refId}.\n{e.Message}");
        }
        catch (Exception e)
        {
            result = StatusCode(StatusCodes.Status500InternalServerError, e);
        }

        return result;
    }

    /// <inheritdoc cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Put(TMultiple, string, string)" />
    public abstract IActionResult Put(TMultiple obj, string? zoneId = null, string? contextId = null);

    /// <inheritdoc cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Delete(TPrimaryKey, string, string)" />
    [HttpDelete("{id}")]
    public virtual IActionResult Delete(
        [FromRoute(Name = "id")] string refId,
        [FromQuery] string? zoneId = null,
        [FromQuery] string? contextId = null)
    {
        if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
        {
            return Unauthorized();
        }

        // Check ACLs and return StatusCode(StatusCodes.Status403Forbidden) if appropriate.
        if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.DELETE))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        IActionResult result;

        try
        {
            RequestParameter[] requestParameters = Request.GetQueryParameters().ToArray();
            Service.Delete(refId, zoneId, contextId, requestParameters);
            result = StatusCode(StatusCodes.Status204NoContent);
        }
        catch (ArgumentException e)
        {
            result = BadRequest($"Invalid argument: id={refId}.\n{e.Message}");
        }
        catch (DeleteException e)
        {
            result = BadRequest($"Request failed for object {TypeName} with ID of {refId}.\n{e.Message}");
        }
        catch (NotFoundException e)
        {
            result = NotFound($"Object {TypeName} with ID of {refId} not found.\n{e.Message}");
        }
        catch (Exception e)
        {
            result = StatusCode(StatusCodes.Status500InternalServerError, e);
        }

        return result;
    }

    /// <inheritdoc cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Delete(deleteRequestType, string, string)" />
    [HttpDelete]
    public virtual IActionResult Delete(
        deleteRequestType deleteRequest,
        [FromQuery] string? zoneId = null,
        [FromQuery] string? contextId = null)
    {
        if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
        {
            return Unauthorized();
        }

        // Check ACLs and return StatusCode(StatusCodes.Status403Forbidden) if appropriate.
        if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.DELETE))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        ICollection<deleteStatus> deleteStatuses = new List<deleteStatus>();

        try
        {
            foreach (deleteIdType deleteId in deleteRequest.deletes)
            {
                var status = new deleteStatus
                {
                    id = deleteId.id
                };

                try
                {
                    RequestParameter[] requestParameters = Request.GetQueryParameters().ToArray();
                    Service.Delete(deleteId.id, zoneId, contextId, requestParameters);
                    status.statusCode = StatusCodes.Status204NoContent.ToString();
                }
                catch (ArgumentException e)
                {
                    status.error = ProviderUtils.CreateError(
                        HttpStatusCode.BadRequest,
                        TypeName,
                        $"Invalid argument: id={deleteId.id}.\n{e.Message}");
                    status.statusCode = StatusCodes.Status400BadRequest.ToString();
                }
                catch (DeleteException e)
                {
                    status.error = ProviderUtils.CreateError(
                        HttpStatusCode.BadRequest,
                        TypeName,
                        $"Request failed for object {TypeName} with ID of {deleteId.id}.\n{e.Message}");
                    status.statusCode = StatusCodes.Status400BadRequest.ToString();
                }
                catch (NotFoundException e)
                {
                    status.error = ProviderUtils.CreateError(
                        HttpStatusCode.NotFound,
                        TypeName,
                        $"Object {TypeName} with ID of {deleteId.id} not found.\n{e.Message}");
                    status.statusCode = StatusCodes.Status404NotFound.ToString();
                }
                catch (Exception e)
                {
                    status.error = ProviderUtils.CreateError(
                        HttpStatusCode.InternalServerError,
                        TypeName,
                        $"Request failed for object {TypeName} with ID of {deleteId.id}.\n{e.Message}");
                    status.statusCode = StatusCodes.Status500InternalServerError.ToString();
                }

                deleteStatuses.Add(status);
            }
        }
        catch (Exception)
        {
            // Need to ignore exceptions otherwise it would not be possible to return status records of processed objects.
        }

        var deleteResponse = new deleteResponseType { deletes = deleteStatuses.ToArray() };

        return Ok(deleteResponse);
    }

    /// <inheritdoc cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Head(string, string)" />
    [HttpHead]
    public virtual IActionResult Head([FromQuery] string? zoneId = null, [FromQuery] string? contextId = null)
    {
        if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
        {
            return Unauthorized();
        }

        // Check ACLs and return StatusCode(StatusCodes.Status403Forbidden) if appropriate.
        if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.QUERY))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        if (!Request.Headers.ValidatePagingParameters(out string? errorMessage))
        {
            return BadRequest(errorMessage);
        }

        IActionResult result;

        try
        {
            result = GetAll(zoneId, contextId);

            // Clear the body content.
            // TODO Set the body content length to that of the original body before it was cleared.
            byte[] emptyByteArray = Array.Empty<byte>();
            Response.Body.WriteAsync(emptyByteArray, 0, emptyByteArray.Length);

            if (Service is ISupportsChangesSince supportsChangesSince)
            {
                Response.Headers.Add(
                    "changesSinceMarker",
                    supportsChangesSince.ChangesSinceMarker ?? string.Empty);
            }
        }
        catch (ArgumentException e)
        {
            result = BadRequest(
                $"One or more parameters of the GET request (associated with the HEAD request) are invalid.\n{e.Message}");
        }
        catch (QueryException e)
        {
            result = BadRequest($"HEAD request failed for object {TypeName}.\n{e.Message}");
        }
        catch (ContentTooLargeException)
        {
            result = StatusCode(StatusCodes.Status413RequestEntityTooLarge);
        }
        catch (Exception e)
        {
            result = StatusCode(StatusCodes.Status500InternalServerError, e);
        }

        return result;
    }

    /// <inheritdoc cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.BroadcastEvents(string, string)" />
    [HttpGet]
    public virtual IActionResult BroadcastEvents(string? zoneId = null, string? contextId = null)
    {
        var eventService = Service as IEventService<TMultiple>;

        if (eventService == null)
        {
            return BadRequest("Support for SIF Events has not been implemented.");
        }

        IActionResult result;

        try
        {
            IRegistrationService registrationService = RegistrationManager.GetProviderRegistrationService(
                ProviderSettings,
                _sessionService);

            if (registrationService is NoRegistrationService)
            {
                result = BadRequest("SIF Events are only supported in a BROKERED environment.");
            }
            else
            {
                IEventIterator<TMultiple> eventIterator = eventService.GetEventIterator(zoneId, contextId);

                if (eventIterator == null)
                {
                    result = BadRequest("SIF Events implementation is not valid.");
                }
                else
                {
                    Model.Infrastructure.Environment environment = registrationService.Register();

                    // Retrieve the current Authorisation Token.
                    AuthorisationToken token = registrationService.AuthorisationToken;

                    // Retrieve the EventsConnector endpoint URL.
                    string url = environment.ParseServiceUrl(
                        ServiceType.UTILITY,
                        InfrastructureServiceNames.eventsConnector);

                    while (eventIterator.HasNext())
                    {
                        SifEvent<TMultiple> sifEvent = eventIterator.GetNext();

                        var requestHeaders = new NameValueCollection
                        {
                            { EventParameterType.eventAction.ToDescription(), sifEvent.EventAction.ToDescription() },
                            { EventParameterType.messageId.ToDescription(), sifEvent.Id.ToString() },
                            { EventParameterType.messageType.ToDescription(), "EVENT" },
                            { EventParameterType.serviceName.ToDescription(), $"{TypeName}s" }
                        };

                        switch (sifEvent.EventAction)
                        {
                            case EventAction.UPDATE_FULL:
                                requestHeaders.Add(EventParameterType.Replacement.ToDescription(), "FULL");
                                break;

                            case EventAction.UPDATE_PARTIAL:
                                requestHeaders.Add(EventParameterType.Replacement.ToDescription(), "PARTIAL");
                                break;
                        }

                        string requestBody = SerialiseEvents(sifEvent.SifObjects);
                        HttpUtils.PostRequest(
                            url,
                            token,
                            requestBody,
                            ProviderSettings.CompressPayload,
                            contentTypeOverride: ContentType.ToDescription(),
                            acceptOverride: Accept.ToDescription(),
                            requestHeaders: requestHeaders);
                    }

                    result = Ok();
                }
            }
        }
        catch (Exception e)
        {
            result = StatusCode(StatusCodes.Status500InternalServerError, e);
        }

        return result;
    }

    /// <inheritdoc cref="IEventPayloadSerialisable{TMultiple}.SerialiseEvents(TMultiple)" />
    public abstract string SerialiseEvents(TMultiple obj);
}