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
using System.Runtime.Serialization;

namespace Sif.Framework.Models.Exceptions
{
    /// <summary>
    /// This exception represents the situation where a requested action or operation is rejected.
    /// </summary>
    [Serializable]
    public class RejectedException : BaseException
    {
        /// <inheritdoc />
        public RejectedException()
        {
        }

        /// <inheritdoc />
        public RejectedException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public RejectedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc />
        protected RejectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}