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

using System.Collections.Generic;

namespace Sif.Framework.Service.Providers
{

    /// <summary>
    /// This is a convenience interface that defines the services available for Providers of SIF data model objects
    /// whereby the primary key is of type System.String and the multiple objects entity is represented as a list of
    /// single objects.
    /// </summary>
    /// <typeparam name="T">SIF data model object type.</typeparam>
    public interface IBasicProviderService<T> : IObjectService<T, List<T>, string>
    {
    }

}
