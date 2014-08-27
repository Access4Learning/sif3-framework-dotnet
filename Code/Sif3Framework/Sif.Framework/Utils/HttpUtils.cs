/*
 * Copyright 2014 Systemic Pty Ltd
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

using System;
using System.IO;
using System.Net;
using System.Text;

namespace Sif.Framework.Utils
{

    /// <summary>
    /// This is a utility class for HTTP operations.
    /// </summary>
    static class HttpUtils
    {
        enum RequestMethod { DELETE, GET, POST, PUT }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMethod"></param>
        /// <param name="url"></param>
        /// <param name="authorisationToken"></param>
        /// <returns></returns>
        private static HttpWebRequest CreateHttpWebRequest(RequestMethod requestMethod, string url, string authorisationToken)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/xml";
            request.Method = requestMethod.ToString();
            request.KeepAlive = false;
            request.Accept = "application/xml";
            request.Headers.Add("Authorization", authorisationToken);
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMethod"></param>
        /// <param name="url"></param>
        /// <param name="authorisationToken"></param>
        /// <returns></returns>
        private static string RequestWithoutPayload(RequestMethod requestMethod, string url, string authorisationToken)
        {
            HttpWebRequest request = CreateHttpWebRequest(requestMethod, url, authorisationToken);

            using (WebResponse response = request.GetResponse())
            {
                string responseString = null;

                if (response == null)
                {
                    Console.WriteLine("Response is null");
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
        private static string RequestWithPayload(RequestMethod requestMethod, string url, string authorisationToken, string body)
        {
            HttpWebRequest request = CreateHttpWebRequest(requestMethod, url, authorisationToken);

            using (Stream requestStream = request.GetRequestStream())
            {
                byte[] payload = UTF8Encoding.UTF8.GetBytes(body);
                requestStream.Write(payload, 0, payload.Length);

                using (WebResponse response = request.GetResponse())
                {
                    string responseString = null;

                    if (response == null)
                    {
                        Console.WriteLine("Response is null");
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
        /// <returns></returns>
        public static string GetRequest(string url, string authorisationToken)
        {
            return RequestWithoutPayload(RequestMethod.GET, url, authorisationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="authorisationToken"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static string PostRequest(string url, string authorisationToken, string body)
        {
            return RequestWithPayload(RequestMethod.POST, url, authorisationToken, body);
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

    }

}
