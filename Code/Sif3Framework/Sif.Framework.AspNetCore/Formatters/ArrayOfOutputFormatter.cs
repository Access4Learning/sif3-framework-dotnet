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
using Sif.Framework.Service.Serialisation;
using System.Xml;
using System.Xml.Serialization;

namespace Sif.Framework.AspNetCore.Formatters;

/// <summary>
/// Remove the ArrayOf prefix and append an "s" postfix when serialising a collection of objects.
/// </summary>
public class ArrayOfOutputFormatter<T> : XmlSerializerOutputFormatter
{
    private readonly string? _dataModelNamespace;

    /// <summary>
    /// Create an instance of this formatter with the namespace provided.
    /// </summary>
    /// <param name="dataModelNamespace">Data mode namespace.</param>
    public ArrayOfOutputFormatter(string? dataModelNamespace)
    {
        _dataModelNamespace = dataModelNamespace;
    }

    /// <inheritdoc />
    protected override bool CanWriteType(Type type)
    {
        return typeof(IEnumerable<T>).IsAssignableFrom(type);
    }

    /// <inheritdoc />
    // ReSharper disable once RedundantAssignment
    protected override void Serialize(XmlSerializer xmlSerializer, XmlWriter xmlWriter, object value)
    {
        var xmlRootAttribute = new XmlRootAttribute($"{typeof(T).Name}s")
        {
            Namespace = _dataModelNamespace,
            IsNullable = false
        };

        ISerialiser<List<T>> serialiser = SerialiserFactory.GetXmlSerialiser<List<T>>(xmlRootAttribute);
        xmlSerializer = (XmlSerializer)serialiser;
        xmlSerializer.Serialize(xmlWriter, value);
    }
}