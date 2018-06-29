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
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Sif.Framework.WebApi.Content
{
    /// <summary>
    /// Base class for the representation of compressed HTTP content.
    /// </summary>
    internal abstract class CompressionContent : HttpContent
    {
        protected ICompressor Compressor { get; private set; }
        protected HttpContent OriginalContent { get; private set; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="content">Original content.</param>
        /// <param name="compressor">Compressor to be used on the content.</param>
        public CompressionContent(HttpContent content, ICompressor compressor)
        {
            Compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
            OriginalContent = content ?? throw new ArgumentNullException(nameof(content));
            CopyContentHeaders();
        }

        /// <summary>
        /// Copy content header fields from the original content to the new content.
        /// </summary>
        private void CopyContentHeaders()
        {
            foreach (KeyValuePair<string, IEnumerable<string>> header in OriginalContent.Headers)
            {
                Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            Headers.ContentEncoding.Add(Compressor.EncodingType);
        }

        /// <summary>
        /// This method will always return false.
        /// <see cref="HttpContent.TryComputeLength(out long)"/>
        /// </summary>
        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}