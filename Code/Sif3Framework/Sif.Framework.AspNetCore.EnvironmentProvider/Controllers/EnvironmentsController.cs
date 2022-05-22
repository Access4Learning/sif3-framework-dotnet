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

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sif.Framework.Service.Authentication;
using Sif.Framework.Service.Infrastructure;
using Sif.Specification.Infrastructure;
using Tardigrade.Framework.AspNetCore.Extensions;
using Tardigrade.Framework.Exceptions;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.AspNetCore.EnvironmentProvider.Controllers;

/// <summary>
/// SIF specific REST API Controller for the Environment SIF Object type.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EnvironmentsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IEnvironmentService _service;

    /// <summary>
    /// Service used for request authentication.
    /// </summary>
    protected IAuthenticationService<IHeaderDictionary> AuthenticationService { get; }

    /// <summary>
    /// Create an instance of this REST API Controller.
    /// </summary>
    /// <param name="mapper">Object mapper.</param>
    /// <param name="service">Service used for operations of Environments.</param>
    /// <param name="authenticationService">Authentication service.</param>
    /// <exception cref="ArgumentNullException">A parameter is null.</exception>
    public EnvironmentsController(
        IMapper mapper,
        IEnvironmentService service,
        IAuthenticationService<IHeaderDictionary> authenticationService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        AuthenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    }

    /// <summary>
    /// DELETE: api/environments/{id}
    /// 200 OK
    /// 404 Not Found
    /// 500 Internal Server Error
    /// </summary>
    /// <param name="id">Unique identifier of the Environment to delete.</param>
    /// <returns>Result of the delete action.</returns>
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers))
        {
            return Unauthorized("DELETE Environment request failed due to invalid authentication credentials.");
        }

        IActionResult result;

        try
        {
            Environment model = _service.Retrieve(id);

            if (model == null)
            {
                result = NotFound();
            }
            else
            {
                _service.Delete(model);
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
    /// GET: api/environments/{id}
    /// 200 OK
    /// 404 Not Found
    /// 500 Internal Server Error
    /// </summary>
    /// <param name="id">Unique identifier of the Environment to retrieve.</param>
    /// <returns>Environment with a matching unique identifier.</returns>
    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers))
        {
            return Unauthorized("GET Environment request failed due to invalid authentication credentials.");
        }

        IActionResult result;

        try
        {
            Environment environment = _service.Retrieve(id);

            if (environment == null)
            {
                result = NotFound();
            }
            else
            {
                var environmentType = _mapper.Map<environmentType>(environment);
                result = new ObjectResult(environmentType);
            }
        }
        catch (ServiceException e)
        {
            result = this.StatusCode(StatusCodes.Status500InternalServerError, message: e.GetBaseException().Message);
        }

        return result;
    }

    /// <summary>
    /// GET api/environments/index
    /// 200 OK
    /// </summary>
    /// <returns>Success message.</returns>
    [HttpGet("index")]
    public IActionResult Index()
    {
        return Ok(new { Startup = "Success" });
    }

    /// <summary>
    /// POST api/environments/environment
    /// 201 Created
    /// 400 Bad Request
    /// 500 Internal Server Error
    /// </summary>
    /// <param name="item">Environment to create.</param>
    /// <returns>Environment created (including allocated unique identifier).</returns>
    [HttpPost("environment")]
    public IActionResult Post([FromBody] environmentType item)
    {
        if (!AuthenticationService.VerifyInitialAuthenticationHeader(Request.Headers, out string _))
        {
            return Unauthorized("POST Environment request failed due to invalid authentication credentials.");
        }

        IActionResult result;

        try
        {
            var environment = _mapper.Map<Environment>(item);
            environment = _service.Create(environment);
            var environmentType = _mapper.Map<environmentType>(environment);
            result = CreatedAtAction(nameof(Get), new { environmentType.id }, environmentType);
        }
        catch (AlreadyExistsException e)
        {
            result = this.StatusCode(StatusCodes.Status409Conflict, message: e.Message);
        }
        catch (NotFoundException e)
        {
            result = this.BadRequest(message: e.Message);
        }
        catch (ServiceException e)
        {
            result = this.StatusCode(StatusCodes.Status500InternalServerError, message: e.Message);
        }
        catch (ValidationException e)
        {
            result = this.BadRequest(message: e.Message);
        }

        return result;
    }
}