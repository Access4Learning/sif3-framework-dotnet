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

namespace Sif.Framework.Demo.Consumer
{
    class DemoConsumer
    {
        private static string environmentUrl = "http://localhost:62921/api/solutions/Sif3DemoSolution/environments";
        private static string password = "SecretDem0";
        private static string studentUrl = "StudentPersonals";
        private static string username = "Sif3DemoApp";

        private string GetRequest(string url, string sessionToken)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/xml";
            request.Method = "GET";
            request.KeepAlive = false;
            request.Accept = "application/xml";
            request.Headers.Add("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:" + password, sessionToken))));

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

        private string PostRequest(string url, string body)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/xml";
            request.Method = "POST";
            request.KeepAlive = false;
            request.Accept = "application/xml";
            request.Headers.Add("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(username + ":" + password)));

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

        static void Main(string[] args)
        {
            DemoConsumer demo = new DemoConsumer();
            StringBuilder body = new StringBuilder();
            body.Append("<environment xmlns='http://www.sifassociation.org/infrastructure/3.0' type='DIRECT'>");
            body.Append("  <solutionId>Sif3DemoSolution</solutionId>");
            body.Append("  <authenticationMethod>Basic</authenticationMethod>");
            body.Append("  <instanceId></instanceId>");
            body.Append("  <userToken></userToken>");
            body.Append("  <consumerName>Sif3DemoStudentPersonalConsumer</consumerName>");
            body.Append("  <applicationInfo>");
            body.Append("    <applicationKey>Sif3DemoApp</applicationKey>");
            body.Append("    <supportedInfrastructureVersion>3.0</supportedInfrastructureVersion>");
            body.Append("    <supportedDataModel>SIF-AU</supportedDataModel>");
            body.Append("    <supportedDataModelVersion>3.0</supportedDataModelVersion>");
            body.Append("    <transport>REST</transport>");
            body.Append("  </applicationInfo>");
            body.Append("</environment>");

            try
            {
                string xml = demo.PostRequest(environmentUrl, body.ToString());
                //XmlDocument environmentXml = new XmlDocument();
                //environmentXml.LoadXml(xml);
                //string sessionToken = environmentXml.SelectSingleNode("//sessionToken").InnerText;
                //string baseUrl = environmentXml.SelectSingleNode("//infrastructureService[@name='requestsConnector']").InnerText;
                //xml = demo.GetRequest(baseUrl + "/" + studentUrl, sessionToken);
                //XmlDocument studentXml = new XmlDocument();
                //studentXml.LoadXml(xml);
                Console.WriteLine("Request stream retrieved without error");
                Console.Out.WriteLine(xml);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occurred:" + e.ToString());
            }
            finally
            {
                Console.Out.WriteLine("Press any key");
                Console.ReadKey();
            }

        }

    }

}
