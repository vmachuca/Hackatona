#region "Usingss"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

#endregion

#region "Implementation"

namespace Core.DataObjects
{
    #region "Public Class"

    [DataContract]
    public class Grafico
    {
        #region "Public Web Attributes"

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public string subtitle { get; set; }

        [DataMember]
        public List<string> categories = new List<string>();

        [DataMember]
        public List<double> series = new List<double>();

        #endregion

        #region "Internal Web Attributes"

        [DataMember]
        internal string yTitle { get; set; }

        [DataMember]
        internal List<List<double>> seriesList = new List<List<double>>();

        [DataMember]
        internal List<string> seriesNameList = new List<string>();

        [DataMember]
        internal string Sigla { get; set; }

        #endregion
    }

    #endregion
}

#endregion