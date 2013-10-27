#region "Usings"

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Core.BusinessLogic;
using Core.Web.Tools;
using Core.DataObjects;
using Oracle.DataAccess.Client;

#endregion

#region "Implementation"

namespace Core.Web.RestService
{
    #region "Public Implemented Class"

    [ServiceContract(Namespace = "", Name = "JSONP")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RestJsonp
    {
        #region "Public Methods"

        #region "Event Implementation"

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            ResponseFormat = WebMessageFormat.Json)]
        public Stream getEvents(string callback, string state, string acronyms, string type)
        {
            ///Returning the events
            List<Evento> eventos = new Rest().getEvents(state, acronyms, type);

            ///Returning the json stream
            return new Serialize().ObjectToJsonPStream<List<Evento>>(eventos, callback);
        }

        #endregion

        #endregion
    }

    #endregion
}

#endregion