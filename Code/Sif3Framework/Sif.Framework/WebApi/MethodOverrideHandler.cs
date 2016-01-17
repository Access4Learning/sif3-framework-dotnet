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

using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sif.Framework.WebApi
{

    /// <summary>
    /// Handler for managing method overrides of REST requests.
    /// </summary>
    public class MethodOverrideHandler : DelegatingHandler
    {
        readonly string[] overrideHeaders = { "methodOverride", "X-HTTP-Method-Override" };

        /// <summary>
        /// Handle the override of the POST and PUT methods used for Query by Example and multiple object deletes
        /// respectively.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string overrideHeader = null;

            foreach (string header in overrideHeaders)
            {

                if (request.Headers.Contains(header))
                {
                    overrideHeader = header;
                    break;
                }

            }

            // Check for HTTP POST with the X-HTTP-Method-Override header.
            if (request.Method == HttpMethod.Post && overrideHeader != null)
            {
                // Check if the header value is in our methods list.
                string method = request.Headers.GetValues(overrideHeader).FirstOrDefault();

                //if (overrideMethods.Contains(method, StringComparer.InvariantCultureIgnoreCase))
                if ("GET".Equals(method, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Change the request method.
                    request.Method = new HttpMethod(method);
                }

            }
            // Check for HTTP PUT with the X-HTTP-Method-Override header.
            else if (request.Method == HttpMethod.Put && overrideHeader != null)
            {
                // Check if the header value is in our methods list.
                string method = request.Headers.GetValues(overrideHeader).FirstOrDefault();

                if ("DELETE".Equals(method, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Change the request method.
                    request.Method = new HttpMethod(method);
                }

            }

            return base.SendAsync(request, cancellationToken);
        }

    }

}

