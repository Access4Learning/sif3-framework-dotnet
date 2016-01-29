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

using Sif.Framework.WebApi.RouteConstraints;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace Sif.Framework.WebApi.ControllerSelectors
{

    /// <summary>
    /// This class enhances the default controller selector to take into account Matrix Parameters.
    /// <para>This class is based upon code from the ASP.NET Web API Samples.</para>
    /// <see cref="http://aspnet.codeplex.com/sourcecontrol/latest#Samples/WebApi/NamespaceControllerSelector/ReadMe.txt">
    /// Namespace Controller Selector Sample
    /// </see>
    /// </summary>
    public class ServiceProviderHttpControllerSelector : DefaultHttpControllerSelector
    {

        /// <summary>
        /// <see cref="DefaultHttpControllerSelector(HttpConfiguration)">DefaultHttpControllerSelector</see>
        /// </summary>
        public ServiceProviderHttpControllerSelector(HttpConfiguration config)
            : base(config)
        {
        }

        /// <summary>
        /// This method will additionally parse out Matrix Parameters from the controller name.
        /// <see cref="DefaultHttpControllerSelector.GetControllerName(HttpRequestMessage)">GetControllerName</see>
        /// </summary>
        public override string GetControllerName(HttpRequestMessage request)
        {
            string controllerName = base.GetControllerName(request);
            string parsedControllerName = null;

            if (controllerName != null)
            {
                parsedControllerName = Regex.Match(controllerName, SegmentPrefixConstraint.ControllerNamePattern).Groups[1].Value;
            }

            return parsedControllerName;
        }

    }

}