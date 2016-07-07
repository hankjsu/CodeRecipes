
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace dotnet.Services
{
    public interface IRemoteRequestService
    {
        string Get(string url,NameValueCollection data);

        string Get(string url,NameValueCollection data,CookieContainer cookieContainer);

        string Get(string url,NameValueCollection data,CookieContainer cookieContainer,int timeout);
        
        string Post(string url,NameValueCollection data);

        string Post(string url,NameValueCollection data,CookieContainer cookieContainer);

        string Post(string url,NameValueCollection data,CookieContainer cookieContainer,Encoding encoding);
    }
}