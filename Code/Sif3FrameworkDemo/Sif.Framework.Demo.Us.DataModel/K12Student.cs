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
using Sif.Specification.DataModel.Us;
using System.Xml.Serialization;

namespace Sif.Framework.Demo.Us.DataModel
{

    /// <summary>
    /// 
    /// </summary>
    [XmlTypeAttribute(Namespace = "http://www.sifassociation.org/datamodel/na/3.2")]
    [XmlRootAttribute("k12Student", Namespace = "http://www.sifassociation.org/datamodel/na/3.2", IsNullable = false)]
    public class K12Student : k12StudentType, IPersistable<string>
    {

        /// <summary>
        /// 
        /// </summary>
        public string Id
        {

            get
            {
                return refId;
            }
            set
            {
                refId = value;
            }

        }

    }

}
