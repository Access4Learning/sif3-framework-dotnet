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

using Sif.Framework.Controllers;
using Sif.Framework.Extensions;
using Sif.Framework.Providers;
using Sif.Framework.Utils;
using System;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Sif.Framework.WebApi
{

    /// <summary>
    /// Controller type resolver that recognises SIF Service Providers.
    /// </summary>
    public class ServiceProviderHttpControllerTypeResolver : DefaultHttpControllerTypeResolver
    {

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServiceProviderHttpControllerTypeResolver()
            : base(IsHttpEndpoint)
        { }

        /// <summary>
        /// Check whether the specified type defines a Controller endpoint.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if the endpoint defines a Controller; false otherwise.</returns>
        internal static bool IsHttpEndpoint(Type type)
        {
            return ProviderUtils.isController(type);
        }

    }

}
