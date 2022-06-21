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
    /// This class represents the base class for customised exceptions.
    /// </summary>
    [Serializable]
    public abstract class BaseException : Exception
    {

        /// <summary>
        /// Unique identifier associated with this exception.
        /// </summary>
        public string ExceptionReference { get; private set; }

        /// <summary>
        /// <see cref="System.Exception()"/>
        /// </summary>
        public BaseException()
            : base()
        {
            ExceptionReference = GenerateUniqueReference();
        }

        /// <summary>
        /// <see cref="System.Exception(System.String)"/>
        /// </summary>
        public BaseException(string message)
            : base(message)
        {
            ExceptionReference = GenerateUniqueReference();
        }

        /// <summary>
        /// <see cref="System.Exception(System.String, System.Exception)"/>
        /// </summary>
        public BaseException(string message, Exception inner)
            : base(message, inner)
        {
            ExceptionReference = GenerateUniqueReference();
        }

        /// <summary>
        /// <see cref="System.Exception(System.Runtime.Serialization.SerializationInfo, System.Runtime.Serialization.StreamingContext)"/>
        /// </summary>
        protected BaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ExceptionReference = GenerateUniqueReference();
        }

        /// <summary>
        /// This method will generate a unique reference to be associated with an exception.
        /// </summary>
        /// <returns>A unique reference.</returns>
        private string GenerateUniqueReference()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode()).ToString();
        }

        /// <summary>
        /// This method will prefix the exception message with a unique reference.
        /// </summary>
        public override string Message
        {

            get
            {
                return "[EXCEPTION_REF=" + GenerateUniqueReference() + "]" + " " + base.Message;
            }

        }

    }

}
