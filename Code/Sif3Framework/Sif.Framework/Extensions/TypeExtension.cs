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
using System.Linq;

namespace Sif.Framework.Extensions
{
    /// <summary>
    /// This class defines extension methods used throughout this framework.
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// Extension of types to check whether one is assignable from a generic type.
        /// </summary>
        /// <param name="givenType">Given type to check.</param>
        /// <param name="genericType">Generic type assignable from.</param>
        /// <returns></returns>
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            bool isAssignable =
                givenType.GetInterfaces().Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType) ||
                (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType) ||
                (givenType.BaseType != null && IsAssignableToGenericType(givenType.BaseType, genericType));

            return isAssignable;
        }
    }
}