/*
 * Copyright 2022 Systemic Pty Ltd
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

using Microsoft.AspNetCore.Mvc.Formatters;
using Sif.Framework.Services.Serialisation;
using System.Xml;
using System.Xml.Serialization;

namespace Sif.Framework.AspNetCore.Formatters;

/// <summary>
/// Remove the ArrayOf prefix and append an "s" postfix when serialising a collection of objects.
/// </summary>
public class ArrayOfOutputFormatter<T> : XmlSerializerOutputFormatter
{
    /// <inheritdoc />
    protected override bool CanWriteType(Type type)
    {
        return typeof(IEnumerable<T>).IsAssignableFrom(type);
    }

    /// <inheritdoc />
    // ReSharper disable once RedundantAssignment
    protected override void Serialize(XmlSerializer xmlSerializer, XmlWriter xmlWriter, object value)
    {
        XmlRootAttribute? xmlRootAttribute = null;

        // Check for an XML root attribute in the generic type.
        if (Attribute.GetCustomAttribute(typeof(T), typeof(XmlRootAttribute)) is XmlRootAttribute existingAttribute)
        {
            xmlRootAttribute = new XmlRootAttribute($"{existingAttribute.ElementName}s")
            {
                Namespace = existingAttribute.Namespace,
                IsNullable = existingAttribute.IsNullable
            };
        }

        ISerialiser<List<T>> serialiser = SerialiserFactory.GetXmlSerialiser<List<T>>(xmlRootAttribute);
        xmlSerializer = (XmlSerializer)serialiser;
        xmlSerializer.Serialize(xmlWriter, value);
    }
}