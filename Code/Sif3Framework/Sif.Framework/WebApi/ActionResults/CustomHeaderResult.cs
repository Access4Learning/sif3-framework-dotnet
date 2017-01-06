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

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sif.Framework.WebApi.ActionResults
{

    /// <summary>
    /// Represents an action result with a custom header.
    /// </summary>
    public class CustomHeaderResult : IHttpActionResult
    {
        private IHttpActionResult actionResult;
        private string headerName;
        private ICollection<string> headerValues;

        /// <summary>
        /// Create instance based on an existing action result and a custom header.
        /// </summary>
        /// <param name="actionResult">Original action result.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValues">Values associated with the header.</param>
        /// <exception cref="ArgumentException">headerName or headerValues is invalid.</exception>
        /// <exception cref="ArgumentNullException">actionResult is null.</exception>
        public CustomHeaderResult(IHttpActionResult actionResult, string headerName, ICollection<string> headerValues)
        {

            if (actionResult == null)
            {
                throw new ArgumentNullException("actionResult");
            }

            if (string.IsNullOrWhiteSpace(headerName))
            {
                throw new ArgumentException("Header name cannot be null, empty or blank.", "headerName");
            }

            if (headerValues == null || headerValues.Count == 0)
            {
                throw new ArgumentException("Header values cannot be null or empty.", "headerValues");
            }

            this.actionResult = actionResult;
            this.headerName = headerName;
            this.headerValues = headerValues;
        }

        /// <summary>
        /// Include a custom header in the original action result.
        /// <see cref="IHttpActionResult.ExecuteAsync(CancellationToken)"/>
        /// </summary>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await actionResult.ExecuteAsync(cancellationToken);
            response.Headers.Add(headerName, headerValues);

            return response;
        }

    }

}
