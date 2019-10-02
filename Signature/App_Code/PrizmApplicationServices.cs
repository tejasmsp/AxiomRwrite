using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using Viewer.Class;

namespace Viewer
{
    public class PrizmApplicationServices : System.Web.UI.Page
    {
        /// <summary>
        /// Forwards any given request to PrizmApplicationServices
        /// </summary>
        /// <param name="context">The HTTP Context that will be used to access both request and response</param>
        /// <param name="path">The path that will be used to invoke PrizmApplicationServices</param>
        public static void ForwardRequest(HttpContext context, string path)
        {
            // Create a request with same data in navigator request
            var request = GetProxyRequest(context, context.Request.HttpMethod, path, context.Request.Url.Query);
            string GUID = "";
            if (path.Contains("Document") && path.Contains("MarkupBurner") && context.Request.Url.Query.Contains("GUID") == true)
            {
                GUID = context.Request.Url.Query.Split('&')[1].Split('=')[1];
            }
            // Send the request to the remote server and return the response
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }

            UpdateContextResponse(context, response, path, GUID);
            response.Close();
            context.Response.End();
        }


        private static HttpWebRequest GetProxyRequest(HttpContext context, string method, string path, string query)
        {
            var cookieContainer = new CookieContainer();

            // Create a request to the server
            var request = CreatePasRequest(method, path, query);
            request.KeepAlive = true;
            request.CookieContainer = cookieContainer;
            // Set special headers
            if (context.Request.AcceptTypes != null && context.Request.AcceptTypes.Any())
            {
                request.Accept = string.Join(",", context.Request.AcceptTypes);
            }
            request.ContentType = context.Request.ContentType;
            request.UserAgent = context.Request.UserAgent;

            // Copy headers
            foreach (var headerKey in context.Request.Headers.AllKeys)
            {
                if (WebHeaderCollection.IsRestricted(headerKey))
                {
                    continue;
                }
                request.Headers[headerKey] = context.Request.Headers[headerKey];
            }

            // Send Cookie extracted from the original request
            for (var i = 0; i < context.Request.Cookies.Count; i++)
            {
                var navigatorCookie = context.Request.Cookies[i];
                var c = new Cookie(navigatorCookie.Name, navigatorCookie.Value)
                {
                    Domain = request.RequestUri.Host,
                    Expires = navigatorCookie.Expires,
                    HttpOnly = navigatorCookie.HttpOnly,
                    Path = navigatorCookie.Path,
                    Secure = navigatorCookie.Secure
                };
                cookieContainer.Add(c);
            }

            // Write the body extracted from the incoming request
            if (request.Method != "GET"
                && request.Method != "HEAD")
            {
                context.Request.InputStream.Position = 0;
                var clientStream = context.Request.InputStream;
                var clientPostData = new byte[context.Request.InputStream.Length];
                clientStream.Read(clientPostData, 0,
                                 (int)context.Request.InputStream.Length);

                request.ContentType = context.Request.ContentType;
                request.ContentLength = clientPostData.Length;
                var stream = request.GetRequestStream();
                stream.Write(clientPostData, 0, clientPostData.Length);
                stream.Close();
            }
            return request;
        }

        private static HttpWebRequest GetProxyRequestWithQuery(HttpContext context, string method, string path = "", NameValueCollection query = null)
        {
            var queryString = "";
            if (query != null)
            {
                queryString = ToQueryString(query);
            }
            return GetProxyRequest(context, method, path, queryString);
        }

        private static byte[] GetResponseStreamBytes(WebResponse response)
        {
            const int bufferSize = 256;
            var buffer = new byte[bufferSize];
            var memoryStream = new MemoryStream();

            var responseStream = response.GetResponseStream();
            var remoteResponseCount = responseStream.Read(buffer, 0, bufferSize);

            while (remoteResponseCount > 0)
            {
                memoryStream.Write(buffer, 0, remoteResponseCount);
                remoteResponseCount = responseStream.Read(buffer, 0, bufferSize);
            }

            var responseData = memoryStream.ToArray();
            memoryStream.Close();
            responseStream.Close();

            memoryStream.Dispose();
            responseStream.Dispose();

            return responseData;
        }

