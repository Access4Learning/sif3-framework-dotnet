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

using Sif.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Sif.Framework.Utils
{

    /// <summary>
    /// This is a utility class for HTTP operations.
    /// </summary>
    public static class HttpUtils
    {

        internal enum RequestMethod { DELETE, GET, POST, PUT }

        enum RequestHeader
        {
            [Description("X-HTTP-Method-Override")]
            methodOverride,
            [Description("methodOverride")]
            methodOverrideSif,
            mustUseAdvisory,
            navigationPage,
            navigationPageSize,
            applicationKey,
            sourceName
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMethod"></param>
        /// <param name="url"></param>
        /// <param name="authorisationToken"></param>
        /// <param name="navigationPage"></param>
        /// <param name="navigationPageSize"></param>
        /// <param name="methodOverride">Overrides the method header.</param>
        /// <param name="contentTypeOverride">Overrides the ContentType header.</param>
        /// <param name="acceptOverride">Overrides the Accept header.</param>
        /// <returns></returns>
        private static HttpWebRequest CreateHttpWebRequest(RequestMethod requestMethod, string url, string authorisationToken, int? navigationPage = null, int? navigationPageSize = null, string methodOverride = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/xml";
            request.Method = requestMethod.ToString();
            request.KeepAlive = false;
            request.Accept = "application/xml";
            request.Headers.Add("Authorization", authorisationToken);

            if (navigationPage.HasValue)
            {
                request.Headers.Add("navigationPage", navigationPage.Value.ToString());
            }

            if (navigationPageSize.HasValue)
            {
                request.Headers.Add("navigationPageSize", navigationPageSize.Value.ToString());
            }

            if (!String.IsNullOrWhiteSpace(methodOverride))
            {
                request.Headers.Add("X-HTTP-Method-Override", methodOverride.Trim());
            }

            if (!String.IsNullOrWhiteSpace(methodOverride))
            {
                request.Headers.Add("methodOverride", methodOverride.Trim());
            }

            if (!String.IsNullOrWhiteSpace(acceptOverride))
            {
                request.Accept = acceptOverride.Trim();
            }

            if (!String.IsNullOrWhiteSpace(contentTypeOverride))
            {
                request.ContentType = contentTypeOverride.Trim();
            }

            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMethod"></param>
        /// <param name="url"></param>
        /// <param name="authorisationToken"></param>
        /// <param name="navigationPage"></param>
        /// <param name="navigationPageSize"></param>
        /// <param name="contentTypeOverride">Overrides the ContentType header.</param>
        /// <param name="acceptOverride">Overrides the Accept header.</param>
        /// <returns></returns>
        private static string RequestWithoutPayload(RequestMethod requestMethod, string url, string authorisationToken, int? navigationPage = null, int? navigationPageSize = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            HttpWebRequest request = CreateHttpWebRequest(requestMethod, url, authorisationToken, navigationPage: navigationPage, navigationPageSize: navigationPageSize, contentTypeOverride: contentTypeOverride, acceptOverride: acceptOverride);

            using (WebResponse response = request.GetResponse())
            {

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMethod"></param>
        /// <param name="url"></param>
        /// <param name="authorisationToken"></param>
        /// <param name="body"></param>
        /// <param name="methodOverride">Overrides the method of the request, e.g. to implement a GET with a payload over a POST request.</param>
        /// <param name="contentTypeOverride">Overrides the ContentType header.</param>
        /// <param name="acceptOverride">Overrides the Accept header.</param>
        /// <returns></returns>
        private static string RequestWithPayload(RequestMethod requestMethod, string url, string authorisationToken, string body, string methodOverride = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            HttpWebRequest request = CreateHttpWebRequest(requestMethod, url, authorisationToken, methodOverride: methodOverride, contentTypeOverride: contentTypeOverride, acceptOverride: acceptOverride);

            using (Stream requestStream = request.GetRequestStream())
            {
                if (body != null)
                {
                    byte[] payload = UTF8Encoding.UTF8.GetBytes(body);
                    requestStream.Write(payload, 0, payload.Length);
                }
                using (WebResponse response = request.GetResponse())
                {
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="authorisationToken"></param>
        /// <param name="contentTypeOverride">Overrides the ContentType header.</param>
        /// <param name="acceptOverride">Overrides the Accept header.</param>
        /// <returns></returns>
        public static string DeleteRequest(string url, string authorisationToken, string contentTypeOverride = null, string acceptOverride = null)
        {
            return RequestWithoutPayload(RequestMethod.DELETE, url, authorisationToken, contentTypeOverride: contentTypeOverride, acceptOverride: acceptOverride);
        }

        public static string DeleteRequest(string url, string authorisationToken, string body, string contentTypeOverride = null, string acceptOverride = null)
        {
            return RequestWithPayload(RequestMethod.DELETE, url, authorisationToken, body, contentTypeOverride, acceptOverride);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="authorisationToken"></param>
        /// <param name="navigationPage"></param>
        /// <param name="navigationPageSize"></param>
        /// <param name="contentTypeOverride">Overrides the ContentType header.</param>
        /// <param name="acceptOverride">Overrides the Accept header.</param>
        /// <returns></returns>
        public static string GetRequest(string url, string authorisationToken, int? navigationPage = null, int? navigationPageSize = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            return RequestWithoutPayload(RequestMethod.GET, url, authorisationToken, navigationPage, navigationPageSize, contentTypeOverride, acceptOverride);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="authorisationToken"></param>
        /// <param name="body"></param>
        /// <param name="methodOverride"></param>
        /// <param name="contentTypeOverride">Overrides the ContentType header.</param>
        /// <param name="acceptOverride">Overrides the Accept header.</param>
        /// <returns></returns>
        public static string PostRequest(string url, string authorisationToken, string body, string methodOverride = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            return RequestWithPayload(RequestMethod.POST, url, authorisationToken, body, methodOverride, contentTypeOverride, acceptOverride);
        }

        /// <summary>
        /// Makes and sends a put request.
        /// </summary>
        /// <param name="url">Where to send the request.</param>
        /// <param name="authorisationToken">The authorization token.</param>
        /// <param name="body">The data payload to send.</param>
        /// <param name="methodOverride">The method that can be used to override the PUT, e.g. to issue a GET with a payload.</param>
        /// <param name="contentTypeOverride">Overrides the ContentType header.</param>
        /// <param name="acceptOverride">Overrides the Accept header.</param>
        /// <returns></returns>
        public static string PutRequest(string url, string authorisationToken, string body, string methodOverride = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            return RequestWithPayload(RequestMethod.PUT, url, authorisationToken, body, methodOverride, contentTypeOverride, acceptOverride);
        }

        /// <summary>
        /// This method will additionally add the exception message to the reason phrase of the error response.
        /// <see cref="System.Net.Http.HttpRequestMessageExtensions.CreateErrorResponse(System.Net.HttpStatusCode, System.Exception)"/>
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
        /// This method will additionally add the message specified to the reason phrase of the error response.
        /// <see cref="System.Net.Http.HttpRequestMessageExtensions.CreateErrorResponse(System.Net.HttpStatusCode, System.String)"/>
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
            IEnumerable<String> headerValues;

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
        internal static bool? GetMustUseAdvisory(HttpHeaders headers)
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
        internal static bool HasPagingHeaders(HttpHeaders headers)
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
        /// <param name="zone">Zone associated with a request.</param>
        /// <param name="context">Zone context.</param>
        /// <returns>String of Matrix Parameters.</returns>
        public static string MatrixParameters(string zone = null, string context = null)
        {
            string matrixParameters = "";

            if (!string.IsNullOrWhiteSpace(zone))
            {
                matrixParameters += ";zone=" + zone.Trim();
            }

            if (!string.IsNullOrWhiteSpace(context))
            {
                matrixParameters += ";context=" + context.Trim();
            }

            return matrixParameters;
        }
    }
}
