/*
 * Copyright 2020 Systemic Pty Ltd
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

using Sif.Framework.Extensions;
using Sif.Framework.Model.Authentication;
using Sif.Framework.Model.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;

namespace Sif.Framework.Utils
{

    /// <summary>
    /// This is a utility class for HTTP operations.
    /// </summary>
    public static class HttpUtils
    {

        internal enum RequestMethod { DELETE, GET, HEAD, POST, PUT }

        [Obsolete("To be replaced by EventParameterType, RequestParameterType and ResponseParameterType")]
        internal enum RequestHeader
        {
            applicationKey,
            changesSinceMarker,
            contextId,
            deleteMessageId,
            eventAction,
            messageId,
            messageType,
            [Description("X-HTTP-Method-Override")]
            methodOverride,
            [Description("methodOverride")]
            methodOverrideSif,
            minWaitTime,
            mustUseAdvisory,
            navigationPage,
            navigationPageSize,
            Replacement,
            serviceName,
            serviceType,
            sourceName,
            timestamp,
            zoneId
        }

        /// <summary>
        /// Create a HTTP web request.
        /// </summary>
        /// <param name="requestMethod">Request method, e.g. GET.</param>
        /// <param name="url">Request endpoint.</param>
        /// <param name="authorisationToken">The authorization token.</param>
        /// <param name="compressPayload">Compress payload flag.</param>
        /// <param name="serviceType">Service type.</param>
        /// <param name="navigationPage">Current paging index.</param>
        /// <param name="navigationPageSize">Page size.</param>
        /// <param name="methodOverride">Overrides the method of the request, e.g. to implement a GET with a payload over a POST request.</param>
        /// <param name="contentTypeOverride">Overrides the ContentType header.</param>
        /// <param name="acceptOverride">Overrides the Accept header.</param>
        /// <param name="mustUseAdvisory">Flag to indicate whether the object's identifier should be retained.</param>
        /// <param name="deleteMessageId">Unique identifier of the SIF Event message to delete.</param>
        /// <param name="requestHeaders">Other header fields that need to be included.</param>
        /// <returns>HTTP web request.</returns>
        private static HttpWebRequest CreateHttpWebRequest(RequestMethod requestMethod,
            string url,
            AuthorisationToken authorisationToken,
            bool compressPayload,
            ServiceType? serviceType = null,
            int? navigationPage = null,
            int? navigationPageSize = null,
            string methodOverride = null,
            string contentTypeOverride = null,
            string acceptOverride = null,
            bool? mustUseAdvisory = null,
            string deleteMessageId = null,
            NameValueCollection requestHeaders = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/xml";
            request.Method = requestMethod.ToString();
            request.KeepAlive = false;
            request.Accept = "application/xml";
            request.Headers.Add("Authorization", authorisationToken.Token);
            request.Headers.Add("timestamp", authorisationToken.Timestamp ?? DateTime.UtcNow.ToString("o"));

            if (compressPayload)
            {
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            }

            if (serviceType.HasValue)
            {
                request.Headers.Add("serviceType", serviceType.Value.ToDescription());
            }

            if (navigationPage.HasValue)
            {
                request.Headers.Add("navigationPage", navigationPage.Value.ToString());
            }

            if (navigationPageSize.HasValue)
            {
                request.Headers.Add("navigationPageSize", navigationPageSize.Value.ToString());
            }

            if (!string.IsNullOrWhiteSpace(methodOverride))
            {
                request.Headers.Add("X-HTTP-Method-Override", methodOverride.Trim());
            }

            if (!string.IsNullOrWhiteSpace(methodOverride))
            {
                request.Headers.Add("methodOverride", methodOverride.Trim());
            }

            if (!string.IsNullOrWhiteSpace(deleteMessageId))
            {
                request.Headers.Add(RequestHeader.deleteMessageId.ToDescription(), deleteMessageId.Trim());
            }

            if (!string.IsNullOrWhiteSpace(acceptOverride))
            {
                request.Accept = acceptOverride.Trim();
            }

            if (!string.IsNullOrWhiteSpace(contentTypeOverride))
            {
                request.ContentType = contentTypeOverride.Trim();
            }

            if (mustUseAdvisory.HasValue)
            {
                request.Headers.Add(RequestHeader.mustUseAdvisory.ToDescription(), mustUseAdvisory.Value.ToString());
            }

            if (requestHeaders != null)
            {

                foreach (string name in requestHeaders)
                {

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        string value = requestHeaders[name];

                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            request.Headers.Add(name.Trim(), value.Trim());
                        }

                    }

                }

            }

            return request;
        }

        /// <summary>
        /// Make a HTTP request (with or without a payload).
        /// </summary>
        /// <param name="requestMethod">Request method, e.g. GET.</param>
        /// <param name="url">Request endpoint.</param>
        /// <param name="authorisationToken">The authorization token.</param>
        /// <param name="compressPayload">Compress payload flag.</param>
        /// <param name="responseHeaders">Response headers returned.</param>
        /// <param name="body">The data payload to send.</param>
        /// <param name="serviceType">Service type.</param>
        /// <param name="navigationPage">Current paging index.</param>
        /// <param name="navigationPageSize">Page size.</param>
        /// <param name="methodOverride">Overrides the method of the request, e.g. to implement a GET with a payload over a POST request.</param>
        /// <param name="contentTypeOverride">Overrides the ContentType header.</param>
        /// <param name="acceptOverride">Overrides the Accept header.</param>
        /// <param name="mustUseAdvisory">Flag to indicate whether the object's identifier should be retained.</param>
        /// <param name="deleteMessageId">Unique identifier of the SIF Event message to delete.</param>
        /// <param name="requestHeaders">Other header fields that need to be included.</param>
        /// <returns>Response (with headers).</returns>
        /// <exception cref="AuthenticationException">Request is not authorised (authentication failed).</exception>
        /// <exception cref="UnauthorizedAccessException">Request is forbidden (access denied).</exception>
        /// <exception cref="Exception">Unexpected error occurred.</exception>
        private static string MakeRequest(RequestMethod requestMethod,
            string url,
            AuthorisationToken authorisationToken,
            bool compressPayload,
            out WebHeaderCollection responseHeaders,
            string body = null,
            ServiceType? serviceType = null,
            int? navigationPage = null,
            int? navigationPageSize = null,
            string methodOverride = null,
            string contentTypeOverride = null,
            string acceptOverride = null,
            bool? mustUseAdvisory = null,
            string deleteMessageId = null,
            NameValueCollection requestHeaders = null)
        {
            HttpWebRequest request = CreateHttpWebRequest(requestMethod,
                url,
                authorisationToken,
                compressPayload,
                serviceType,
                navigationPage,
                navigationPageSize,
                methodOverride,
                contentTypeOverride,
                acceptOverride,
                mustUseAdvisory,
                deleteMessageId,
                requestHeaders);

            try
            {

                if (body == null)
                {

                    using (WebResponse response = request.GetResponse())
                    {
                        responseHeaders = response.Headers;
                        string responseString = null;

                        if (response != null)
                        {

                            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                            {
                                responseString = reader.ReadToEnd().Trim();
                            }

                        }

                        return responseString;
                    }

                }
                else
                {

                    using (Stream requestStream = request.GetRequestStream())
                    {
                        byte[] payload = Encoding.UTF8.GetBytes(body);
                        requestStream.Write(payload, 0, payload.Length);

                        using (WebResponse response = request.GetResponse())
                        {
                            responseHeaders = response.Headers;
                            string responseString = null;

                            if (response != null)
                            {

                                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                                {
                                    responseString = reader.ReadToEnd().Trim();
                                }

                            }

                            return responseString;
                        }

                    }

                }

            }
            catch (Exception e)
            {

                if ((e is WebException) && ((WebException)e).Response is HttpWebResponse)
                {
                    HttpWebResponse httpWebResponse = ((WebException)e).Response as HttpWebResponse;

                    if (httpWebResponse.StatusCode.Equals(HttpStatusCode.Unauthorized))
                    {
                        throw new AuthenticationException("Request is not authorised (authentication failed).", e);
                    }
                    else if (httpWebResponse.StatusCode.Equals(HttpStatusCode.Forbidden))
                    {
                        throw new UnauthorizedAccessException("Request is forbidden (access denied).", e);
                    }

                }

                throw e;
            }

        }

        /// <summary>
        /// Make a DELETE request.
        /// </summary>
        /// <param name="url">Request endpoint.</param>
        /// <param name="authorisationToken">The authorization token.</param>
        /// <param name="compressPayload">Compress payload flag.</param>
        /// <param name="serviceType">Service type.</param>
        /// <param name="contentTypeOverride">Overrides the ContentType header.</param>
        /// <param name="acceptOverride">Overrides the Accept header.</param>
        /// <returns>Response.</returns>
        /// <exception cref="AuthenticationException">Request is not authorised (authentication failed).</exception>
        /// <exception cref="UnauthorizedAccessException">Request is forbidden (access denied).</exception>
        /// <exception cref="Exception">Unexpected error occurred.</exception>
        public static string DeleteRequest(string url,
            AuthorisationToken authorisationToken,
            bool compressPayload,
            ServiceType serviceType = ServiceType.OBJECT,
            string contentTypeOverride = null,
            string acceptOverride = null)
        {
            WebHeaderCollection responseHeaders;

            return MakeRequest(RequestMethod.DELETE,
                url,
                authorisationToken,
                compressPayload,
                out responseHeaders,
                serviceType: serviceType,
                contentTypeOverride: contentTypeOverride,
                acceptOverride: acceptOverride);
        }

        /// <summary>
        /// Make a DELETE request.
        /// </summary>
        /// <param name="url">Request endpoint.</param>
        /// <param name="authorisationToken">The authorization token.</param>
        /// <param name="body">The data payload to send.</param>
        /// <param name="compressPayload">Compress payload flag.</param>
        /// <param name="serviceType">Service type.</param>
        /// <param name="contentTypeOverride">Overrides the ContentType header.</param>
        /// <param name="acceptOverride">Overrides the Accept header.</param>
        /// <returns>Response.</returns>
        /// <exception cref="AuthenticationException">Request is not authorised (authentication failed).</exception>
        /// <exception cref="UnauthorizedAccessException">Request is forbidden (access denied).</exception>
        /// <exception cref="Exception">Unexpected error occurred.</exception>
        public static string DeleteRequest(string url,
            AuthorisationToken authorisationToken,
            string body,
            bool compressPayload,
            ServiceType serviceType = ServiceType.OBJECT,
            string contentTypeOverride = null,
            string acceptOverride = null)
        {
            WebHeaderCollection responseHeaders;

            return MakeRequest(RequestMethod.DELETE,
                url,
                authorisationToken,
                compressPayload,
                out responseHeaders,
                body: body,
                serviceType: serviceType,
                contentTypeOverride: contentTypeOverride,
                acceptOverride: acceptOverride);
        }

        /// <summary>
        /// Make a GET request.
        /// </summary>
        /// <param name="url">Request endpoint.</param>
        /// <param name="authorisationToken">The authorization token.</param>
        /// <param name="compressPayload">Compress payload flag.</param>
        /// <param name="responseHeaders">Response headers returned.</param>
        /// <param name="serviceType">Service type.</param>
        /// <param name="navigationPage">Current paging index.</param>
        /// <param name="navigationPageSize">Page size.</param>
        /// <param name="contentTypeOverride">Overrides the ContentType header.</param>
        /// <param name="acceptOverride">Overrides the Accept header.</param>
        /// <param name="deleteMessageId">Unique identifier of the SIF Event message to delete.</param>
        /// <returns>Response (with headers).</returns>
        /// <exception cref="AuthenticationException">Request is not authorised (authentication failed).</exception>
        /// <exception cref="UnauthorizedAccessException">Request is forbidden (access denied).</exception>
        /// <exception cref="Exception">Unexpected error occurred.</exception>
        public static string GetRequestAndHeaders(string url,
            AuthorisationToken authorisationToken,
            bool compressPayload,
            out WebHeaderCollection responseHeaders,
            ServiceType serviceType = ServiceType.OBJECT,
            int? navigationPage = null,
            int? navigationPageSize = null,
            string contentTypeOverride = null,
            string acceptOverride = null,
            string deleteMessageId = null)
        {
            return MakeRequest(RequestMethod.GET,
                url,
                authorisationToken,
                compressPayload,
                out responseHeaders,
                serviceType: serviceType,
                navigationPage: navigationPage,
                navigationPageSize: navigationPageSize,
                contentTypeOverride: contentTypeOverride,
                acceptOverride: acceptOverride,
                deleteMessageId: deleteMessageId);
        }

        /// <summary>
        /// Make a HEAD request.
        /// </summary>
        /// <param name="url">Request endpoint.</param>
        /// <param name="authorisationToken">The authorization token.</param>
        /// <param name="compressPayload">Compress payload flag.</param>
        /// <param name="serviceType">Service type.</param>
        /// <param name="navigationPage">Current paging index.</param>
        /// <param name="navigationPageSize">Page size.</param>
        /// <param name="contentTypeOverride">Overrides the ContentType header.</param>
        /// <param name="acceptOverride">Overrides the Accept header.</param>
        /// <returns>Response.</returns>
        /// <exception cref="AuthenticationException">Request is not authorised (authentication failed).</exception>
        /// <exception cref="UnauthorizedAccessException">Request is forbidden (access denied).</exception>
        /// <exception cref="Exception">Unexpected error occurred.</exception>
        public static string GetRequest(string url,
            AuthorisationToken authorisationToken,
            bool compressPayload,
            ServiceType serviceType = ServiceType.OBJECT,
            int? navigationPage = null,
            int? navigationPageSize = null,
            string contentTypeOverride = null,
            string acceptOverride = null)
        {
            WebHeaderCollection responseHeaders;

            return MakeRequest(RequestMethod.GET,
                url,
                authorisationToken,
                compressPayload,
                out responseHeaders,
                serviceType: serviceType,
                navigationPage: navigationPage,
                navigationPageSize: navigationPageSize,
                contentTypeOverride: contentTypeOverride,
                acceptOverride: acceptOverride);
        }

        /// <summary>
        /// Make a HEAD request.
        /// </summary>
        /// <param name="url">Request endpoint.</param>
        /// <param name="authorisationToken">The authorization token.</param>
        /// <param name="compressPayload">Compress payload flag.</param>
        /// <param name="serviceType">Service type.</param>
        /// <returns>Web response headers.</returns>
        /// <exception cref="AuthenticationException">Request is not authorised (authentication failed).</exception>
        /// <exception cref="UnauthorizedAccessException">Request is forbidden (access denied).</exception>
        /// <exception cref="Exception">Unexpected error occurred.</exception>
        public static WebHeaderCollection HeadRequest(
            string url,
            AuthorisationToken authorisationToken,
            bool compressPayload,
            ServiceType serviceType = ServiceType.OBJECT)
        {
            HttpWebRequest request =
                CreateHttpWebRequest(RequestMethod.HEAD, url, authorisationToken, compressPayload, serviceType);
            WebHeaderCollection responseHeaders = new WebHeaderCollection();

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    responseHeaders = response.Headers;
                }

            }
            catch (Exception e)
            {

                if ((e is WebException) && ((WebException)e).Response is HttpWebResponse)
                {
                    HttpWebResponse httpWebResponse = ((WebException)e).Response as HttpWebResponse;

                    if (httpWebResponse.StatusCode.Equals(HttpStatusCode.Unauthorized))
                    {
                        throw new AuthenticationException("Request is not authorised (authentication failed).", e);
                    }
                    else if (httpWebResponse.StatusCode.Equals(HttpStatusCode.Forbidden))
                    {
                        throw new UnauthorizedAccessException("Request is forbidden (access denied).", e);
                    }

                }

                throw e;
            }

            return responseHeaders;
        }

        /// <summary>
        /// Make a POST request.
        /// </summary>
        /// <param name="url">Request endpoint.</param>
        /// <param name="authorisationToken">The authorization token.</param>
        /// <param name="body">The data payload to send.</param>
        /// <param name="compressPayload">Compress payload flag.</param>
        /// <param name="serviceType">Service type.</param>
        /// <param name="methodOverride">The method that can be used to override the POST, e.g. to issue a GET with a payload.</param>
        /// <param name="contentTypeOverride">Overrides the ContentType header.</param>
        /// <param name="acceptOverride">Overrides the Accept header.</param>
        /// <param name="mustUseAdvisory">Flag to indicate whether the object's identifier should be retained.</param>
        /// <param name="requestHeaders">Other header fields that need to be included.</param>
        /// <returns>Response.</returns>
        /// <exception cref="AuthenticationException">Request is not authorised (authentication failed).</exception>
        /// <exception cref="UnauthorizedAccessException">Request is forbidden (access denied).</exception>
        /// <exception cref="Exception">Unexpected error occurred.</exception>
        public static string PostRequest(string url,
            AuthorisationToken authorisationToken,
            string body,
            bool compressPayload,
            ServiceType serviceType = ServiceType.OBJECT,
            string methodOverride = null,
            string contentTypeOverride = null,
            string acceptOverride = null,
            bool? mustUseAdvisory = null,
            NameValueCollection requestHeaders = null)
        {
            WebHeaderCollection responseHeaders;

            return MakeRequest(RequestMethod.POST,
                url,
                authorisationToken,
                compressPayload,
                out responseHeaders,
                body: body,
                serviceType: serviceType,
                methodOverride: methodOverride,
                contentTypeOverride: contentTypeOverride,
                acceptOverride: acceptOverride,
                mustUseAdvisory: mustUseAdvisory,
                requestHeaders: requestHeaders);
        }

        /// <summary>
        /// Make a PUT request.
        /// </summary>
        /// <param name="url">Request endpoint.</param>
        /// <param name="authorisationToken">The authorization token.</param>
        /// <param name="body">The data payload to send.</param>
        /// <param name="compressPayload">Compress payload flag.</param>
        /// <param name="serviceType">Service type.</param>
        /// <param name="methodOverride">The method that can be used to override the PUT, e.g. to issue a DELETE with a payload.</param>
        /// <param name="contentTypeOverride">Overrides the ContentType header.</param>
        /// <param name="acceptOverride">Overrides the Accept header.</param>
        /// <returns>Response.</returns>
        /// <exception cref="AuthenticationException">Request is not authorised (authentication failed).</exception>
        /// <exception cref="UnauthorizedAccessException">Request is forbidden (access denied).</exception>
        /// <exception cref="Exception">Unexpected error occurred.</exception>
        public static string PutRequest(string url,
            AuthorisationToken authorisationToken,
            string body,
            bool compressPayload,
            ServiceType serviceType = ServiceType.OBJECT,
            string methodOverride = null,
            string contentTypeOverride = null,
            string acceptOverride = null)
        {
            WebHeaderCollection responseHeaders;

            return MakeRequest(RequestMethod.PUT,
                url,
                authorisationToken,
                compressPayload,
                out responseHeaders,
                body: body,
                serviceType: serviceType,
                methodOverride: methodOverride,
                contentTypeOverride: contentTypeOverride,
                acceptOverride: acceptOverride);
        }

        /// <summary>
        /// This method will add the exception message to the reason phrase of the error response.
        /// </summary>
        public static HttpResponseMessage CreateErrorResponse(HttpRequestMessage request, HttpStatusCode httpStatusCode, Exception exception)
        {
            string exceptionMessage = (exception.Message == null ? "" : exception.Message.Trim());
            HttpResponseMessage response = request.CreateErrorResponse(httpStatusCode, exception);

            // The ReasonPhrase may not contain new line characters.
            response.ReasonPhrase = StringUtils.RemoveNewLines(exceptionMessage);

            return response;
        }

        /// <summary>
        /// This method will add the message specified to the reason phrase of the error response.
        /// </summary>
        public static HttpResponseMessage CreateErrorResponse(HttpRequestMessage request, HttpStatusCode httpStatusCode, string message)
        {
            HttpResponseMessage response = request.CreateErrorResponse(httpStatusCode, message);

            // The ReasonPhrase may not contain new line characters.
            response.ReasonPhrase = StringUtils.RemoveNewLines(message);

            return response;
        }

        /// <summary>
        /// Retrieve the string value associated with the header of the passed name.
        /// </summary>
        /// <param name="headers">Request headers to check.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <exception cref="ArgumentNullException">Parameter headers is null.</exception>
        /// <exception cref="InvalidOperationException">Duplicate headers were found.</exception>
        /// <returns>String value associated with the header if found; null otherwise.</returns>
        internal static string GetHeaderValue(HttpHeaders headers, string headerName)
        {

            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            string value = null;
            IEnumerable<string> headerValues;

            if (headers.TryGetValues(headerName, out headerValues))
            {
                value = headerValues.SingleOrDefault();
            }

            return value;
        }

        /// <summary>
        /// Retrieve the boolean value associated with the header of the passed name.
        /// </summary>
        /// <param name="headers">Request headers to check.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <exception cref="ArgumentNullException">Parameter headers is null.</exception>
        /// <exception cref="FormatException">Header value was not a boolean.</exception>
        /// <exception cref="InvalidOperationException">Duplicate headers were found.</exception>
        /// <returns>Boolean value associated with the header if found; null otherwise.</returns>
        internal static bool? GetBoolHeaderValue(HttpHeaders headers, string headerName)
        {
            string value = GetHeaderValue(headers, headerName);
            bool? boolValue = null;

            if (value != null)
            {
                boolValue = bool.Parse(value);
            }

            return boolValue;
        }

        /// <summary>
        /// Retrieve the unsigned integere value associated with the header of the passed name.
        /// </summary>
        /// <param name="headers">Request headers to check.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <exception cref="ArgumentNullException">Parameter headers is null.</exception>
        /// <exception cref="FormatException">Header value was not numeric.</exception>
        /// <exception cref="InvalidOperationException">Duplicate headers were found.</exception>
        /// <exception cref="OverflowException">Header value is less than UInt32.MinValue (0) or greater than UInt32.MaxValue.</exception>
        /// <returns>Unsigned integer value associated with the header if found; null otherwise.</returns>
        internal static uint? GetUintHeaderValue(HttpHeaders headers, string headerName)
        {
            string value = GetHeaderValue(headers, headerName);
            uint? uintValue = null;

            if (value != null)
            {
                uintValue = uint.Parse(value);
            }

            return uintValue;
        }

        /// <summary>
        /// Retrieve the value for the "must use advisory" header.
        /// </summary>
        /// <param name="headers">Request headers.</param>
        /// <exception cref="ArgumentNullException">Parameter is null.</exception>
        /// <exception cref="FormatException">Header value was not a boolean.</exception>
        /// <exception cref="InvalidOperationException">Duplicate headers were found.</exception>
        /// <returns>Must use advisory value if found; null otherwise.</returns>
        public static bool? GetMustUseAdvisory(HttpHeaders headers)
        {
            return GetBoolHeaderValue(headers, RequestHeader.mustUseAdvisory.ToDescription());
        }

        /// <summary>
        /// Retrieve the value for the navigation page header.
        /// </summary>
        /// <param name="headers">Request headers.</param>
        /// <exception cref="ArgumentNullException">Parameter is null.</exception>
        /// <exception cref="FormatException">Header value was not numeric.</exception>
        /// <exception cref="InvalidOperationException">Duplicate headers were found.</exception>
        /// <exception cref="OverflowException">Header value is less than UInt32.MinValue (0) or greater than UInt32.MaxValue.</exception>
        /// <returns>Navigation page value if found; null otherwise.</returns>
        internal static uint? GetNavigationPage(HttpHeaders headers)
        {
            return GetUintHeaderValue(headers, RequestHeader.navigationPage.ToDescription());
        }

        /// <summary>
        /// Retrieve the value for the navigation page size header.
        /// </summary>
        /// <param name="headers">Request headers.</param>
        /// <exception cref="ArgumentNullException">Parameter is null.</exception>
        /// <exception cref="FormatException">Header value was not numeric.</exception>
        /// <exception cref="InvalidOperationException">Duplicate headers were found.</exception>
        /// <exception cref="OverflowException">Header value is less than UInt32.MinValue (0) or greater than UInt32.MaxValue.</exception>
        /// <returns>Navigation page size value if found; null otherwise.</returns>
        internal static uint? GetNavigationPageSize(HttpHeaders headers)
        {
            return GetUintHeaderValue(headers, RequestHeader.navigationPageSize.ToDescription());
        }

        /// <summary>
        /// Determine whether a method override has been specified.
        /// </summary>
        /// <param name="headers">Request headers.</param>
        /// <exception cref="ArgumentNullException">Parameter is null.</exception>
        /// <returns>True if method override header found; false otherwise.</returns>
        internal static bool HasMethodOverrideHeader(HttpHeaders headers)
        {

            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            bool hasMethodOverride = headers.Contains(RequestHeader.methodOverride.ToDescription());
            bool hasMethodOverrideSif = headers.Contains(RequestHeader.methodOverrideSif.ToDescription());

            return (hasMethodOverride || hasMethodOverrideSif);
        }

        /// <summary>
        /// Determine whether paging parameters have been specified.
        /// </summary>
        /// <param name="headers">Request headers.</param>
        /// <exception cref="ArgumentNullException">Parameter is null.</exception>
        /// <returns>True if paging parameters have been found; false otherwise.</returns>
        public static bool HasPagingHeaders(HttpHeaders headers)
        {

            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            return headers.Contains(RequestHeader.navigationPage.ToDescription()) || headers.Contains(RequestHeader.navigationPageSize.ToDescription());
        }

        /// <summary>
        /// Check whether the parameters used for paging are valid.
        /// </summary>
        /// <param name="headers">Request headers.</param>
        /// <param name="errorMessage">Error description if validation failed.</param>
        /// <exception cref="ArgumentNullException">Parameter headers is null.</exception>
        /// <returns>True if paging parameters are valid; false otherwise.</returns>
        internal static bool ValidatePagingParameters(HttpHeaders headers, out string errorMessage)
        {
            errorMessage = "";
            uint? navigationPage = null;
            uint? navigationPageSize = null;

            try
            {
                navigationPage = GetNavigationPage(headers);
            }
            catch (FormatException)
            {
                errorMessage = RequestHeader.navigationPage.ToDescription() + " value is not numeric.";
            }
            catch (InvalidOperationException)
            {
                errorMessage = "Duplicate " + RequestHeader.navigationPage.ToDescription() + " headers were found.";
            }
            catch (OverflowException)
            {
                errorMessage = RequestHeader.navigationPage.ToDescription() + " value is less than Int32.MinValue or greater than Int32.MaxValue.";
            }

            try
            {
                navigationPageSize = GetNavigationPageSize(headers);
            }
            catch (FormatException)
            {
                errorMessage += (errorMessage == "" ? "" : " ") + RequestHeader.navigationPageSize.ToDescription() + " value is not numeric.";
            }
            catch (InvalidOperationException)
            {
                errorMessage += (errorMessage == "" ? "" : " ") + "Duplicate " + RequestHeader.navigationPageSize.ToDescription() + " headers were found.";
            }
            catch (OverflowException)
            {
                errorMessage += (errorMessage == "" ? "" : " ") + RequestHeader.navigationPageSize.ToDescription() + " value is less than Int32.MinValue or greater than Int32.MaxValue.";
            }

            if (navigationPage.HasValue && !navigationPageSize.HasValue)
            {
                errorMessage = RequestHeader.navigationPage.ToDescription() + " header was found, but not " + RequestHeader.navigationPageSize.ToDescription() + ".";
            }
            else if (!navigationPage.HasValue && navigationPageSize.HasValue)
            {
                errorMessage = RequestHeader.navigationPageSize.ToDescription() + " header was found, but not " + RequestHeader.navigationPage.ToDescription() + ".";
            }

            return string.IsNullOrWhiteSpace(errorMessage);
        }

        /// <summary>
        /// Retrieve the applicationKey property from the header.
        /// </summary>
        /// <param name="headers">Request headers.</param>
        /// <returns>applicationKey value if set; null otherwise.</returns>
        internal static string GetApplicationKey(HttpHeaders headers)
        {
            return GetHeaderValue(headers, RequestHeader.applicationKey.ToDescription());
        }

        /// <summary>
        /// Retrieve the sourceName property from the header.
        /// </summary>
        /// <param name="headers">Request headers.</param>
        /// <returns>sourceName value if set; null otherwise.</returns>
        internal static string GetSourceName(HttpHeaders headers)
        {
            return GetHeaderValue(headers, RequestHeader.sourceName.ToDescription());
        }

        /// <summary>
        /// Retrieve the timestamp property from the header.
        /// </summary>
        /// <param name="headers">Request headers.</param>
        /// <returns>timestamp value if set; null otherwise.</returns>
        internal static string GetTimestamp(HttpHeaders headers)
        {
            return GetHeaderValue(headers, RequestHeader.timestamp.ToDescription());
        }

        /// <summary>
        /// Gets the content type from the request headers.
        /// </summary>
        /// <param name="Request">HTTP Request</param>
        public static string GetContentType(HttpRequestMessage Request)
        {
            return Request.Content.Headers.ContentType.MediaType;
        }

        /// <summary>
        /// Gets the accept type from the request headers.
        /// </summary>
        /// <param name="Request">HTTP Request</param>
        public static string GetAccept(HttpRequestMessage Request)
        {
            string[] values = (from a in Request.Headers.Accept select a.MediaType).ToArray();

            if (values == null || values.Length == 0)
            {
                return "plain/text";
            }

            return values[0];
        }

        /// <summary>
        /// Build up a string of Matrix Parameters based upon the passed parameters.
        /// </summary>
        /// <param name="zoneId">Zone associated with a request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <returns>String of Matrix Parameters.</returns>
        public static string MatrixParameters(string zoneId = null, string contextId = null)
        {
            string matrixParameters = "";

            if (!string.IsNullOrWhiteSpace(zoneId))
            {
                matrixParameters += ";zoneId=" + zoneId.Trim();
            }

            if (!string.IsNullOrWhiteSpace(contextId))
            {
                matrixParameters += ";contextId=" + contextId.Trim();
            }

            return matrixParameters;
        }

    }

}