        private static void UpdateContextResponse(HttpContext context, HttpWebResponse response, string path, string GUID)
        {
            try
            {
                // Copy headers
                foreach (var headerKey in response.Headers.AllKeys)
                {
                    if (WebHeaderCollection.IsRestricted(headerKey))
                    {
                        continue;
                    }
                    context.Response.AddHeader(headerKey, response.Headers[headerKey]);
                }
                context.Response.ContentType = response.ContentType;
                context.Response.Cookies.Clear();

                foreach (Cookie receivedCookie in response.Cookies)
                {
                    var c = new HttpCookie(receivedCookie.Name,
                        receivedCookie.Value)
                    {
                        Domain = context.Request.Url.Host,
                        Expires = receivedCookie.Expires,
                        HttpOnly = receivedCookie.HttpOnly,
                        Path = receivedCookie.Path,
                        Secure = receivedCookie.Secure
                    };
                    context.Response.Cookies.Add(c);
                }
                var responseData = GetResponseStreamBytes(response);
                if ((path.Contains("Document") && path.Contains("MarkupBurner") && context.Request.Url.Query.Contains("GUID") == true))
                {
                    string EncryptID = System.Web.HttpUtility.UrlDecode(context.Request.UrlReferrer.AbsoluteUri.Split('?')[1].Substring(7)).Replace(' ', '+');
                    int ID = Convert.ToInt32(EncryptDecrypt.Decrypt(EncryptID));
                    string OrderNo = string.Empty;
                    string PartNo = string.Empty;

                    var result = DbAccess.GetFileOrderData(ID);
                    if (result != null && result.Rows.Count > 0)
                    {
                        OrderNo = Convert.ToString(result.Rows[0]["OrderNo"]);
                        PartNo = Convert.ToString(result.Rows[0]["PartNo"]);
                    }

                    string FilePath = ConfigurationManager.AppSettings["UploadRoot"].ToString();
                    string FilePathDoc = string.Format("{0}{1}\\{2}", FilePath, OrderNo, PartNo);
                    FilePathDoc = FilePathDoc + "\\" + GUID;
                    if (!Directory.Exists(Path.GetDirectoryName(FilePathDoc)))
                        Directory.CreateDirectory(Path.GetDirectoryName(FilePathDoc));

                    FileStream file = System.IO.File.Create(FilePathDoc);
                    file.Write(responseData, 0, responseData.Length);
                    file.Close();
                }
                // Send the response to client          
                // if (!(path.Contains("Document") && path.Contains("MarkupBurner")))
                // {
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.ContentType = response.ContentType;
                context.Response.OutputStream.Write(responseData, 0, responseData.Length);
                context.Response.StatusCode = (int)response.StatusCode;
                // }
            }
            catch (Exception ex)
            { }
        }

        private static HttpWebRequest CreatePasRequest(string method, string path = "", string queryString = "")
        {
            queryString = queryString ?? "";
            if (queryString.StartsWith("?"))
            {
                queryString = queryString.Remove(0, 1);
            }
            var uriBuilder = new UriBuilder(PccConfig.PrizmApplicationServicesScheme, PccConfig.PrizmApplicationServicesHost,
                PccConfig.PrizmApplicationServicesPort, path)
            { Query = queryString };

            var url = uriBuilder.ToString();
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method.ToUpper().Trim();
            return request;
        }

        private static string ToQueryString(NameValueCollection nvc)
        {
            var list = new List<string>();
            foreach (var key in nvc.AllKeys)
            {
                foreach (var value in nvc.GetValues(key))
                {
                    list.Add(string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)));
                }
            }
            return string.Join("&", list.ToArray());
        }

        private static Dictionary<string, object> JsonToDictionary(string json)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<Dictionary<string, object>>(json);
        }
    }
}