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
using System.Runtime.Serialization;

namespace Sif.Framework.Model.Exceptions
{

    /// <summary>
    /// This exception represents the situation where the message content is too large.
    /// </summary>
    [Serializable]
    public class ContentTooLargeException : BaseException
    {

        /// <summary>
        /// <see cref="BaseException()"/>
        /// </summary>
        public ContentTooLargeException()
            : base("The message content is too large.")
        {
        }

        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public ContentTooLargeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        public ContentTooLargeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// <see cref="BaseException(SerializationInfo, StreamingContext)"/>
        /// </summary>
        protected ContentTooLargeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }

}
