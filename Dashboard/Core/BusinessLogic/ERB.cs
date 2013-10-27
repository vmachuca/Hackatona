#region "Usings"

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Core.DataObjects;
using Core.Web.Utils;
using Core.Web.Tools;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

#endregion

#region "Implementation"

namespace Core.BusinessLogic
{
    #region "Public Class"

    [DataContract]
    public class ERB
    {
        #region "Internal Attributes"

        internal string Tecnologia { get; set; }

        #endregion

        #region "Public Attributes"

        public bool Alert { get; set; }
        public bool Critical { get; set; }

        public bool HistoryAlert { get; set; }
        public bool HistoryCritical { get; set; }

        #endregion

        #region "Internal Web Attributes"

        [DataMember]
        internal double Latitude { get; set; }
        [DataMember]
        internal double Longitude { get; set; }

        #endregion

        #region "Public Web Attributes"

        [DataMember]
        public string Sigla { get; set; }

        #endregion

        #region "Private Attributes"

        private int Setor;
        private double QoSUtilizado ;
        private double QoSDisponivel;

        #endregion

        #region "Public Methods"

        /// <summary>
        /// Get the erb by the name
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="erbName">The erb name</param>
        /// <returns>A ERB</returns>
        public ERB Get(string state, string erbName)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            ///The search url
            //string searchUrl = "http://10.126.111.203:6080/arcgis/rest/services/SIGDI_ERBs/MapServer/0/query?where=SIG_ERB+%3D+%27{0}%27+and+UF%3D%27{1}%27&text=&outFields=sig_erb%2C+uf&returnGeometry=true&f=pjson";
            string searchUrl = "http://10.126.111.213/ArcGIS/rest/services/SIG-DI/ERBs/MapServer/0/query?where=SIG_ERB+%3D+%27{0}%27+and+UF%3D%27{1}%27&text=&outFields=sig_erb%2C+uf&returnGeometry=true&f=pjson";

            ///Searching
            string result = Request.Try(string.Format(searchUrl, erbName.ToUpper(), state.ToUpper()), "GET", false, Encoding.UTF8, true);

            var listaDynamic = new[]
            { 
                new { geometry = new {X = .0, Y = .0}, attributes = new {SIG_ERB = "", UF = ""}}
            }.ToList();

            JObject o = JObject.Parse(result);
            var list = JsonConvert.DeserializeAnonymousType(o["features"].ToString(), listaDynamic);

            ///The list of erbs
            List<ERB> erbs = new List<ERB>();

            ///For each row add the erb
            foreach (var item in list)
                if (item.attributes.UF.ToUpper() == state.ToUpper() && item.attributes.SIG_ERB.ToUpper() == erbName.ToUpper())
                {
                    erbs.Add(new ERB()
                    {
                        Latitude = item.geometry.Y,
                        Longitude = item.geometry.X
                    });
                }
            ///Returning
            return erbs.Count > 0 ? erbs[0] : null;
        }

        #endregion
    }

    #endregion
}

#endregion