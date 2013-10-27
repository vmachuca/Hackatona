#region "Usings"

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

#endregion

#region "Implementation"

namespace Core.BusinessLogic
{
    #region "Public Class"

    [DataContract]
    public class BestServer : BestServerService.BestServer
    {
        #region "Public Attributes"

        [DataMember]
        public int Setor { get; set; }
        [DataMember]
        public string SiglaGSM { get; set; }
        [DataMember]
        public string SiglaWCDMA { get; set; }
        [DataMember]
        public int Ordem { get; set; }

        #endregion

        #region "Public Methods"

        public List<BestServer> Get(string lat, string lon)
        {
            ///Getting the results
            Core.BestServerService.BestServer[] bestServers = new BestServerService.SOAPService().ConsultarErbs(double.Parse(lon, CultureInfo.InvariantCulture.NumberFormat), double.Parse(lat, CultureInfo.InvariantCulture.NumberFormat));

            ///The dictionary of best servers filtered
            Dictionary<int, Core.BusinessLogic.BestServer> bestServerFiltered = new Dictionary<int, Core.BusinessLogic.BestServer>();

            ///For each bestserver
            foreach (Core.BestServerService.BestServer bestServer in bestServers)
            {
                ///Verify if exist the key
                if (!bestServerFiltered.ContainsKey(bestServer.Ordem))
                {
                    ///Creating the new
                    bestServerFiltered.Add(bestServer.Ordem, new Core.BusinessLogic.BestServer());

                    ///Setting the fields
                    bestServerFiltered[bestServer.Ordem].Ordem = bestServer.Ordem;
                    bestServerFiltered[bestServer.Ordem].UF = bestServer.UF;
                    bestServerFiltered[bestServer.Ordem].Tecnologia = bestServer.Tecnologia;
                    bestServerFiltered[bestServer.Ordem].Setor = bestServer.Setor;
                    bestServerFiltered[bestServer.Ordem].Frequencia = bestServer.Frequencia;
                    bestServerFiltered[bestServer.Ordem].CGI = bestServer.CGI;
                    bestServerFiltered[bestServer.Ordem].X = bestServer.X;
                    bestServerFiltered[bestServer.Ordem].Y = bestServer.Y;
                    bestServerFiltered[bestServer.Ordem].SiglaGSM = string.Empty;
                    bestServerFiltered[bestServer.Ordem].SiglaWCDMA = string.Empty;
                }

                if (bestServer.Tecnologia.ToUpper().Equals("GSM"))
                    bestServerFiltered[bestServer.Ordem].SiglaGSM = bestServer.SiglaErb;
                else if (bestServer.Tecnologia.ToUpper().Equals("WCDMA"))
                    bestServerFiltered[bestServer.Ordem].SiglaWCDMA = bestServer.SiglaErb;
            }

            ///Creating ditinct and creating the list
            return bestServerFiltered.Values.ToList();
        }

        #endregion
    }

    #endregion
    
}

#endregion