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

using Sif.Framework.WebApi;
using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Http;

namespace Sif.Framework.Extensions
{

    /// <summary>
    /// This class defines extension methods used throughout this framework.
    /// </summary>
    static class Extension
    {

        /// <summary>
        /// Extension of types to check whether one is assignable from a generic type.
        /// </summary>
        /// <param name="givenType">Given type to check.</param>
        /// <param name="genericType">Generic type assignable from.</param>
        /// <returns></returns>
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            bool isAssignable = givenType.GetInterfaces().Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType) ||
                (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType) ||
                (givenType.BaseType != null && IsAssignableToGenericType(givenType.BaseType, genericType));

            return isAssignable;
        }

        /// <summary>
        /// Extension to the ApiController that adds an error message to the NotFound result.
        /// </summary>
        /// <param name="controller">Web API Rest Controller.</param>
        /// <param name="message">Error message.</param>
        /// <returns></returns>
        public static NotFoundErrorMessageResult NotFound(this ApiController controller, string message)
        {
            NotFoundErrorMessageResult result = new NotFoundErrorMessageResult(controller.Request, message);

            return result;
        }

        /// <summary>
        /// Extension to enumerated types that recognise the description attribute.
        /// </summary>
        /// <param name="enumeration">Enumerated type.</param>
        /// <returns>Description attribute if it exists; result of ToString() otherwise.</returns>
        public static string ToDescription(this Enum enumeration)
        {
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)enumeration.GetType()
                .GetField(enumeration.ToString())
                .GetCustomAttributes(false)
                .Where(a => a is DescriptionAttribute)
                .FirstOrDefault();
            string description = (descriptionAttribute != null ? descriptionAttribute.Description : enumeration.ToString());

            return description;
        }

    }

}
