/*
 * Copyright 2014 Systemic Pty Ltd
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

using Sif.Framework.Model.Persistence;

namespace Sif.Framework.Model.Infrastructure
{

    public class ApplicationInfo : IPersistable<long>
    {

        public virtual long Id { get; set; }

        public virtual ProductIdentity AdapterProduct { get; set; }

        /// <summary>
        /// An opaque (to the SIF standard) element which contains any required Consumer authentication information.
        /// The content of this element is site-specific.  For a Direct Environment which accepts Consumer Registration
        /// Requests from a mobile application, this element might contain a combination of the User ID and Password.
        /// </summary>
        public virtual string ApplicationKey { get; set; }

        public virtual ProductIdentity ApplicationProduct { get; set; }

        public virtual string DataModelNamespace { get; set; }

        /// <summary>
        /// The version of the SIF infrastructure which the Consumer supports.
        /// </summary>
        public virtual string SupportedInfrastructureVersion { get; set; }

        /// <summary>
        /// The transport which the Consumer expects the infrastructure to use to interoperate with it.  The default is
        /// whichever transport the create request was issued on
        /// </summary>
        public virtual string Transport { get; set; }

    }

}
