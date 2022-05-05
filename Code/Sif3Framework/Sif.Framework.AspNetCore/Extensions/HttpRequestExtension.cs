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
using Microsoft.Extensions.Primitives;
using Sif.Framework.Model.Parameters;

namespace Sif.Framework.AspNetCore.Extensions;

/// <summary>
/// This static class contains extension methods for the HttpRequest class.
/// </summary>
public static class HttpRequestExtension
{
    /// <summary>
    /// Get the query parameters associated with the HTTP Request.
    /// </summary>
    /// <param name="request">HTTP Request to check.</param>
    /// <returns>Query Parameters associated with the http Request if found; an empty collection otherwise.</returns>
    /// <exception cref="ArgumentNullException">Parameter is null.</exception>
    public static IEnumerable<RequestParameter> GetQueryParameters(this HttpRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var requestParameters = new List<RequestParameter>();

        foreach ((string? key, StringValues value) in request.Query)
        {
            requestParameters.AddRange(
                value.Select(stringValue => new RequestParameter(key, stringValue, ConveyanceType.QueryParameter)));
        }

        return requestParameters.ToArray();
    }
}