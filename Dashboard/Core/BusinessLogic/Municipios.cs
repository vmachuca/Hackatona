using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Serialization;

namespace Core.BusinessLogic
{
    [DataContract]
    public class Municipios
    {
        [DataMember]
        public string IBGE { get; set; }
        [DataMember]
        public string Nome { get; set; }

        public List<Municipios> ObterMunicipios(string uf)
        {
            ///The return list
            List<Municipios> listaRetorno = new List<Municipios>();

            if (!string.IsNullOrEmpty(uf))
            {
                ///Creating the url
                string url = string.Format("http://10.126.111.74/ArcGIS/rest/services/Municipios_IBGE/MapServer/0/query?text=&geometry=&geometryType=esriGeometryPoint&inSR=&spatialRel=esriSpatialRelIntersects&relationParam=&objectIds=&where=Sigla%3D%27{0}%27&time=&returnIdsOnly=false&returnGeometry=false&maxAllowableOffset=&outSR=&outFields=GEOCODIG_M%2CNome_Munic&f=pjson", uf);
                
                ///Creating the http web request
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);

                ///Getting the response
                WebResponse response = httpRequest.GetResponse();

                ///Getting the text
                string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

                JavaScriptSerializer jss = new JavaScriptSerializer();

                IDictionary<string, object> results = jss.DeserializeObject(json) as IDictionary<string, object>;

                if (results != null && results.ContainsKey("features"))
                {
                    IEnumerable<object> candidates = results["features"] as IEnumerable<object>;

                    foreach (IDictionary<string, object> candidate in candidates)
                    {
                        listaRetorno.Add(new Municipios()
                        {
                            Nome = (candidate["attributes"] as IDictionary<string, object>)["Nome_Munic"].ToString().Trim(),
                            IBGE = (candidate["attributes"] as IDictionary<string, object>)["GEOCODIG_M"].ToString().Trim()
                        });
                    }
                }
            }

            return listaRetorno.OrderBy(a => a.Nome).ToList();
        }
    }
}
