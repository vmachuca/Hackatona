#region "Usings"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using Core.DataObjects;

#endregion

#region "Implementation"

namespace Core.BusinessLogic
{
    #region "Public Class"

    [DataContract]
    public class Setor
    {
        #region "Internal Attributes"

        internal bool Ok { get; set; }
        
        internal Dictionary<DateTime, double> Values = new Dictionary<DateTime, double>();

        #endregion

        #region "Public Attributes"

        public double Value { get; set; }

        public bool Alert { get; set; }
        public bool Critical { get; set; }
        public bool HistoryAlert { get; set; }
        public bool HistoryCritical { get; set; }

        public string Hour { get; set; }

        public Grafico Chart { get; set; }

        #endregion

        #region "Public Web Attributes"

        [DataMember]
        public int Number { get; set; }

        #endregion
    }

    #endregion
}

#endregion