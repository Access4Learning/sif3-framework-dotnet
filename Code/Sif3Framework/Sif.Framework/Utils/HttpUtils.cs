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

using Sif.Framework.Extensions;
using Sif.Framework.Models.Authentication;
using Sif.Framework.Models.Infrastructure;
using Sif.Framework.Models.Parameters;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;

namespace Sif.Framework.Utils
{
    /// <summary>
    /// This is a utility class for HTTP operations.
    /// </summary>
    public static class HttpUtils
    {
        internal enum RequestMethod
        { DELETE, GET, HEAD, POST, PUT }

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
            var request = (HttpWebRequest)WebRequest.Create(url);
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
                request.Headers.Add(RequestParameterType.deleteMessageId.ToDescription(), deleteMessageId.Trim());
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
                request.Headers.Add(
                    RequestParameterType.mustUseAdvisory.ToDescription(),
                    mustUseAdvisory.Value.ToString());
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
                            using (var reader = new StreamReader(response.GetResponseStream()))
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
                                using (var reader = new StreamReader(response.GetResponseStream()))
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
                if (e is WebException webException && webException.Response is HttpWebResponse httpWebResponse)
                {
                    switch (httpWebResponse.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            throw new AuthenticationException(
                                "Request is not authorised (authentication failed).", webException);
                        case HttpStatusCode.Forbidden:
                            throw new UnauthorizedAccessException(
                                "Request is forbidden (access denied).", webException);
                    }
                }

                throw;
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
            return MakeRequest(RequestMethod.DELETE,
                url,
                authorisationToken,
                compressPayload,
                out WebHeaderCollection _,
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
            return MakeRequest(RequestMethod.DELETE,
                url,
                authorisationToken,
                compressPayload,
                out WebHeaderCollection _,
                body,
                serviceType,
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
            return MakeRequest(RequestMethod.GET,
                url,
                authorisationToken,
                compressPayload,
                out WebHeaderCollection _,
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
            WebHeaderCollection responseHeaders;

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    responseHeaders = response.Headers;
                }
            }
            catch (Exception e)
            {
                if (e is WebException webException && webException.Response is HttpWebResponse httpWebResponse)
                {
                    switch (httpWebResponse.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            throw new AuthenticationException(
                                "Request is not authorised (authentication failed).", webException);
                        case HttpStatusCode.Forbidden:
                            throw new UnauthorizedAccessException(
                                "Request is forbidden (access denied).", webException);
                    }
                }

                throw;
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
            return MakeRequest(RequestMethod.POST,
                url,
                authorisationToken,
                compressPayload,
                out WebHeaderCollection _,
                body,
                serviceType,
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
            return MakeRequest(RequestMethod.PUT,
                url,
                authorisationToken,
                compressPayload,
                out WebHeaderCollection _,
                body,
                serviceType,
                methodOverride: methodOverride,
                contentTypeOverride: contentTypeOverride,
                acceptOverride: acceptOverride);
        }

        /// <summary>
        /// Gets the accept type from the request headers.
        /// </summary>
        /// <param name="request">HTTP Request</param>
        public static string GetAccept(HttpRequestMessage request)
        {
            string[] values = (from a in request.Headers.Accept select a.MediaType).ToArray();

            if (values.Length == 0)
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
            var matrixParameters = "";

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