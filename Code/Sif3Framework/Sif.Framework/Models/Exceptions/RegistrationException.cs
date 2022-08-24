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
    /// This exception represents an error from the Registration service.
    /// </summary>
    [Serializable]
    public class RegistrationException : BaseException
    {
        /// <inheritdoc />
        public RegistrationException()
        {
        }

        /// <inheritdoc />
        protected RegistrationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public RegistrationException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public RegistrationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}