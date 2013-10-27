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
using Core.Web.RestService;
using Core.Web.Tools;

#endregion

#region "Implementation"

namespace Core.BusinessLogic
{
    #region "Public Class"

    [DataContract]
    public class Reclamacao
    {
        #region "Constructor"

        /// <summary>
        /// the main constructor
        /// </summary>
        public Reclamacao() { }

        #endregion

        #region "Public Web Attributes"

        [DataMember]
        public string DataAtendimento { get; set; }

        [DataMember]
        public string DetalheReclamacaoAtendimento { get; set; }

        [DataMember]
        public string NomeAtendente { get; set; }

        [DataMember]
        public string NumeroAtendimento { get; set; }

        [DataMember]
        public string NumeroLinha { get; set; }

        [DataMember]
        public string Plano { get; set; }

        [DataMember]
        public string ReclamacaoAtendimento { get; set; }

        [DataMember]
        public string Tecnologia { get; set; }

        [DataMember]
        public string CodigoTipoOcorrencia { get; set; }

        [DataMember]
        public string DescricaoTipoOcorrencia { get; set; }

        [DataMember]
        public string PesquisaRealizada { get; set; }

        [DataMember]
        public string LocalizacaoSelecionada { get; set; }

        [DataMember]
        public string CoberturaGSM { get; set; }

        [DataMember]
        public string Cobertura3G { get; set; }

        [DataMember]
        public string BIOR { get; set; }

        [DataMember]
        public double Latitude { get; set; }

        [DataMember]
        public double Longitude { get; set; }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// verify if exist problem with the complaints
        /// </summary>
        /// <param name="state">the state</param>
        /// <param name="city">the city</param>
        /// <param name="date">the date</param>
        /// <returns>if problems exists return true else false</returns>
        public bool Verify(string state, string city, string date, string hour)
        {
            ///Get the complaint list
            List<Reclamacao> complaints = this.Get(state, city, date, hour);

            ///Verify if complaint is not null and if exists any complaint for location in that date
            if (complaints != null && complaints.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// gets all the complaints
        /// </summary>
        /// <param name="state">the state</param>
        /// <param name="city">the city</param>
        /// <param name="date">the date</param>
        /// <returns>list of complaint</returns>
        public List<Reclamacao> Get(string state, string city, string date, string hour)
        {
            ///The date time
            DateTime dateTime = DateTime.ParseExact(string.Concat(date, " ", hour), "dd/MM/yyyy HH", CultureInfo.InvariantCulture);

            ///The url
            string url = string.Format(
                "http://10.128.184.83/smap/servicos/roteiroDinamico/consultaAtendimento?login=admin&senha=admin&estado={0}&municipio={1}&dataInicial={2}&dataFinal={3}",
                state,
                city,
                string.Concat((dateTime - new TimeSpan(0, 23, 0, 0)).ToString("ddMMyyyyHH"), "00"),
                string.Concat(dateTime.ToString("ddMMyyyyHH"), "59"));

            string resultString = Request.Try(url, "GET", false, Encoding.UTF7, true);

            ///Verify if its null
            if (!string.IsNullOrEmpty(resultString))
            {
                ///The xml document
                XDocument xmlDocument = XDocument.Parse(resultString.Replace("&lt;", "<").Replace("&gt;", ">").Replace("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>", ""));

                ///The complaint result
                var complaints = (from complaint in xmlDocument.Descendants("reclamacao")
                                  where (complaint.Element("latitude").Value != "" && complaint.Element("longitude").Value != "")
                                  select new Reclamacao()
                                  {
                                      DataAtendimento = DateTime.ParseExact(complaint.Element("dataAtendimento").Value.Trim(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"),
                                      DetalheReclamacaoAtendimento = (complaint.Element("detalheReclamacaoAtendimento") != null ? complaint.Element("detalheReclamacaoAtendimento").Value : ""),
                                      Latitude = double.Parse(complaint.Element("latitude").Value.Trim(), CultureInfo.InvariantCulture.NumberFormat),
                                      Longitude = double.Parse(complaint.Element("longitude").Value.Trim(), CultureInfo.InvariantCulture.NumberFormat),
                                      NomeAtendente = complaint.Element("nomeAtendente").Value.Trim(),
                                      NumeroAtendimento = complaint.Element("numeroAtendimento").Value.Trim(),
                                      NumeroLinha = complaint.Element("numLinha").Value.Trim(),
                                      Plano = complaint.Element("plano").Value.Trim(),
                                      ReclamacaoAtendimento = (complaint.Element("reclamacaoAtendimento") != null ? complaint.Element("reclamacaoAtendimento").Value : ""),
                                      Tecnologia = complaint.Element("tecnologia").Value.Trim(),
                                      CodigoTipoOcorrencia = complaint.Element("codigoTipoOcorrencia").Value,
                                      DescricaoTipoOcorrencia = complaint.Element("descricaoTipoOcorrencia").Value,
                                      PesquisaRealizada = complaint.Element("pesquisaRealizada").Value,
                                      LocalizacaoSelecionada = complaint.Element("localizacaoSelecionada").Value,
                                      CoberturaGSM = complaint.Element("coberturaGSM").Value,
                                      Cobertura3G = complaint.Element("cobertura3G").Value,
                                      BIOR = complaint.Element("bior").Value
                                  }).ToList();

                ///Returning the complaints
                return complaints;
            }
            else return null;
        }

        #endregion
    }

    #endregion
}

#endregion