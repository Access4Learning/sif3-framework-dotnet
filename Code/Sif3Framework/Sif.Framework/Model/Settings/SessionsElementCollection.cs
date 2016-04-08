/*
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

using System.Configuration;

namespace Sif.Framework.Model.Settings
{

    /// <summary>
    /// Represents the "sessions" configuration element containing a collection of child "session" elements.
    /// </summary>
    [ConfigurationCollection(typeof(SessionElement), AddItemName = SessionElement.ElementReference, CollectionType = ConfigurationElementCollectionType.BasicMap)]
    class SessionsElementCollection : ConfigurationElementCollection
    {
        public const string ElementCollectionReference = "sessions";

        /// <summary>
        /// Collection indexer based upon an integer index.
        /// </summary>
        /// <param name="index">Index into the collection.</param>
        /// <returns>Session element at the specified index.</returns>
        SessionElement this[int index]
        {

            get
            {
                return (SessionElement)base.BaseGet(index);
            }

            set
            {

                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }

                base.BaseAdd(index, value);
            }

        }

        /// <summary>
        /// Readonly collection indexer based upon a string index.
        /// </summary>
        /// <param name="name">Index into the collection.</param>
        /// <returns>Session element at the specified index.</returns>
        public new SessionElement this[string name]
        {

            get
            {
                return (SessionElement)base.BaseGet(name);
            }

        }

        /// <summary>
        /// <see cref="System.Configuration.CreateNewElement()"/>
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new SessionElement();
        }

        /// <summary>
        /// <see cref="System.Configuration.GetElementKey(System.Configuration.ConfigurationElement)"/>
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SessionElement)element).SessionToken;
        }

        /// <summary>
        /// <see cref="System.Configuration.ConfigurationElementCollection.BaseAdd(System.Configuration.ConfigurationElement)"/>
        /// </summary>
        public void Add(SessionElement session)
        {
            base.BaseAdd(session);
        }

        /// <summary>
        /// <see cref="System.Configuration.ConfigurationElementCollection.BaseClear()"/>
        /// </summary>
        public void Clear()
        {
            base.BaseClear();
        }

        /// <summary>
        /// <see cref="System.Configuration.ConfigurationElementCollection.BaseGetKey(System.Int32)"/>
        /// </summary>
        public string GetKey(int index)
        {
            return (string)base.BaseGetKey(index);
        }

        /// <summary>
        /// <see cref="System.Configuration.ConfigurationElementCollection.BaseRemove(System.String)"/>
        /// </summary>
        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        /// <summary>
        /// Removes the passed session element from the collection.
        /// <see cref="System.Configuration.ConfigurationElementCollection.BaseRemove(System.Object)"/>
        /// </summary>
        public void Remove(SessionElement session)
        {
            base.BaseRemove(GetElementKey(session));
        }

        /// <summary>
        /// <see cref="System.Configuration.ConfigurationElementCollection.RemoveAt(System.Int32)"/>
        /// </summary>
        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

    }

}
