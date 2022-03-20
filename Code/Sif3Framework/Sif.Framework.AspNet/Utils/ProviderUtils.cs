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

using Sif.Framework.AspNet.Controllers;
using Sif.Framework.AspNet.Providers;
using Sif.Framework.Extensions;
using Sif.Framework.Providers;
using System;
using System.Web.Http.Controllers;

namespace Sif.Framework.AspNet.Utils
{
    public class ProviderUtils
    {
        /// <summary>
        /// Returns true if the given type is a controller, false otherwise.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>See def.</returns>
        public static bool IsController(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            bool isController = type.IsClass &&
                type.IsVisible &&
                !type.IsAbstract &&
                (type.IsAssignableToGenericType(typeof(IProvider<,,>)) ||
                type.IsAssignableToGenericType(typeof(SifController<,>)) ||
                typeof(FunctionalServiceProvider).IsAssignableFrom(type)) &&
                typeof(IHttpController).IsAssignableFrom(type) &&
                type.Name.EndsWith("Provider");

            return isController;
        }
    }
}