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
    /// Message parameter that may be used for Requests, Responses and Events.
    /// </summary>
    public class MessageParameter
    {
        /// <summary>
        /// Create an instance of a message parameter.
        /// </summary>
        /// <param name="name">Name of the message parameter.</param>
        /// <param name="value">Value associated with the message parameter.</param>
        /// <exception cref="ArgumentNullException">Either name and/or value are null or empty.</exception>
        public MessageParameter(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            Name = name.Trim();
            Value = value.Trim();
        }

        /// <summary>
        /// Name of the message parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Value associated with the message parameter.
        /// </summary>
        public string Value { get; }
    }
}