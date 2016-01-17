/*
 * Copyright 2016 Systemic Pty Ltd
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

using Sif.Framework.Utils;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace Sif.Framework.WebApi
{

    /// <summary>
    /// This exception handler produces a custom error response to clients for unexpected exceptions.
    /// </summary>
    public class GlobalUnexpectedExceptionHandler : ExceptionHandler
    {

        /// <summary>
        /// This class represents an unexpected exception result.
        /// </summary>
        private class UnexpectedExceptionResult : IHttpActionResult
        {
            private string exceptionMessage;
            private string exceptionStackTrace;
            private HttpRequestMessage requestMessage;

            /// <summary>
            /// Create an instance of UnexpectedExceptionResult.
            /// </summary>
            /// <param name="requestMessage">HTTP request message.</param>
            /// <param name="exception">Exception raised.</param>
            public UnexpectedExceptionResult(HttpRequestMessage requestMessage, Exception exception)
            {
                this.requestMessage = requestMessage;
                exceptionMessage = (exception.Message == null ? "" : exception.Message.Trim());
                exceptionStackTrace = (exception.StackTrace == null ? "" : exception.StackTrace.Trim());
            }

            /// <summary>
            /// Creates an HttpResponseMessage asynchronously.
            /// </summary>
            /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
            /// <returns>A task that, when completed, contains the HttpResponseMessage.</returns>
            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = new StringContent(exceptionStackTrace);
                responseMessage.RequestMessage = requestMessage;
                // The ReasonPhrase may not contain new line characters.
                responseMessage.ReasonPhrase = StringUtils.RemoveNewLines(exceptionMessage);
                return Task.FromResult(responseMessage);
            }

        }

        /// <summary>
        /// This method will handle the exception synchronously.
        /// </summary>
        /// <param name="context">The exception handler context.</param>
        public override void Handle(ExceptionHandlerContext context)
        {
            context.Result = new UnexpectedExceptionResult(context.Request, context.Exception);
        }

    }

}
