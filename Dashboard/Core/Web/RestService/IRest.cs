using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Core.BusinessLogic;
using System.IO;
using Core.DataObjects;

namespace Core.Web.RestService
{
    [ServiceContract]
    public interface IRest
    {
        #region "Event Declaration"

        [OperationContract]
        [WebGet(
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "getEvents?state={state}&acronyms={acronyms}&type={type}"
        )]
        List<Evento> getEvents(string state, string acronyms, string type);        

        #endregion
    }
}