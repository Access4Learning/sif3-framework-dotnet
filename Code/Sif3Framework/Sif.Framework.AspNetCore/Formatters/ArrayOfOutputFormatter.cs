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
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Serialisation;
using System.Xml;
using System.Xml.Serialization;

namespace Sif.Framework.AspNetCore.Formatters;

/// <summary>
/// Remove the ArrayOf prefix and append an "s" postfix when serialising a collection of objects.
/// </summary>
public class ArrayOfOutputFormatter<TObject> : XmlSerializerOutputFormatter
{
    private readonly IFrameworkSettings _settings;

    /// <summary>
    /// Create an instance of this formatter using the settings provided.
    /// </summary>
    /// <param name="settings">Settings used to configure this formatter.</param>
    public ArrayOfOutputFormatter(IFrameworkSettings settings)
    {
        _settings = settings;
    }

    /// <inheritdoc />
    protected override bool CanWriteType(Type type)
    {
        return typeof(IEnumerable<TObject>).IsAssignableFrom(type);
    }

    /// <inheritdoc />
    // ReSharper disable once RedundantAssignment
    protected override void Serialize(XmlSerializer xmlSerializer, XmlWriter xmlWriter, object value)
    {
        var xmlRootAttribute = new XmlRootAttribute($"{typeof(TObject).Name}s")
        {
            Namespace = _settings.DataModelNamespace,
            IsNullable = false
        };

        ISerialiser<List<TObject>> serialiser = SerialiserFactory.GetXmlSerialiser<List<TObject>>(xmlRootAttribute);
        xmlSerializer = (XmlSerializer)serialiser;
        xmlSerializer.Serialize(xmlWriter, value);
    }
}