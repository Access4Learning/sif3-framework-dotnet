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

using Sif.Framework.AspNet.ActionResults;
using System.Web.Http.ExceptionHandling;

namespace Sif.Framework.AspNet.Handlers
{
    /// <summary>
    /// This exception handler produces a custom error response to clients for unexpected exceptions.
    /// </summary>
    public class GlobalUnexpectedExceptionHandler : ExceptionHandler
    {
        /// <summary>
        /// This method will handle the exception synchronously.
        /// </summary>
        /// <param name="context">The exception handler context.</param>
        public override void Handle(ExceptionHandlerContext context)
        {
            context.Result = new UnexpectedExceptionResult(context.Request, context.Exception);
        }
    }
}