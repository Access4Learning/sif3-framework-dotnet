﻿/*
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
using Sif.Framework.Extensions;
using Sif.Framework.Models.Parameters;

namespace Sif.Framework.AspNetCore.Extensions;

/// <summary>
/// This static class contains extension methods for the IHeaderDictionary class.
/// </summary>
public static class HeaderDictionaryExtension
{
    private const string Space = " ";

    private static readonly string NavigationPageHeader = RequestParameterType.navigationPage.ToDescription();

    private static readonly string NavigationPageSizeHeader =
        RequestParameterType.navigationPageSize.ToDescription();

    /// <summary>
    /// Retrieve the applicationKey property from the header.
    /// </summary>
    /// <param name="headers">Request headers.</param>
    /// <returns>applicationKey value if set; null otherwise.</returns>
    /// <exception cref="ArgumentNullException">Parameter headers is null.</exception>
    /// <exception cref="InvalidOperationException">Duplicate headers were found.</exception>
    public static string? GetApplicationKey(this IHeaderDictionary headers)
    {
        return headers.GetHeaderValue(RequestParameterType.applicationKey.ToDescription());
    }

    /// <summary>
    /// Retrieve the boolean value associated with the header of the passed name.
    /// </summary>
    /// <param name="headers">Request headers to check.</param>
    /// <param name="headerName">Name of the header.</param>
    /// <returns>Boolean value associated with the header if found; null otherwise.</returns>
    /// <exception cref="ArgumentNullException">Parameter headers is null.</exception>
    /// <exception cref="FormatException">Header value was not a boolean.</exception>
    /// <exception cref="InvalidOperationException">Duplicate headers were found.</exception>
    public static bool? GetBoolHeaderValue(this IHeaderDictionary headers, string headerName)
    {
        string? value = headers.GetHeaderValue(headerName);
        bool? boolValue = null;

        if (value != null)
        {
            boolValue = bool.Parse(value);
        }

        return boolValue;
    }

    /// <summary>
    /// Retrieve the string value associated with the header of the passed name.
    /// </summary>
    /// <param name="headers">Request headers to check.</param>
    /// <param name="headerName">Name of the header.</param>
    /// <returns>String value associated with the header if found; null otherwise.</returns>
    /// <exception cref="ArgumentNullException">Parameter headers is null, or headerName is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Duplicate headers were found.</exception>
    public static string? GetHeaderValue(this IHeaderDictionary headers, string headerName)
    {
        if (headers == null) throw new ArgumentNullException(nameof(headers));

        if (string.IsNullOrWhiteSpace(headerName)) throw new ArgumentNullException(nameof(headerName));

        StringValues header = headers[headerName];

        if (StringValues.IsNullOrEmpty(header)) return null;

        if (header.Count > 1) throw new InvalidOperationException($"Duplicate {headerName} headers found.");

        return header[0];
    }

    /// <summary>
    /// Retrieve the value for the "must use advisory" header.
    /// </summary>
    /// <param name="headers">Request headers.</param>
    /// <returns>Must use advisory value if found; null otherwise.</returns>
    /// <exception cref="ArgumentNullException">Parameter is null.</exception>
    /// <exception cref="FormatException">Header value was not a boolean.</exception>
    /// <exception cref="InvalidOperationException">Duplicate headers were found.</exception>
    public static bool? GetMustUseAdvisory(this IHeaderDictionary headers)
    {
        return headers.GetBoolHeaderValue(RequestParameterType.mustUseAdvisory.ToDescription());
    }

    /// <summary>
    /// Retrieve the value for the navigation page header.
    /// </summary>
    /// <param name="headers">Request headers.</param>
    /// <returns>Navigation page value if found; null otherwise.</returns>
    /// <exception cref="ArgumentNullException">Parameter is null.</exception>
    /// <exception cref="FormatException">Header value was not numeric.</exception>
    /// <exception cref="InvalidOperationException">Duplicate headers were found.</exception>
    /// <exception cref="OverflowException">Header value is less than UInt32.MinValue (0) or greater than UInt32.MaxValue.</exception>
    public static uint? GetNavigationPage(this IHeaderDictionary headers)
    {
        return headers.GetUintHeaderValue(NavigationPageHeader);
    }

    /// <summary>
    /// Retrieve the value for the navigation page size header.
    /// </summary>
    /// <param name="headers">Request headers.</param>
    /// <returns>Navigation page size value if found; null otherwise.</returns>
    /// <exception cref="ArgumentNullException">Parameter is null.</exception>
    /// <exception cref="FormatException">Header value was not numeric.</exception>
    /// <exception cref="InvalidOperationException">Duplicate headers were found.</exception>
    /// <exception cref="OverflowException">Header value is less than UInt32.MinValue (0) or greater than UInt32.MaxValue.</exception>
    public static uint? GetNavigationPageSize(this IHeaderDictionary headers)
    {
        return headers.GetUintHeaderValue(NavigationPageSizeHeader);
    }

    /// <summary>
    /// Retrieve the sourceName property from the header.
    /// </summary>
    /// <param name="headers">Request headers.</param>
    /// <returns>sourceName value if set; null otherwise.</returns>
    /// <exception cref="ArgumentNullException">Parameter headers is null.</exception>
    /// <exception cref="InvalidOperationException">Duplicate headers were found.</exception>
    public static string? GetSourceName(this IHeaderDictionary headers)
    {
        return headers.GetHeaderValue(RequestParameterType.sourceName.ToDescription());
    }

    /// <summary>
    /// Retrieve the timestamp property from the header.
    /// </summary>
    /// <param name="headers">Request headers.</param>
    /// <returns>timestamp value if set; null otherwise.</returns>
    /// <exception cref="ArgumentNullException">Parameter headers is null.</exception>
    /// <exception cref="InvalidOperationException">Duplicate headers were found.</exception>
    public static string? GetTimestamp(this IHeaderDictionary headers)
    {
        return headers.GetHeaderValue(RequestParameterType.timestamp.ToDescription());
    }

    /// <summary>
    /// Retrieve the unsigned integer value associated with the header of the passed name.
    /// </summary>
    /// <param name="headers">Request headers to check.</param>
    /// <param name="headerName">Name of the header.</param>
    /// <returns>Unsigned integer value associated with the header if found; null otherwise.</returns>
    /// <exception cref="ArgumentNullException">Parameter headers is null.</exception>
    /// <exception cref="FormatException">Header value was not numeric.</exception>
    /// <exception cref="InvalidOperationException">Duplicate headers were found.</exception>
    /// <exception cref="OverflowException">Header value is less than UInt32.MinValue (0) or greater than UInt32.MaxValue.</exception>
    public static uint? GetUintHeaderValue(this IHeaderDictionary headers, string headerName)
    {
        string? value = headers.GetHeaderValue(headerName);
        uint? uintValue = null;

        if (value != null)
        {
            uintValue = uint.Parse(value);
        }

        return uintValue;
    }

    /// <summary>
    /// Determine whether a method override has been specified.
    /// </summary>
    /// <param name="headers">Request headers.</param>
    /// <returns>True if method override header found; false otherwise.</returns>
    /// <exception cref="ArgumentNullException">Parameter is null.</exception>
    public static bool HasMethodOverrideHeader(this IHeaderDictionary headers)
    {
        if (headers == null) throw new ArgumentNullException(nameof(headers));

        bool hasMethodOverride = headers.ContainsKey(RequestParameterType.methodOverride.ToDescription());
        bool hasMethodOverrideSif = headers.ContainsKey(RequestParameterType.methodOverrideSif.ToDescription());

        return hasMethodOverride || hasMethodOverrideSif;
    }

    /// <summary>
    /// Determine whether paging parameters have been specified.
    /// </summary>
    /// <param name="headers">Request headers.</param>
    /// <returns>True if paging parameters have been found; false otherwise.</returns>
    /// <exception cref="ArgumentNullException">Parameter is null.</exception>
    public static bool HasPagingHeaders(this IHeaderDictionary headers)
    {
        if (headers == null) throw new ArgumentNullException(nameof(headers));

        return headers.ContainsKey(NavigationPageHeader) || headers.ContainsKey(NavigationPageSizeHeader);
    }

    /// <summary>
    /// Check whether the parameters used for paging are valid.
    /// </summary>
    /// <param name="headers">Request headers.</param>
    /// <param name="errorMessage">Error description if validation failed.</param>
    /// <returns>True if paging parameters are valid; false otherwise.</returns>
    /// <exception cref="ArgumentNullException">Parameter headers is null.</exception>
    public static bool ValidatePagingParameters(this IHeaderDictionary headers, out string? errorMessage)
    {
        errorMessage = null;
        uint? navigationPage = null;
        uint? navigationPageSize = null;

        try
        {
            navigationPage = headers.GetNavigationPage();
        }
        catch (FormatException)
        {
            errorMessage = $"{NavigationPageHeader} value is not numeric.";
        }
        catch (InvalidOperationException)
        {
            errorMessage = $"Duplicate {NavigationPageHeader} headers were found.";
        }
        catch (OverflowException)
        {
            errorMessage =
                $"{NavigationPageHeader} value is less than Int32.MinValue or greater than Int32.MaxValue.";
        }

        try
        {
            navigationPageSize = headers.GetNavigationPageSize();
        }
        catch (FormatException)
        {
            errorMessage += $"{errorMessage ?? Space}{NavigationPageSizeHeader} value is not numeric.";
        }
        catch (InvalidOperationException)
        {
            errorMessage += $"{errorMessage ?? Space}Duplicate {NavigationPageSizeHeader} headers were found.";
        }
        catch (OverflowException)
        {
            errorMessage +=
                $"{errorMessage ?? Space}{NavigationPageSizeHeader} value is less than Int32.MinValue or greater than Int32.MaxValue.";
        }

        if (navigationPage.HasValue && !navigationPageSize.HasValue)
        {
            errorMessage =
                $"{errorMessage ?? Space}{NavigationPageHeader} header was found, but not {NavigationPageSizeHeader}.";
        }
        else if (!navigationPage.HasValue && navigationPageSize.HasValue)
        {
            errorMessage =
                $"{errorMessage ?? Space}{NavigationPageSizeHeader} header was found, but not {NavigationPageHeader}.";
        }

        return string.IsNullOrWhiteSpace(errorMessage);
    }
}