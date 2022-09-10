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

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sif.Framework.AspNet.ActionResults
{
    /// <summary>
    /// Represents an action result that returns no content. An example use would be as a response to a HEAD request.
    /// </summary>
    public class EmptyContentResult : IHttpActionResult
    {
        private readonly IHttpActionResult _actionResult;

        /// <summary>
        /// Create instance based on an existing action result.
        /// </summary>
        /// <param name="actionResult">Original action result.</param>
        /// <exception cref="ArgumentNullException">actionResult is null.</exception>
        public EmptyContentResult(IHttpActionResult actionResult)
        {
            _actionResult = actionResult ?? throw new ArgumentNullException(nameof(actionResult));
        }

        /// <summary>
        /// Clear the contents of the original action result.
        /// <see cref="IHttpActionResult.ExecuteAsync(CancellationToken)"/>
        /// </summary>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await _actionResult.ExecuteAsync(cancellationToken);

            if (!response.IsSuccessStatusCode) return response;

            byte[] oldContent = await response.Content.ReadAsByteArrayAsync();
            var newContent = new StringContent(string.Empty);
            newContent.Headers.Clear();

            foreach (KeyValuePair<string, IEnumerable<string>> oldHeader in response.Content.Headers)
            {
                newContent.Headers.Add(oldHeader.Key, oldHeader.Value);
            }

            newContent.Headers.ContentLength = oldContent.Length;
            response.Content = newContent;

            return response;
        }
    }
}