using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace dotnet.Services
{
    public class RemoteRequestServices : IRemoteRequestService
    {
        public string Get(string url, NameValueCollection data)
        {
            return Get(url,data,null);
        }

        public string Get(string url, NameValueCollection data, CookieContainer cookieContainer)
        {
            return Get(url,data,cookieContainer,60000);
        }

        public string Get(string url, NameValueCollection data, CookieContainer cookieContainer, int timeout)
        {
            var strRet = new StringBuilder();

            try{
                var qPos = url.IndexOf('?');
                if (qPos>-1)
                {
                    url = url.Substring(0,qPos);
                }
                var serilizedData = SerializeData(data);
                var actualUrl = string.IsNullOrEmpty(serilizedData)?url:(url+"?"+serilizedData);

                var encode = Encoding.UTF8;

                var request = (HttpWebRequest)WebRequest.Create(actualUrl);

                if (cookieContainer!=null)
                {
                    request.CookieContainer = cookieContainer;
                }
                request.ContinueTimeout = timeout;

                var responese = (HttpWebResponse)request.GetResponseAsync().Result;
                var responeseStream = responese.GetResponseStream();

                var readStream = new StreamReader(responeseStream,encode);
                var read = new char[256];
                var count = readStream.Read(read,0,256);
                while (count>0)
                {
                    strRet.Append(new string(read,0,count));
                    count = readStream.Read(read,0,256);
                }

                responeseStream.Dispose();
            }
            catch(Exception e)
            {
                throw e;
            }

            return strRet.ToString();
        }

        public string Post(string url, NameValueCollection data)
        {
           return Post(url, data, null);
        }

        public string Post(string url, NameValueCollection data, CookieContainer cookieContainer)
        {
            return Post(url, data, cookieContainer, Encoding.UTF8);
        }

        public string Post(string url, NameValueCollection data, CookieContainer cookieContainer, Encoding encode)
        {
            var strRet = new StringBuilder();

            try
            {
                var dataStream = encode.GetBytes(SerializeData(data));

                var myReq = (HttpWebRequest) WebRequest.Create(url);
                myReq.Method = "POST";
                myReq.ContentType = "application/x-www-form-urlencoded";

                if (cookieContainer!=null)
                {
                    myReq.CookieContainer = cookieContainer;
                }

                var outStream = myReq.GetRequestStreamAsync().Result;
                outStream.Write(dataStream,0,dataStream.Length);
                outStream.Dispose();

                var myResp = (HttpWebResponse) myReq.GetResponseAsync().Result;

                if (cookieContainer!=null)
                {
                    //myResp.Cookies = cookieContainer.GetCookies(myReq.RequestUri);
                }

                var receiveStream = myResp.GetResponseStream();
                var readStream = new StreamReader(receiveStream,encode);
                var read = new char[256];
                var count = readStream.Read(read, 0, 256);
                while (count>0)
                {
                    strRet.Append(new string(read, 0, count));
                    count = readStream.Read(read, 0, 256);
                }
                readStream.Dispose();
                myResp.Dispose();
            }
            catch (Exception e)
            {
                throw e;
            }

            return strRet.ToString();
        }

        private static string SerializeData(NameValueCollection data){
            var sb = new StringBuilder();

            if (data!=null)
            {
                foreach (var item in data.AllKeys)
                {
                    sb.AppendFormat("&{0}={1}",WebUtility.UrlEncode(data[item]));
                }
            }

            var ret = sb.ToString();
            if(ret.StartsWith("&")){
                ret  = ret.Substring(1);
            }

            return ret;
        }
    }
}