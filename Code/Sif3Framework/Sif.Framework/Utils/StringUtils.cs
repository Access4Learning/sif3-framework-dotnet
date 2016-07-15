/*
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

namespace Sif.Framework.Utils
{

    /// <summary>
    /// This is a utility class for string operations.
    /// </summary>
    static class StringUtils
    {

        /// <summary>
        /// Remove new line characters from a string (by replacing with a space character).
        /// </summary>
        /// <param name="content">String content to parse.</param>
        /// <returns>Null if content is null; content with no new line characters otherwise.</returns>
        public static string RemoveNewLines(string content)
        {
            return (content == null ? null : content.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " "));
        }

        public static bool IsEmpty(string content)
        {
            return String.IsNullOrWhiteSpace(content);
        }

        public static bool NotEmpty(string content)
        {
            return !String.IsNullOrWhiteSpace(content);
        }

        public static bool IsEmpty(Guid content)
        {
            return content == null || StringUtils.IsEmpty(content.ToString());
        }

        public static bool NotEmpty(Guid content)
        {
            return content != null && StringUtils.NotEmpty(content.ToString());
        }
    }
}
