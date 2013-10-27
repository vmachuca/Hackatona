using System;
using System.Collections.Generic;
using System.Text;

namespace Core.BusinessLogic
{
    public class Cobertura
    {
        public double Latitutde { get; set; }
        public double Longitude { get; set; }

        public CoberturaService.Cobertura ObterCobertura()
        {
            try
            {
                //Inclui a chamada no monitore
                Core.Web.Tools.Request.Try("http://10.126.111.213/monitore/rest.svc/log?projeto=17&modulo=15&funcao=1&ativo=1", "GET", false, Encoding.UTF8, false);
            }
            catch { }

            CoberturaService.SOAPService service = new CoberturaService.SOAPService();
            CoberturaService.Cobertura cobertura = service.Buscar(Longitude, Latitutde);
            return cobertura;
        }
    }
}