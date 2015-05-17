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
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sif.Framework.Utils
{

    /// <summary>
    /// 
    /// </summary>
    public class MethodOverrideHandler : DelegatingHandler
    {
        readonly string[] _methods = { "DELETE", "GET" };
        const string _header = "X-HTTP-Method-Override";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            // Check for HTTP POST with the X-HTTP-Method-Override header.
            if (request.Method == HttpMethod.Post && request.Headers.Contains(_header))
            {
                // Check if the header value is in our methods list.
                var method = request.Headers.GetValues(_header).FirstOrDefault();

                if (_methods.Contains(method, StringComparer.InvariantCultureIgnoreCase))
                {
                    // Change the request method.
                    request.Method = new HttpMethod(method);
                }

            }

            return base.SendAsync(request, cancellationToken);
        }

    }

}

