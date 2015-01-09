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

using System.Diagnostics;
using System.Web.Http.ExceptionHandling;

namespace Sif.Framework.Utils
{

    /// <summary>
    /// This exception logger will send exception data to configured Trace sources (including the Debug output window
    /// in Visual Studio).
    /// </summary>
    public class TraceExceptionLogger : ExceptionLogger
    {

        /// <summary>
        /// This method will log the exception synchronously to configured Trace sources.
        /// </summary>
        /// <param name="context">The exception logger context.</param>
        public override void Log(ExceptionLoggerContext context)
        {
            Trace.TraceError("[Exception.Message]\n" + context.Exception.Message);
            Trace.TraceError("[Exception.StackTrace]\n" + context.Exception.StackTrace);
            Trace.TraceError("[Request]\n" + context.Request);
        }

    }

}
