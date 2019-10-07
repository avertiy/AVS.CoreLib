using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.ClientApi.WebClients
{
    public abstract class BaseWebClient
    {
        /// <summary>
        /// Proxy = ProxyHelper.GetWebProxy();
        /// </summary>
        public IWebProxy Proxy { get; set; }

        public string BaseAddress { get; set; }
        public string ApiVersion { get; set; }
        public string RelativeUrl { get; set; }

        public string LastResponse { get; set; }
        
        public void Validate()
        {
            if(string.IsNullOrEmpty(BaseAddress))
                throw new Exception($"BaseUrl is empty. Ensure {this.GetType().Name} has been properly setup.");
        }

        protected virtual string GetUrl(string command)
        {
            return $"{BaseAddress}{RelativeUrl}{command}";
        }

        protected string GetCommandUrl(string apiVersion, string command)
        {
            return $"{BaseAddress}{apiVersion ?? ApiVersion}{RelativeUrl}{command}";
        }

        protected virtual HttpWebRequest CreateHttpWebRequest(string method, string url)
        {
            var request = WebRequest.CreateHttp(url);
            request.Method = method;
            request.UserAgent = "AVS ApiWebClient v." + Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            request.Timeout = Timeout.Infinite;
            request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip,deflate";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            
            if (Proxy!=null)
            {
                request.Proxy = Proxy;
                Debug.WriteLine("ApiWebClient.Proxy {0}", Proxy);
            }
            
            Debug.WriteLine("WebRequest to: " + url);
            return request;
        }

        public string GetResponse(HttpWebRequest createRequest)
        {
            int attempt = 0;
            start:
            var jsonString = GetResponseInternal(createRequest);
            if (jsonString != null && jsonString.Contains("The remote server returned an error: (422).") && attempt++ < 2)
            {
                //sometimes exchange returns 422 error, but on the second attempt it is ok
                goto start;
            }
            return jsonString;
        }
        
        public async Task<string> GetResponseAsync(HttpWebRequest request)
        {
            int attempt = 0;
            start:
            var jsonText = await GetResponseAsyncInternal(request);
            if (jsonText != null && jsonText.Contains("The remote server returned an error: (422).") && attempt++ < 2)
            {
                //sometimes exchange returns 422 error, but on the second attempt it is ok
                goto start;
            }
            return jsonText;
            //return new JsonResponseResult() { JsonText = jsonText };
        }

        [DebuggerStepThrough]
        protected async Task<string> GetResponseAsyncInternal(HttpWebRequest request)
        {
            try
            {
                WebResponse response = await request.GetResponseAsync().ConfigureAwait(false);
                using (response)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null)
                            throw new NullReferenceException("The HttpWebRequest's response stream cannot be empty.");
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            LastResponse = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                            return LastResponse;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"{{ \"error\": \"Request to {request.RequestUri} failed. {ex.Message}\" }}";
            }
        }
        
        [DebuggerStepThrough]
        protected string GetResponseInternal(HttpWebRequest request)
        {
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null)
                            throw new NullReferenceException("The HttpWebRequest's response stream cannot be empty.");
                        using (StreamReader streamReader = new StreamReader(responseStream))
                            LastResponse = streamReader.ReadToEnd();
                    }
                }

                return LastResponse;
            }
            catch (Exception ex)
            {
                return $"{{ \"error\": \"Request to {request.RequestUri} failed. {ex.Message}\" }}";
            }
        }
    }
}
