﻿/*
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

using System.IO;
using System.IO.Compression;

namespace Sif.Framework.Service.Compression
{
    /// <summary>
    /// Compressor for handling the "deflate" encoding type.
    /// </summary>
    public class DeflateCompressor : ICompressor
    {
        /// <inheritdoc cref="ICompressor.EncodingType" />
        public string EncodingType => "deflate";

        /// <inheritdoc cref="ICompressor.Compress(Stream)" />
        public Stream Compress(Stream stream)
        {
            return new DeflateStream(stream, CompressionMode.Compress, true);
        }

        /// <inheritdoc cref="ICompressor.Decompress(Stream)" />
        public Stream Decompress(Stream stream)
        {
            return new DeflateStream(stream, CompressionMode.Decompress, true);
        }
    }
}