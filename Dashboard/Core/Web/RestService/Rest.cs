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
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Core.BusinessLogic;
using Core.DataObjects;
using Core.Web.Tools;

#endregion

#region "Implementation"

namespace Core.Web.RestService
{
    #region "Public Implemented Class"

    public class Rest : IRest
    {
        #region "Public Methods"

        #region "Event Implementation"

        public List<Evento> getEvents(string state, string acronyms, string type)
        {
            ///Returning the events
            return new Evento().Get(state, acronyms, type);
        }

        #endregion

        #endregion
    }

    #endregion
}

#endregion