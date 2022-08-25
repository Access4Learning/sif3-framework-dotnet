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

using Microsoft.AspNetCore.Builder;
using Sif.Framework.AspNetCore.Middlewares;

namespace Sif.Framework.AspNetCore.Extensions;

/// <summary>
/// This static class contains extension methods for the IApplicationBuilder interface.
/// </summary>
public static class ApplicationBuilderExtension
{
    /// <summary>
    /// Add middleware for managing method override HTTP Request headers.
    /// </summary>
    /// <param name="builder">Application builder.</param>
    /// <returns>Application builder.</returns>
    public static IApplicationBuilder UseMethodOverrideMiddleware(this IApplicationBuilder builder) =>
        builder.UseMiddleware<MethodOverrideMiddleware>();
}