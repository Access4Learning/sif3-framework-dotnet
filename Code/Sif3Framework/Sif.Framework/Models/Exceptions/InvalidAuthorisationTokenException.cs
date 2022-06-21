/*
 * Copyright 2017 Systemic Pty Ltd
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

namespace Sif.Framework.Model.Exceptions
{

    /// <summary>
    /// This exception represents the situation where an authorisation token is not recognised or is invalid.
    /// </summary>
    [Serializable]
    public class InvalidAuthorisationTokenException : BaseException
    {

        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public InvalidAuthorisationTokenException()
            : base("The authorisation token is not recognised or is invalid.")
        {
        }

        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public InvalidAuthorisationTokenException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        public InvalidAuthorisationTokenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// <see cref="BaseException(SerializationInfo, StreamingContext)"/>
        /// </summary>
        protected InvalidAuthorisationTokenException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }

}
