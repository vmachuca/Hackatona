#region "Usings"

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

#endregion

#region "Implementation"

namespace Core.BusinessLogic
{
    #region "Public Class"
    
    [DataContract]
    public class Alarme
    {
        #region "Constructor"

        /// <summary>
        /// the main constructor
        /// </summary>
        public Alarme() { }

        #endregion

        #region "Public Web Attributes"

        [DataMember]
        public string Sigla { get; set; }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// verify if exist alarms
        /// </summary>
        /// <param name="state">the state</param>
        /// <returns>if problems exists return true else false</returns>
        public bool Verify(string state)
        {
            ///Get the alarm list
            List<Alarme> alarms = this.Get(state);

            ///Verify if alarms is not null and if exists any alarm for location in that date
            if (alarms != null && alarms.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// gets all the alarms
        /// </summary>
        /// <param name="state">the state</param>
        /// <returns>list of alarm</returns>
        public List<Alarme> Get(string state)
        {
            ///The url
            string url = string.Format(
                "http://10.126.111.210/ibi_apps/WFServlet?IBIF_webapp=/ibi_apps&IBIC_server=EDASERVE&IBIWF_msgviewer=OFF&IBIAPP_app=cmd_aar&IBIC_user=wfscmd&IBIF_ex=elementos_param.fex&UF={0}",
                state);

            ///Creating the http web request
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);

            ///Getting the response
            WebResponse response = httpRequest.GetResponse();

            ///Getting the text
            string resultString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            ///Verify if its null
            if (!string.IsNullOrEmpty(resultString))
            {
                ///The xml document
                XDocument xmlDocument = XDocument.Parse(resultString.Replace("&lt;", "<").Replace("&gt;", ">").Replace("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>", ""));

                ///The alarms result
                var alarmsQuery = (from alarm in xmlDocument.Descendants("fxf").Descendants("report").Descendants("table").Descendants("tr")
                                   select alarm.Descendants("td")).ToList();

                ///The alarms list
                List<Alarme> alarms = new List<Alarme>();

                ///For each alarm in the query
                foreach (var alarmQuery in alarmsQuery)
                {
                    ///Creating the alrarm
                    Alarme alarm = new Alarme();

                    ///For each element in the alarm query
                    foreach (XElement alarmElement in alarmQuery.ToList())
                    {
                        if (alarmElement.Attribute("colnum").Value.Equals("c0"))
                            alarm.Sigla = alarmElement.Value + alarm.Sigla;
                        else if (alarmElement.Attribute("colnum").Value.Equals("c2"))
                            alarm.Sigla = alarm.Sigla + alarmElement.Value;
                    }

                    alarms.Add(alarm);
                }

                ///Returning the alarms
                return alarms;
            }
            else return null;
        }

        #endregion
    }

    #endregion
}

#endregion