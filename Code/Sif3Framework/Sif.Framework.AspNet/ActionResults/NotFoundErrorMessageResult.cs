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

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sif.Framework.AspNet.ActionResults
{
    /// <summary>
    /// Represents an action result that returns a HttpStatusCode.NotFound response with the specified error message.
    /// </summary>
    public class NotFoundErrorMessageResult : IHttpActionResult
    {
        private readonly string _message;
        private readonly HttpRequestMessage _request;

        /// <summary>
        /// Create instance with an error message.
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <param name="message">Error message.</param>
        internal NotFoundErrorMessageResult(HttpRequestMessage request, string message)
        {
            _message = message;
            _request = request;
        }

        /// <inheritdoc cref="IHttpActionResult.ExecuteAsync(CancellationToken)" />
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(_message),
                RequestMessage = _request
            };

            return Task.FromResult(response);
        }
    }
}