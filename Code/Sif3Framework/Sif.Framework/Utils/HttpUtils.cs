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

using log4net;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace Sif.Framework.Utils
{

    /// <summary>
    /// This is a utility class for HTTP operations.
    /// </summary>
    static class HttpUtils
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        enum RequestMethod { DELETE, GET, POST, PUT }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMethod"></param>
        /// <param name="url"></param>
        /// <param name="authorisationToken"></param>
        /// <param name="navigationPage"></param>
        /// <param name="navigationPageSize"></param>
        /// <returns></returns>
        private static HttpWebRequest CreateHttpWebRequest(RequestMethod requestMethod, string url, string authorisationToken, int? navigationPage = null, int? navigationPageSize = null, string methodOverride = null)
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
        /// <returns></returns>
        private static string RequestWithoutPayload(RequestMethod requestMethod, string url, string authorisationToken, int? navigationPage = null, int? navigationPageSize = null)
        {
            HttpWebRequest request = CreateHttpWebRequest(requestMethod, url, authorisationToken, navigationPage: navigationPage, navigationPageSize: navigationPageSize);

            using (WebResponse response = request.GetResponse())
            {
                string responseString = null;

                if (response == null)
                {
                    if (log.IsDebugEnabled) log.Debug("Response is null");
                }
                else
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
        /// <returns></returns>
        private static string RequestWithPayload(RequestMethod requestMethod, string url, string authorisationToken, string body, string methodOverride = null)
        {
            HttpWebRequest request = CreateHttpWebRequest(requestMethod, url, authorisationToken, methodOverride: methodOverride);

            using (Stream requestStream = request.GetRequestStream())
            {
                byte[] payload = UTF8Encoding.UTF8.GetBytes(body);
                requestStream.Write(payload, 0, payload.Length);

                using (WebResponse response = request.GetResponse())
                {
                    string responseString = null;

                    if (response == null)
                    {
                        if (log.IsDebugEnabled) log.Debug("Response is null");
                    }
                    else
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
        /// <returns></returns>
        public static string DeleteRequest(string url, string authorisationToken)
        {
            return RequestWithoutPayload(RequestMethod.DELETE, url, authorisationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="authorisationToken"></param>
        /// <param name="navigationPage"></param>
        /// <param name="navigationPageSize"></param>
        /// <returns></returns>
        public static string GetRequest(string url, string authorisationToken, int? navigationPage = null, int? navigationPageSize = null)
        {
            return RequestWithoutPayload(RequestMethod.GET, url, authorisationToken, navigationPage, navigationPageSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="authorisationToken"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static string PostRequest(string url, string authorisationToken, string body, string methodOverride = null)
        {
            return RequestWithPayload(RequestMethod.POST, url, authorisationToken, body, methodOverride);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="authorisationToken"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static string PutRequest(string url, string authorisationToken, string body)
        {
            return RequestWithPayload(RequestMethod.PUT, url, authorisationToken, body);
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

    }

}
