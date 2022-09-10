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

using Sif.Framework.Models.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Sif.Framework.AspNet.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the HttpRequestMessage class.
    /// </summary>
    public static class HttpRequestMessageExtension
    {
        /// <summary>
        /// Get the query parameters associated with the HTTP Request.
        /// </summary>
        /// <param name="request">HTTP Request to check.</param>
        /// <returns>Query Parameters associated with the http Request if found; an empty collection otherwise.</returns>
        /// <exception cref="ArgumentNullException">Parameter is null.</exception>
        public static IEnumerable<RequestParameter> GetQueryParameters(this HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return request
                .GetQueryNameValuePairs()
                .Select(kvp => new RequestParameter(kvp.Key, kvp.Value, ConveyanceType.QueryParameter));
        }
    }
}