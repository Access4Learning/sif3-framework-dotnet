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

namespace Sif.Framework.AspNet.Utils
{
    /// <summary>
    /// This is a utility class for HTTP operations.
    /// </summary>
    public static class HttpUtils
    {
        /// <summary>
        /// This method will add the exception message to the reason phrase of the error response.
        /// </summary>
        public static HttpResponseMessage CreateErrorResponse(
            HttpRequestMessage request,
            HttpStatusCode httpStatusCode,
            Exception exception)
        {
            string exceptionMessage = exception.Message == null ? "" : exception.Message.Trim();
            HttpResponseMessage response = request.CreateErrorResponse(httpStatusCode, exception);

            // The ReasonPhrase may not contain new line characters.
            response.ReasonPhrase = exceptionMessage.RemoveNewLines();

            return response;
        }

        /// <summary>
        /// This method will add the message specified to the reason phrase of the error response.
        /// </summary>
        public static HttpResponseMessage CreateErrorResponse(
            HttpRequestMessage request,
            HttpStatusCode httpStatusCode,
            string message)
        {
            HttpResponseMessage response = request.CreateErrorResponse(httpStatusCode, message);

            // The ReasonPhrase may not contain new line characters.
            response.ReasonPhrase = message.RemoveNewLines();

            return response;
        }
    }
}