﻿/*
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
using System.Xml.Serialization;

namespace Sif.Framework.Demo.Provider.Models
{

    public partial class Name : IPersistable<long>
    {

        [XmlIgnoreAttribute]
        public virtual long Id { get; set; }

        public virtual string FamilyName { get; set; }

        public virtual string GivenName { get; set; }

        [XmlAttributeAttribute()]
        public virtual NameType Type { get; set; }

    }

}
