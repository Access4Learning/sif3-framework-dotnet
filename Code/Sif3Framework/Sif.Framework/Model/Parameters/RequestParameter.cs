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

using System;

namespace Sif.Framework.Model.Parameters
{
    /// <summary>
    /// Message parameter used for Requests.
    /// </summary>
    public class RequestParameter : MessageParameter
    {
        /// <summary>
        /// Create an instance of a request parameter.
        /// </summary>
        /// <param name="name">Name of the request parameter.</param>
        /// <param name="type">Conveyance type for the request parameter.</param>
        /// <param name="value">Value associated with the request parameter.</param>
        /// <exception cref="ArgumentNullException">Either name and/or value are null or empty.</exception>
        public RequestParameter(string name, ConveyanceType type, string value) : base(name, value)
        {
            Type = type;
        }

        /// <summary>
        /// Conveyance type for the request parameter.
        /// </summary>
        public ConveyanceType Type { get; }
    }
}