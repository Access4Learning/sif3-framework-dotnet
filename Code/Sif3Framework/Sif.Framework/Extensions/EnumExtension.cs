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
using System.ComponentModel;
using System.Linq;

namespace Sif.Framework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the enum type.
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Extension to enumerated types that recognise the description attribute.
        /// </summary>
        /// <param name="enumeration">Enumerated type.</param>
        /// <returns>Description attribute if it exists; result of ToString() otherwise.</returns>
        public static string ToDescription(this Enum enumeration)
        {
            if (enumeration == null) throw new ArgumentNullException(nameof(enumeration));

            var descriptionAttribute = (DescriptionAttribute)enumeration
                .GetType()
                .GetField(enumeration.ToString())
                ?.GetCustomAttributes(false)
                .FirstOrDefault(a => a is DescriptionAttribute);
            string description =
                descriptionAttribute != null ? descriptionAttribute.Description : enumeration.ToString();

            return description;
        }
    }
}