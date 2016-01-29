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
    /// Used to bind matrix parameter values from the URI.
    /// <para>This class is based upon code from the ASP.NET Web API Samples.</para>
    /// <see cref="https://aspnet.codeplex.com/SourceControl/latest#Samples/WebApi/RouteConstraintsAndModelBindersSample/ReadMe.txt">
    /// Route Constraints and Model Binders by Implementing Matrix Parameters Sample
    /// </see>
    /// </summary>
    public class MatrixParameterAttribute : ModelBinderAttribute
    {
        private readonly string segment;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <example>
        /// <c>[MatrixParam] string[] color</c> will match all color values from the whole path.
        /// </example>
        public MatrixParameterAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="segment">
        /// Can be empty, a target prefix value, or a general route name embeded in "{" and "}".
        /// </param>
        /// <example>
        /// <c>[MatrixParam("")] string[] color</c> will match all color values from the whole path.
        /// <c>[MatrixParam("oranges")] string[] color</c> will match color only from the segment starting with
        /// "oranges" like .../oranges;color=red/...
        /// <c>[MatrixParam("{fruits}")] string[] color</c> will match color only from the route .../{fruits}/...
        /// </example>
        public MatrixParameterAttribute(string segment)
        {
            this.segment = segment;
        }

        /// <summary>
        /// Can be empty, a target prefix value, or a general route name embeded in "{" and "}".
        /// </summary>
        public string Segment
        {

            get { return segment; }

        }

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

            return new ModelBinderParameterBinding(parameter, new MatrixParameterModelBinder(segment), valueProviderFactories);
        }

    }

}
