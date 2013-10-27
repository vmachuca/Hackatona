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

#endregion

#region "Implementation"

namespace Core.BusinessLogic
{
    #region "Public Class"
    
    [DataContract]
    public class BIOR
    {
        #region "Constructor"

        /// <summary>
        /// the main constructor
        /// </summary>
        public BIOR() { }

        #endregion

        #region "Public Web Attributes"

        public string Nome { get; set; }
        
        public string DataOcorrencia { get; set; }
        public string DataAbertura { get; set; }
        public string DataEntrega { get; set; }
        public string DataPrevisaoRetorno { get; set; }
        public string DataNormalizacao { get; set; }
        public string DataFechamento { get; set; }
        public string Usuario { get; set; }
        public string RegiaoAfetada { get; set; }
        public string AreaAfetada { get; set; }
        public string AreaAfetadaComplemento { get; set; }
        public string Ent { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string SessaoRede { get; set; }
        public string Bairro { get; set; }

        #endregion

        #region "Public Web Attributes"

        [DataMember]
        public string Problema { get; set; }
        [DataMember]
        public string CodigoBior { get; set; }
        [DataMember]
        public string CodigoEvento { get; set; }
        [DataMember]
        public string Sigla { get; set; }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// verify if exist problem with the biors
        /// </summary>
        /// <param name="state">the state</param>
        /// <param name="city">the city</param>
        /// <param name="date">the date</param>
        /// <returns>if problems exists return true else false</returns>
        public bool Verify(string state, string city, string date, string hour)
        {
            ///Get the bior list
            List<BIOR> biors = this.Get(state, city, date, hour);

            ///Verify if bior is not null and if exists any bior for location in that date
            if (biors != null && biors.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// gets all the biors
        /// </summary>
        /// <param name="state">the state</param>
        /// <param name="city">the city</param>
        /// <param name="date">the date</param>
        /// <returns>list of bior</returns>
        public List<BIOR> Get(string state, string city, string date, string hour)
        {
            ///The date time
            DateTime dateTime = DateTime.ParseExact(string.Concat(date, " ", hour), "dd/MM/yyyy HH", CultureInfo.InvariantCulture);

            ///Getting the city list
            List<Municipios> cities = new Rest().BuscarMunicipios(state);

            ///City IBGE
            var cityIBGE = (from cityQuery in cities
                            where cityQuery.Nome.ToLower().Contains(city.ToLower())
                            select cityQuery);

            ///The url
            string url = string.Format(
                "http://smap.telespcelular.com.br/smap/servicos/roteiroDinamico/consultaBIOR?login=admin&senha=admin&estado={0}&municipio={1}&dataInicial={2}&dataFinal={3}",
                state,
                cityIBGE.First().IBGE,
                string.Concat((dateTime - new TimeSpan(0, 23, 0, 0)).ToString("ddMMyyyyHH"), "00"),
                string.Concat(dateTime.ToString("ddMMyyyyHH"), "59"));

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

                ///The biors result
                var biors = (from bior in xmlDocument.Descendants("bior")
                             where (bior.Element("latitude").Value != "" && bior.Element("longitude").Value != "")
                             select new BIOR
                             {
                                 CodigoBior = bior.Element("codigoBIOR").Value.Trim(),
                                 CodigoEvento = bior.Element("codigoEvento").Value.Trim(),
                                 Sigla = bior.Element("sigla").Value.Trim(),
                                 Nome = bior.Element("nome").Value.Trim(),
                                 Problema = bior.Element("problema").Value.Trim(),
                                 DataOcorrencia = bior.Element("dataOcorrencia").Value.Trim(),
                                 DataAbertura = bior.Element("dataAbertura").Value.Trim(),
                                 DataEntrega = bior.Element("dataEntrega").Value.Trim(),
                                 DataPrevisaoRetorno = bior.Element("dataPrevisaoRetorno").Value.Trim(),
                                 DataNormalizacao = bior.Element("dataNormalizacao").Value.Trim(),
                                 DataFechamento = bior.Element("dataFechamento").Value.Trim(),
                                 Usuario = bior.Element("usuario").Value.Trim(),
                                 RegiaoAfetada = bior.Element("regiaoAfetada").Value.Trim(),
                                 AreaAfetada = bior.Element("areaAfetada").Value.Trim(),
                                 AreaAfetadaComplemento = bior.Element("areaAfetadaComplemento").Value.Trim().Replace("&#xD;", string.Empty),
                                 Ent = bior.Element("ent").Value.Trim(),
                                 Latitude = double.Parse(bior.Element("latitude").Value.Trim(), CultureInfo.InvariantCulture.NumberFormat),
                                 Longitude = double.Parse(bior.Element("longitude").Value.Trim(), CultureInfo.InvariantCulture.NumberFormat),
                                 SessaoRede = bior.Element("secaoRede").Value.Trim(),
                                 Bairro = bior.Element("bairro").Value.Trim()
                             }).ToList();

                ///Returning the biors
                return biors;
            }
            else return null;
        }

        #endregion
    }

    #endregion
}

#endregion


                









                
//.replace(/\%20/g, ' ')
//.replace(/\%21/g, '!')
//.replace(/\%23/g, '#')
//.replace(/\%24/g, '$')
//.replace(/\%25/g, '%')
//.replace(/\%26/g, '&')
//.replace(/\%28/g, '(')
//.replace(/\%29/g, ')')
//.replace(/\%2A/g, '*')
//.replace(/\%2B/g, '+')
//.replace(/\%2C/g, ',')
//.replace(/\%3A/g, ':')
//.replace(/\%3B/g, ';')
//.replace(/\%3C/g, '<')
//.replace(/\%3D/g, '=')
//.replace(/\%3E/g, '>')
//.replace(/\%3F/g, '?')
//.replace(/\%40/g, '@')
//.replace(/\%5B/g, '[')
//.replace(/\%5C/g, '\')
//.replace(/\%5D/g, ']')
//.replace(/\%5E/g, '^')
//.replace(/\%60/g, '`')
//.replace(/\%7B/g, '{')
//.replace(/\%7C/g, '|')
//.replace(/\%7D/g, '}')
//.replace(/\%7E/g, '~')
//.replace(/\%80/g, '€')
//.replace(/\%82/g, '‚')
//.replace(/\%83/g, 'ƒ')
//.replace(/\%84/g, '„')
//.replace(/\%85/g, '…')
//.replace(/\%86/g, '†')
//.replace(/\%87/g, '‡')
//.replace(/\%88/g, 'ˆ')
//.replace(/\%89/g, '‰')
//.replace(/\%8A/g, 'Š')
//.replace(/\%8B/g, '‹')
//.replace(/\%8C/g, 'Œ')
//.replace(/\%8E/g, 'Ž')
//.replace(/\%91/g, '‘')
//.replace(/\%92/g, '’')
//.replace(/\%93/g, '“')
//.replace(/\%94/g, '”')
//.replace(/\%95/g, '•')
//.replace(/\%96/g, '–')
//.replace(/\%97/g, '—')
//.replace(/\%98/g, '˜')
//.replace(/\%99/g, '™')
//.replace(/\%9A/g, 'š')
//.replace(/\%9B/g, '›')
//.replace(/\%9C/g, 'œ')
//.replace(/\%9E/g, 'ž')
//.replace(/\%9F/g, 'Ÿ')
//.replace(/\%A0/g, ' ')
//.replace(/\%A1/g, '¡')
//.replace(/\%A2/g, '¢')
//.replace(/\%A3/g, '£')
//.replace(/\%A4/g, '¤')
//.replace(/\%A5/g, '¥')
//.replace(/\%A6/g, '¦')
//.replace(/\%A7/g, '§')
//.replace(/\%A8/g, '¨')
//.replace(/\%A9/g, '©')
//.replace(/\%AA/g, 'ª')
//.replace(/\%AB/g, '«')
//.replace(/\%AC/g, '¬')
//.replace(/\%AE/g, '®')
//.replace(/\%AF/g, '¯')
//.replace(/\%B0/g, '°')
//.replace(/\%B1/g, '±')
//.replace(/\%B2/g, '²')
//.replace(/\%B3/g, '³')
//.replace(/\%B4/g, '´')
//.replace(/\%B5/g, 'µ')
//.replace(/\%B6/g, '¶')
//.replace(/\%B7/g, '·')
//.replace(/\%B8/g, '¸')
//.replace(/\%B9/g, '¹')
//.replace(/\%BA/g, 'º')
//.replace(/\%BB/g, '»')
//.replace(/\%BC/g, '¼')
//.replace(/\%BD/g, '½')
//.replace(/\%BE/g, '¾')
//.replace(/\%BF/g, '¿')
//.replace(/\%C0/g, 'À')
//.replace(/\%C1/g, 'Á')
//.replace(/\%C2/g, 'Â')
//.replace(/\%C3/g, 'Ã')
//.replace(/\%C4/g, 'Ä')
//.replace(/\%C5/g, 'Å')
//.replace(/\%C6/g, 'Æ')
//.replace(/\%C7/g, 'Ç')
//.replace(/\%C8/g, 'È')
//.replace(/\%C9/g, 'É')
//.replace(/\%CA/g, 'Ê')
//.replace(/\%CB/g, 'Ë')
//.replace(/\%CC/g, 'Ì')
//.replace(/\%CD/g, 'Í')
//.replace(/\%CE/g, 'Î')
//.replace(/\%CF/g, 'Ï')
//.replace(/\%D0/g, 'Ð')
//.replace(/\%D1/g, 'Ñ')
//.replace(/\%D2/g, 'Ò')
//.replace(/\%D3/g, 'Ó')
//.replace(/\%D4/g, 'Ô')
//.replace(/\%D5/g, 'Õ')
//.replace(/\%D6/g, 'Ö')
//.replace(/\%D7/g, '×')
//.replace(/\%D8/g, 'Ø')
//.replace(/\%D9/g, 'Ù')
//.replace(/\%DA/g, 'Ú')
//.replace(/\%DB/g, 'Û')
//.replace(/\%DC/g, 'Ü')
//.replace(/\%DD/g, 'Ý')
//.replace(/\%DE/g, 'Þ')
//.replace(/\%DF/g, 'ß')
//.replace(/\%E0/g, 'à')
//.replace(/\%E1/g, 'á')
//.replace(/\%E2/g, 'â')
//.replace(/\%E3/g, 'ã')
//.replace(/\%E4/g, 'ä')
//.replace(/\%E5/g, 'å')
//.replace(/\%E6/g, 'æ')
//.replace(/\%E7/g, 'ç')
//.replace(/\%E8/g, 'è')
//.replace(/\%E9/g, 'é')
//.replace(/\%EA/g, 'ê')
//.replace(/\%EB/g, 'ë')
//.replace(/\%EC/g, 'ì')
//.replace(/\%ED/g, 'í')
//.replace(/\%EE/g, 'î')
//.replace(/\%EF/g, 'ï')
//.replace(/\%F0/g, 'ð')
//.replace(/\%F1/g, 'ñ')
//.replace(/\%F2/g, 'ò')
//.replace(/\%F3/g, 'ó')
//.replace(/\%F4/g, 'ô')
//.replace(/\%F5/g, 'õ')
//.replace(/\%F6/g, 'ö')
//.replace(/\%F7/g, '÷')
//.replace(/\%F8/g, 'ø')
//.replace(/\%F9/g, 'ù')
//.replace(/\%FA/g, 'ú')
//.replace(/\%FB/g, 'û')
//.replace(/\%FC/g, 'ü')
//.replace(/\%FD/g, 'ý')
//.replace(/\%FE/g, 'þ')
//.replace(/\%FF/g, 'ÿ')
//                
                
