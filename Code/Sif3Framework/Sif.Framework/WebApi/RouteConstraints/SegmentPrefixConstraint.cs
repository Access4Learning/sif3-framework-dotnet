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
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Sif.Framework.WebApi.RouteConstraints
{
    /// <summary>
    /// This constraint will match a route starting with the segmentPrefix + ";" or the segmentPrefix only.
    /// <para>This class is based upon code from the ASP.NET Web API Samples.</para>
    /// <see cref="https://aspnet.codeplex.com/SourceControl/latest#Samples/WebApi/RouteConstraintsAndModelBindersSample/ReadMe.txt">
    /// Route Constraints and Model Binders by Implementing Matrix Parameters Sample
    /// </see>
    /// </summary>
    /// <example>
    /// If Route["{apples:SegmentPrefix}"] is specified, .../apples;color=red/... or .../apples will match, but
    /// .../apples?color=red will not.
    /// </example>
    public class SegmentPrefixConstraint : IHttpRouteConstraint
    {
        internal static readonly string ControllerNamePattern = @"(\w+);?";

        private const string ControllerKey = "controller";

        /// <summary>
        /// <see cref="IHttpRouteConstraint.Match(HttpRequestMessage, IHttpRoute, string, IDictionary{string, object}, HttpRouteDirection)">Match</see>
        /// </summary>
        public bool Match(HttpRequestMessage request,
                          IHttpRoute route,
                          string segmentPrefix,
                          IDictionary<string, object> values,
                          HttpRouteDirection routeDirection)
        {

            if (segmentPrefix == null)
            {
                throw new ArgumentNullException("segmentPrefix");
            }

            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            bool match = false;

            // If the segmentPrefix is "controller", then the route constraint has been based on the routing tables
            // of the WebApiConfig.cs file. In this case, the URI will need to be parsed to remove Matrix Parameters
            // before checking against recognised controllers.
            if (ControllerKey.Equals(segmentPrefix))
            {
                string[] segments = request.RequestUri.Segments;
                Array.Reverse(segments);

                foreach (string segment in segments)
                {

                    if (Regex.IsMatch(segment, ControllerNamePattern))
                    {
                        string controllerName = Regex.Match(segment, ControllerNamePattern).Groups[1].Value;
                        match = GlobalConfiguration.Configuration.Services.GetHttpControllerSelector().GetControllerMapping().ContainsKey(controllerName);

                        if (match)
                        {
                            break;
                        }

                    }

                }

            }
            // If the segmentPrefix is some other value, then the route constraint has been defined using attribute 
            // routing and refers to a controller name.
            else
            {
                object value;

                if (values.TryGetValue(segmentPrefix, out value))
                {
                    string valueString = (string)value;
                    match = valueString != null &&
                        (valueString.StartsWith(segmentPrefix + ";", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(valueString, segmentPrefix, StringComparison.OrdinalIgnoreCase));
                }

            }

            return match;
        }

    }

}
