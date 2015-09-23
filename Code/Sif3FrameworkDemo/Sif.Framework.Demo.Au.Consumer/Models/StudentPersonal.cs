﻿/*
 * Copyright 2015 Systemic Pty Ltd
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
using System;
using System.Xml.Serialization;

namespace Sif.Framework.Demo.Au.Consumer.Models
{

    /// <summary>
    /// 
    /// </summary>
    [XmlRootAttribute(Namespace = "http://www.sifassociation.org/au/datamodel/1.4", IsNullable = false)]
    public partial class StudentPersonal : IPersistable<Guid>
    {

        /// <summary>
        /// 
        /// </summary>
        [XmlAttributeAttribute("RefId")]
        public virtual Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string LocalId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual PersonInfo PersonInfo { get; set; }

    }

}
