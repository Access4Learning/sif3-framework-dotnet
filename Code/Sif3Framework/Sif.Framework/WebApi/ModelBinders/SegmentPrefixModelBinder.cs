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
using System.Linq;
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
    /// <remarks>
    /// If incoming URI's relative path is "/apples:color=red,green/washington;rate=good", "apples" and "washington"
    /// are segment prefixes.
    /// </remarks>
    public class SegmentPrefixModelBinder : IModelBinder
    {

        /// <summary>
        /// <see cref="IModelBinder.BindModel(HttpActionContext, ModelBindingContext)">BindModel</see>
        /// </summary>
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {

            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }

            string segmentName = bindingContext.ModelName;
            ValueProviderResult segmentResult = bindingContext.ValueProvider.GetValue(segmentName);

            if (segmentResult == null)
            {
                return false;
            }

            string segmentValue = segmentResult.AttemptedValue;

            if (segmentValue != null)
            {
                bindingContext.Model = segmentValue.Split(new[] { ";" }, 2, StringSplitOptions.None).First();
            }
            else
            {
                bindingContext.Model = segmentValue;
            }

            return true;
        }

    }

}
