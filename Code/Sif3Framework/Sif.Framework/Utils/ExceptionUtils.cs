/*
 * Copyright 2015 Systemic Pty Ltd
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
using System.Net;

namespace Sif.Framework.Utils
{

    /// <summary>
    /// This is a utility class for exceptions.
    /// </summary>
    public static class ExceptionUtils
    {

        /// <summary>
        /// Build an appropriate error message from the exception (if a WebException).
        /// </summary>
        /// <param name="exception">Exception to check.</param>
        /// <returns>An appropriate error message.</returns>
        public static string InferErrorResponseMessage(Exception exception)
        {
            string message = "";

            if (exception != null && exception is WebException)
            {
                WebException webException = (WebException)exception;

                if (webException.Response != null && webException.Response is HttpWebResponse)
                {
                    HttpWebResponse httpWebResponse = (HttpWebResponse)webException.Response;
                    message = httpWebResponse.StatusCode + " - " + httpWebResponse.StatusDescription;
                }

            }

            return message;
        }

    }

}
