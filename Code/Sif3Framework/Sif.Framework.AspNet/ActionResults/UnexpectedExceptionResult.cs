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

using Sif.Framework.Extensions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sif.Framework.AspNet.ActionResults
{
    /// <summary>
    /// This class represents an unexpected exception result.
    /// </summary>
    public class UnexpectedExceptionResult : IHttpActionResult
    {
        private readonly string _exceptionMessage;
        private readonly string _exceptionStackTrace;
        private readonly HttpRequestMessage _requestMessage;

        /// <summary>
        /// Create an instance of UnexpectedExceptionResult.
        /// </summary>
        /// <param name="requestMessage">HTTP request message.</param>
        /// <param name="exception">Exception raised.</param>
        public UnexpectedExceptionResult(HttpRequestMessage requestMessage, Exception exception)
        {
            _requestMessage = requestMessage;
            _exceptionMessage = (exception?.Message ?? "").Trim();
            _exceptionStackTrace = (exception?.StackTrace ?? "").Trim();
        }

        /// <summary>
        /// Creates an HttpResponseMessage asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the HttpResponseMessage.</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            responseMessage.Content = new StringContent(_exceptionStackTrace);
            responseMessage.RequestMessage = _requestMessage;

            // The ReasonPhrase may not contain new line characters.
            responseMessage.ReasonPhrase = _exceptionMessage.RemoveNewLines();

            return Task.FromResult(responseMessage);
        }
    }
}