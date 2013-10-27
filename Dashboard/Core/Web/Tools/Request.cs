#region "Usings"

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Configuration;
using System.Net.Cache;

#endregion

#region "Implementation"

namespace Core.Web.Tools
{
    #region "Public Class"

    /// <summary>
    /// The class
    /// </summary>
    public class Request
    {
        #region "Public Static Methods"

        public static string Try(string address, string method, bool proxy, Encoding encoding, bool cache)
        {
            try
            {
                HttpWebRequest httpRequest = null;
                Uri uri = new Uri(address);

                httpRequest = (HttpWebRequest)WebRequest.Create(uri);
                httpRequest.Method = method;
                httpRequest.Timeout = 99000;

                if (!cache)
                {
                    HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                    httpRequest.CachePolicy = noCachePolicy;
                    httpRequest.ContentLength = 0;
                }

                if (proxy)
                {
                    string usuario = ConfigurationManager.AppSettings["USUARIO_PROXY"].ToString();
                    string senha = ConfigurationManager.AppSettings["SENHA_PROXY"].ToString();
                    string dominio = ConfigurationManager.AppSettings["DOMINIO_PROXY"].ToString();
                    string endereco = ConfigurationManager.AppSettings["ENDERECO_PROXY"].ToString();

                    WebProxy webProxy = new WebProxy(new Uri(endereco), true);

                    webProxy.Credentials = new NetworkCredential(usuario, senha, dominio);
                    httpRequest.Proxy = webProxy;
                }
                else
                {
                    httpRequest.Proxy = null;
                }

                string result = string.Empty;

                using (HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, encoding))
                        {
                            result = readStream.ReadToEnd();
                        }
                    }
                }

                return result;
            }
            catch
            {
                return "-1";
                
            }
        }

        #endregion
    }

    #endregion
}

#endregion