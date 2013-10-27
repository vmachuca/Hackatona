using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Core.BusinessLogic
{
    [DataContract]
    public class Geocode
    {
        public string pUF { get; set; }
        public string pMunicipio { get; set; }
        public string pEstado { get; set; }
        public string pEndereco { get; set; }
        public string pCEP { get; set; }
        public string pPonto { get; set; }

        public double pLatitude { get; set; }
        public double pLongitude { get; set; }

        [DataMember]
        public string IBGE { get; set; }
        [DataMember]
        public string Municipio { get; set; }
        [DataMember]
        public string Estado { get; set; }
        [DataMember]
        public string EnderecoEncontrado { get; set; }
        [DataMember]
        public double X { get; set; }
        [DataMember]
        public double Y { get; set; }

        public List<Geocode> Buscar()
        {
            GeocodeService.SOAPService service = new Core.GeocodeService.SOAPService();
            GeocodeService.IGeocode[] lista = { };
            List<Geocode> listaRetorno = new List<Geocode>();

            try
            {
                lista = service.BuscarEndereco(pUF, pMunicipio, pEndereco, pCEP, pPonto, "", "", "");
            }
            catch
            {
                return listaRetorno;
            }

            foreach (var item in lista)
            {
                listaRetorno.Add(new Geocode()
                {
                    EnderecoEncontrado = item.EnderecoEncontrado,
                    IBGE = item.IBGE,
                    Municipio = item.Municipio,
                    Estado = item.UF,
                    X = item.X,
                    Y = item.Y
                });
            }

            return listaRetorno;
        }

        public List<Geocode> Reverso()
        {
            GeocodeService.SOAPService service = new Core.GeocodeService.SOAPService();
            GeocodeService.IGeocode[] lista = service.GeocodeReverso(pLongitude, pLatitude, "50", "");

            List<Geocode> listaRetorno = new List<Geocode>();

            foreach (var item in lista)
            {
                listaRetorno.Add(new Geocode()
                {
                    EnderecoEncontrado = item.EnderecoEncontrado,
                    IBGE = item.IBGE,
                    Municipio = item.Municipio,
                    Estado = item.UF,
                    X = item.X,
                    Y = item.Y
                });
            }

            return listaRetorno;
        }
    }
}
