/*
 * Copyright 2017 Systemic Pty Ltd
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

using Sif.Framework.Model.DataModels;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Utils;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sif.Framework.Consumers
{

    /// <summary>
    /// This is a convenience class for Consumers of SIF data model objects whereby the primary key is of type
    /// System.String and the multiple objects entity is represented as a list of single objects.
    /// </summary>
    /// <typeparam name="T">Type that defines a SIF data model object.</typeparam>
    public class BasicConsumer<T> : Consumer<T, List<T>, string>, IBasicConsumer<T> where T : ISifRefId<string>
    {

        /// <summary>
        /// <see cref="Consumer{TSingle,TMultiple,TPrimaryKey}.Consumer(Model.Infrastructure.Environment)">Consumer</see>
        /// </summary>
        public BasicConsumer(Model.Infrastructure.Environment environment)
            : base(environment)
        {
        }

        /// <summary>
        /// <see cref="Consumer{TSingle,TMultiple,TPrimaryKey}.Consumer(string, string, string, string)">Consumer</see>
        /// </summary>
        public BasicConsumer(string applicationKey, string instanceId = null, string userToken = null, string solutionId = null)
            : base(applicationKey, instanceId, userToken, solutionId)
        {
        }

        /// <summary>
        /// <see cref="Consumer{TSingle,TMultiple,TPrimaryKey}.SerialiseMultiple(TMultiple)">SerialiseMultiple</see>
        /// </summary>
        protected override string SerialiseMultiple(List<T> obj)
        {
            XmlRootAttribute xmlRootAttribute = new XmlRootAttribute(TypeName + "s") { Namespace = SettingsManager.ConsumerSettings.DataModelNamespace, IsNullable = false };
            return SerialiserFactory.GetXmlSerialiser<List<T>>(xmlRootAttribute).Serialise(obj);
        }

        /// <summary>
        /// <see cref="Consumer{TSingle,TMultiple,TPrimaryKey}.DeserialiseMultiple(string)">DeserialiseMultiple</see>
        /// </summary>
        protected override List<T> DeserialiseMultiple(string payload)
        {
            XmlRootAttribute xmlRootAttribute = new XmlRootAttribute(TypeName + "s") { Namespace = SettingsManager.ConsumerSettings.DataModelNamespace, IsNullable = false };
            return SerialiserFactory.GetXmlSerialiser<List<T>>(xmlRootAttribute).Deserialise(payload);
        }

    }

}
