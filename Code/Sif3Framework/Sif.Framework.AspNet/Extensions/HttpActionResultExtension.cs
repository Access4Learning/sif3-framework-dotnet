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

using Sif.Framework.AspNet.ActionResults;
using System.Collections.Generic;
using System.Web.Http;

namespace Sif.Framework.AspNet.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the HttpActionResult class.
    /// </summary>
    public static class HttpActionResultExtension
    {
        /// <summary>
        /// Extension to add a custom header to an action result.
        /// </summary>
        /// <param name="actionResult">Action result.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValue">Value associated with the header.</param>
        /// <returns>Action result with a custom header.</returns>
        public static IHttpActionResult AddHeader(
            this IHttpActionResult actionResult,
            string headerName,
            string headerValue)
        {
            return AddHeader(actionResult, headerName, new[] { headerValue });
        }

        /// <summary>
        /// Extension to add a custom header to an action result.
        /// </summary>
        /// <param name="actionResult">Action result.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValues">Values associated with the header.</param>
        /// <returns>Action result with a custom header.</returns>
        public static IHttpActionResult AddHeader(
            this IHttpActionResult actionResult,
            string headerName,
            ICollection<string> headerValues)
        {
            return new CustomHeaderResult(actionResult, headerName, headerValues);
        }

        /// <summary>
        /// Extension to clear the content of an action result but the leave the content length.
        /// </summary>
        /// <param name="actionResult">Action result.</param>
        /// <returns>Action result with empty content.</returns>
        public static IHttpActionResult ClearContent(this IHttpActionResult actionResult)
        {
            return new EmptyContentResult(actionResult);
        }
    }
}