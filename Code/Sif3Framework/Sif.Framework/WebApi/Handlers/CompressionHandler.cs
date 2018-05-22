/*
 * Copyright 2018 Systemic Pty Ltd
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

using Sif.Framework.Service.Compression;
using Sif.Framework.WebApi.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Sif.Framework.WebApi.Handlers
{
    /// <summary>
    /// Message handler for managing HTTP content compression. This handler is based on the solution provided in the
    /// article <see cref="http://www.ronaldrosier.net/blog/2013/07/16/implement_compression_in_aspnet_web_api">Implement compression in ASP.NET Web API</see>.
    /// </summary>
    public class CompressionHandler : DelegatingHandler
    {
        private readonly List<ICompressor> compressors = new List<ICompressor>();

        /// <summary>
        /// Create an instance of this handler based on the deflat and gzip compressors.
        /// </summary>
        public CompressionHandler() : this(new GZipCompressor(), new DeflateCompressor())
        {
        }

        /// <summary>
        /// Create an instance of this handler.
        /// </summary>
        /// <param name="compressors">Compressors used by this handler.</param>
        protected CompressionHandler(params ICompressor[] compressors) => this.compressors.AddRange(compressors);

        /// <summary>
        /// <see cref="DelegatingHandler.SendAsync(HttpRequestMessage, CancellationToken)"/>
        /// </summary>
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Handle compressed content.
            if (request.Content.Headers.ContentEncoding != null && request.Content.Headers.ContentEncoding.Any())
            {
                foreach (string encoding in request.Content.Headers.ContentEncoding)
                {
                    ICompressor compressor = compressors.SingleOrDefault(c => c.EncodingType.Equals(encoding, StringComparison.InvariantCultureIgnoreCase));

                    if (compressor != null)
                    {
                        request.Content = new DecompressedContent(request.Content, compressor);
                        break;
                    }
                }
            }

            // Handle decompressed content.
            if (request.Headers.AcceptEncoding != null && request.Headers.AcceptEncoding.Any())
            {
                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

                if (response.Content == null)
                {
                    return response;
                }

                foreach (StringWithQualityHeaderValue encoding in request.Headers.AcceptEncoding)
                {
                    ICompressor compressor = compressors.FirstOrDefault(c => c.EncodingType.Equals(encoding.Value, StringComparison.InvariantCultureIgnoreCase));

                    if (compressor != null)
                    {
                        response.Content = new CompressedContent(response.Content, compressor);
                        break;
                    }
                }

                return response;
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}