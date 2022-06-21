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

namespace Sif.Framework.Model.Events
{

    /// <summary>
    /// Interface to allow Providers to iterate through SIF Events.
    /// </summary>
    public interface IEventIterator<TMultiple>
    {

        /// <summary>
        /// This method returns the next available SIF Event.
        /// </summary>
        /// <returns>The next available SIF Event. This must return null if there are no further SIF Events available.</returns>
        /// <exception cref="Exceptions.EventException">All errors should be wrapped by this exception.</exception>
        SifEvent<TMultiple> GetNext();

        /// <summary>
        /// This method will check whether there are more SIF Events available, i.e. a call to GetNext() will not
        /// return null.
        /// </summary>
        /// <returns>True if there are more SIF Events available; False otherwise.</returns>
        /// <exception cref="Exceptions.EventException">All errors should be wrapped by this exception.</exception>
        bool HasNext();

    }

}
