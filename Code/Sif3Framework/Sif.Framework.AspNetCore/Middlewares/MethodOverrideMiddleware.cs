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

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Sif.Framework.AspNetCore.Middlewares;

/// <inheritdoc />
public class MethodOverrideMiddleware : IMiddleware
{
    private const string MethodOverride = "methodOverride";
    private const string XHttpMethodOverride = "X-Http-Method-Override";

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        StringValues overrideStringValues = context.Request.Headers[XHttpMethodOverride];

        if (string.IsNullOrEmpty(overrideStringValues))
        {
            overrideStringValues = context.Request.Headers[MethodOverride];
        }

        string? overrideValue = null;

        if (!string.IsNullOrEmpty(overrideStringValues))
        {
            overrideValue = overrideStringValues.ToString();
        }

        bool getOverridePost = HttpMethods.IsPost(context.Request.Method) && HttpMethods.IsGet(overrideValue);
        bool deleteOverridePut = HttpMethods.IsPut(context.Request.Method) && HttpMethods.IsDelete(overrideValue);

        if (getOverridePost || deleteOverridePut)
        {
            context.Request.Method = overrideValue;
        }

        return next(context);
    }
}