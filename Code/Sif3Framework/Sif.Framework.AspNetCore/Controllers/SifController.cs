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
using Sif.Framework.Service.Authentication;
using Tardigrade.Framework.AspNetCore.Extensions;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.Models.Domain;
using Tardigrade.Framework.Models.Persistence;
using Tardigrade.Framework.Services;

namespace Sif.Framework.AspNetCore.Controllers;

/// <summary>
/// SIF specific REST API Controller based on a SIF Object type.
/// </summary>
/// <typeparam name="TEntity">Domain model object associated with the service operations.</typeparam>
/// <typeparam name="TDto">SIF Object type associated with the service operations.</typeparam>
/// <typeparam name="TDtoKey">Unique identifier type for the SIF Object type.</typeparam>
[ApiController]
[ApiConventionType(typeof(DefaultApiConventions))]
[Route("api/[controller]")]
public abstract class SifController<TEntity, TDto, TDtoKey> : ControllerBase where TDto : IHasUniqueIdentifier<TDtoKey>
{
    /// <summary>
    /// Service used for request authentication.
    /// </summary>
    protected IAuthenticationService<IHeaderDictionary> AuthenticationService { get; }

    /// <summary>
    /// Object service associated with the Controller.
    /// </summary>
    protected IDtoService<TEntity, TDto, TDtoKey> Service { get; }

    /// <summary>
    /// Create an instance of this API Controller.
    /// </summary>
    /// <param name="service">Service associated with the object type.</param>
    /// <param name="authenticationService">Authentication service.</param>
    /// <exception cref="ArgumentNullException">A parameter is null.</exception>
    protected SifController(
        IDtoService<TEntity, TDto, TDtoKey> service,
        IAuthenticationService<IHeaderDictionary> authenticationService)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
        AuthenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    }

    /// <summary>
    /// DELETE: api/[controller]/{id}
    /// 200 OK
    /// 400 Bad Request
    /// 404 Not Found
    /// </summary>
    /// <param name="id">Unique identifier of the object to delete.</param>
    /// <returns>Result of the delete action.</returns>
    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete(TDtoKey id)
    {
        //if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers))
        //{
        //    return Unauthorized();
        //}

        ActionResult result;

        try
        {
            TDto model = await Service.RetrieveAsync(id);

            if (model == null)
            {
                result = NotFound();
            }
            else
            {
                await Service.DeleteAsync(model);
                result = Ok();
            }
        }
        catch (ServiceException e)
        {
            result = this.StatusCode(StatusCodes.Status500InternalServerError, message: e.GetBaseException().Message);
        }

        return result;
    }

    /// <summary>
    /// GET: api/[controller]/{id}
    /// 200 OK
    /// 404 Not Found
    /// </summary>
    /// <param name="id">Unique identifier of the object to retrieve.</param>
    /// <returns>Object with a matching unique identifier.</returns>
    [HttpGet("{id}")]
    public virtual async Task<IActionResult> Get(TDtoKey id)
    {
        //if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers))
        //{
        //    return Unauthorized();
        //}

        IActionResult result;

        try
        {
            TDto model = await Service.RetrieveAsync(id);

            if (model == null)
            {
                result = NotFound();
            }
            else
            {
                result = new ObjectResult(model);
            }
        }
        catch (ServiceException e)
        {
            result = this.StatusCode(StatusCodes.Status500InternalServerError, message: e.GetBaseException().Message);
        }

        return result;
    }

    /// <summary>
    /// GET: api/[controller]
    /// 200 OK
    /// 204 No Content
    /// 400 Bad Request
    /// </summary>
    /// <returns>Collection of objects.</returns>
    [HttpGet]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<IActionResult> Get(
        uint? pageSize = null,
        uint? pageIndex = 0,
        string? sortBy = null)
    {
        //if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers))
        //{
        //    return Unauthorized();
        //}

        PagingContext? pagingContext = null;
        Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? sortCondition = null;

        if (pageSize.HasValue)
        {
            pagingContext = new PagingContext { PageIndex = pageIndex ?? 0, PageSize = pageSize.Value };

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                sortCondition = q => q.OrderBy(o => o.Id);
            }
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            sortCondition = q => q.OrderBy(sortBy);
        }

        IActionResult result;

        try
        {
            IEnumerable<TDto> retrieved =
                await Service.RetrieveAsync(pagingContext: pagingContext, sortCondition: sortCondition);
            List<TDto> models = retrieved.ToList();

            if (!models.Any())
            {
                result = NoContent();
            }
            else
            {
                result = new ObjectResult(models);
            }
        }
        catch (ServiceException e)
        {
            result = this.StatusCode(StatusCodes.Status500InternalServerError, message: e.GetBaseException().Message);
        }

        return result;
    }

    /// <summary>
    /// POST: api/[controller]
    /// 201 Created
    /// 400 Bad Request
    /// </summary>
    /// <param name="model">Object to create.</param>
    /// <returns>Object created (including allocated unique identifier).</returns>
    [HttpPost]
    public virtual async Task<IActionResult> Post([FromBody] TDto model)
    {
        //if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers))
        //{
        //    return Unauthorized();
        //}

        //if (!ModelState.IsValid)
        //{
        //    return this.BadRequest(message: "Object to create is not valid.");
        //}

        IActionResult result;

        try
        {
            TDto createdObj = await Service.CreateAsync(model);
            result = CreatedAtAction(nameof(Get), new { id = createdObj.Id }, createdObj);
        }
        catch (ServiceException e)
        {
            result = this.StatusCode(StatusCodes.Status500InternalServerError, message: e.GetBaseException().Message);
        }
        catch (ValidationException e)
        {
            result = this.BadRequest(message: e.GetBaseException().Message);
        }

        return result;
    }

    //
    /// <summary>
    /// PUT: api/[controller]/{id}
    /// 204 No Content
    /// 400 Bad Request
    /// 404 Not Found
    /// </summary>
    /// <param name="id">Unique identifier of the object to update.</param>
    /// <param name="model">Object to update.</param>
    /// <returns>Result of the update action.</returns>
    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Put(TDtoKey id, TDto model)
    {
        //if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers))
        //{
        //    return Unauthorized();
        //}

        //if (!ModelState.IsValid)
        //{
        //    return this.BadRequest(message: "Object to update is not valid.");
        //}

        if (id != null && !id.Equals(model.Id))
        {
            return this.BadRequest(message: "Unique identifier provided does not match that of the object.");
        }

        IActionResult result;

        try
        {
            await Service.UpdateAsync(model);
            result = NoContent();
        }
        catch (NotFoundException)
        {
            result = NotFound();
        }
        catch (ServiceException e)
        {
            result = this.StatusCode(StatusCodes.Status500InternalServerError, message: e.GetBaseException().Message);
        }
        catch (ValidationException e)
        {
            result = this.BadRequest(message: e.GetBaseException().Message);
        }

        return result;
    }
}