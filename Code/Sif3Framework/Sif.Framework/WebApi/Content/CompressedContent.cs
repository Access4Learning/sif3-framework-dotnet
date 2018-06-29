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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sif.Framework.WebApi.Content
{
    /// <summary>
    /// Represents compressed HTTP content.
    /// </summary>
    internal class CompressedContent : CompressionContent
    {
        /// <summary>
        /// <see cref="CompressionContent(HttpContent, ICompressor)"/>
        /// </summary>
        public CompressedContent(HttpContent content, ICompressor compressor) : base(content, compressor)
        {
        }

        /// <summary>
        /// Serialise the original content to a compressed stream.
        /// <see cref="HttpContent.SerializeToStreamAsync(Stream, TransportContext)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">stream is null.</exception>
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            Stream compressedStream = Compressor.Compress(stream);

            return OriginalContent.CopyToAsync(compressedStream).ContinueWith(task =>
            {
                if (compressedStream != null)
                {
                    compressedStream.Dispose();
                }
            });
        }
    }
}