﻿/*
 * Copyright 2015 Systemic Pty Ltd
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
    public class BaseException : Exception
    {
        private string exceptionReference;

        /// <summary>
        /// <see cref="System.Exception()"/>
        /// </summary>
        public BaseException()
            : base()
        {
            exceptionReference = GenerateUniqueReference();
        }

        /// <summary>
        /// <see cref="System.Exception(System.String)"/>
        /// </summary>
        public BaseException(string message)
            : base(message)
        {
            exceptionReference = GenerateUniqueReference();
        }

        /// <summary>
        /// <see cref="System.Exception(System.String, System.Exception)"/>
        /// </summary>
        public BaseException(string message, Exception inner)
            : base(message, inner)
        {
            exceptionReference = GenerateUniqueReference();
        }

        /// <summary>
        /// <see cref="System.Exception(System.Runtime.Serialization.SerializationInfo, System.Runtime.Serialization.StreamingContext)"/>
        /// </summary>
        protected BaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            exceptionReference = GenerateUniqueReference();
        }

        /// <summary>
        /// This method will generate a unique reference to be associated with an exception.
        /// </summary>
        /// <returns>A unique reference.</returns>
        private string GenerateUniqueReference()
        {
            return "[EXCEPTION_REF=" + Math.Abs(Guid.NewGuid().GetHashCode()) + "]";
        }

        /// <summary>
        /// This method will prefix the exception message with a unique reference.
        /// </summary>
        public override string Message
        {

            get
            {
                return exceptionReference + " " + base.Message;
            }

        }

    }

}
