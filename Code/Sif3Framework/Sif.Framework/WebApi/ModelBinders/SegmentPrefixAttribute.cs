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

using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace Sif.Framework.WebApi.ModelBinders
{

    /// <summary>
    /// Used to bind the segment prefix value from the route.
    /// <para>This class is based upon code from the ASP.NET Web API Samples.</para>
    /// <see cref="https://aspnet.codeplex.com/SourceControl/latest#Samples/WebApi/RouteConstraintsAndModelBindersSample/ReadMe.txt">
    /// Route Constraints and Model Binders by Implementing Matrix Parameters Sample
    /// </see>
    /// </summary>
    /// <example>
    /// If [Route["{fruits}/{location}"] is specified and the incoming URI's relative path is
    /// "/apples:color=red,green/washington;rate=good", then in the action's argument list, <c>[SegmentPrefix] string
    /// fruits</c> will have fruits = apples but <c>string location</c> without this attribute will have
    /// location = washington;rate=good.
    /// </example>
    public class SegmentPrefixAttribute : ModelBinderAttribute
    {

        /// <summary>
        /// <see cref="ModelBinderAttribute.GetBinding(HttpParameterDescriptor)">GetBinding</see>
        /// </summary>
        public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
        {

            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            HttpConfiguration config = parameter.Configuration;
            IEnumerable<ValueProviderFactory> valueProviderFactories = GetValueProviderFactories(config);

            return new ModelBinderParameterBinding(parameter, new SegmentPrefixModelBinder(), valueProviderFactories);
        }

    }

}
