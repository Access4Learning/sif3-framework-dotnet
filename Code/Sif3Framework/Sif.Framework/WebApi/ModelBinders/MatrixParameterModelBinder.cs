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
using System.Collections.Specialized;
using System.Linq;
using System.Web;
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
    public class MatrixParameterModelBinder : IModelBinder
    {
        private readonly string segment;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="segment">
        /// Can be empty, a target prefix value, or a route parameter name embeded in "{" and "}".
        /// </param>
        /// <example>
        /// If segment is null or empty, the parameter will match all color values from the whole path.
        /// If segment is "oranges", the parameter will match color only from the segment starting with "oranges" like
        /// .../oranges;color=red/...
        /// If segment is "{fruits}", the parameter will match color only from the route .../{fruits}/...
        /// </example>
        public MatrixParameterModelBinder(string segment)
        {
            this.segment = segment;
        }

        /// <summary>
        /// Can be empty, a target prefix value, or a route parameter name embeded in "{" and "}".
        /// </summary>
        public string Segment
        {

            get { return segment; }

        }

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

            string attributeToBind = bindingContext.ModelName;

            // Match the route segment like [Route("{fruits}")] if possible.
            if (!string.IsNullOrEmpty(segment) && segment.StartsWith("{", StringComparison.Ordinal) && segment.EndsWith("}", StringComparison.Ordinal))
            {
                string segmentName = segment.Substring(1, segment.Length - 2);
                ValueProviderResult segmentResult = bindingContext.ValueProvider.GetValue(segmentName);

                if (segmentResult == null)
                {
                    return false;
                }

                string matrixParamSegment = segmentResult.AttemptedValue;

                if (matrixParamSegment == null)
                {
                    return false;
                }

                IList<string> attributeValues = GetAttributeValues(matrixParamSegment, attributeToBind);

                if (attributeValues != null)
                {
                    bindingContext.Model = attributeValues.ToArray();
                }

                return true;
            }

            // Match values from segments like .../apples;color=red/..., then.
            ICollection<object> values = actionContext.ControllerContext.RouteData.Values.Values;

            // Expand in case that a catch-all constraint will deliver a segment with "/" in it.
            List<string> paramSegments = new List<string>();

            foreach (string value in values)
            {
                paramSegments.AddRange(value.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries));
            }

            List<string> collectedAttributeValues = new List<string>();

            foreach (string paramSegment in paramSegments)
            {

                // If no parameter is specified, as [MatrixParam], get values from all the segments.
                // If a segment prefix is specified like [MatrixParam("apples")], get values only it is matched.
                if (!String.IsNullOrEmpty(segment) && !paramSegment.StartsWith(segment + ";", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                IList<string> attributeValues = GetAttributeValues(paramSegment, attributeToBind);

                if (attributeValues != null)
                {
                    collectedAttributeValues.AddRange(attributeValues);
                }

            }

            bindingContext.Model = collectedAttributeValues.ToArray();

            return (collectedAttributeValues.Count > 0);
        }

        /// <summary>
        /// Get the attribute values for an attribute.
        /// </summary>
        /// <param name="matrixParamSegment">Matrix parameter segment.</param>
        /// <param name="attributeName">Attribute name.</param>
        /// <returns>List of attribute values.</returns>
        private IList<string> GetAttributeValues(string matrixParamSegment, string attributeName)
        {
            NameValueCollection valuesCollection = HttpUtility.ParseQueryString(matrixParamSegment.Replace(";", "&"));
            string attributeValueList = valuesCollection.Get(attributeName);

            if (attributeValueList == null)
            {
                return null;
            }

            return attributeValueList.Split(',');
        }

    }

}
