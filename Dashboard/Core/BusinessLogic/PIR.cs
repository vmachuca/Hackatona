#region "Usings"

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Core.Web.Tools;
using Oracle.DataAccess.Client;

#endregion

#region "Implementation"

namespace Core.BusinessLogic
{
    #region "Public Class"

    [DataContract(Namespace = "", Name = "Incidencia")]
    public class PIR
    {
        #region "Public Attributes"

        
        public string Terminais { get; set; }
        public string DataEvento { get; set; }
        public string DataCriacao { get; set; }
        public string DataVerificacao { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        #endregion

        #region "Public Web Attributes"

        [DataMember]
        public string Sigla { get; set; }
        [DataMember]
        public string TipoEquipamento { get; set; }
        [DataMember]
        public string Limite { get; set; }
        [DataMember]
        public string Chamadas { get; set; }

        #endregion

        /// <summary>
        /// verifies if exists pir
        /// </summary>
        /// <param name="state">the state</param>
        /// <returns>if exists pir</returns>
        public bool Verify(string state)
        {
            ///Get the pir list
            List<PIR> pirs = this.Get(state);

            ///Verify if bior is not null and if exists any bior for location in that date
            if (pirs != null && pirs.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get the pirs
        /// </summary>
        /// <param name="state">the state</param>
        /// <returns>The pir list</returns>
        public List<PIR> Get(string state)
        {
            ///The connection
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["INCIDENCIA"].ConnectionString);

            ///The incidence limit
            string incidenceLimit = ConfigurationManager.AppSettings["LIMITE_INCIDENCIA"];

            ///Verify if the incidence limit is equals zero.
            if (incidenceLimit == "0")
                incidenceLimit = "";
            else
                incidenceLimit = "top " + incidenceLimit;

            ///The sql clause
            string sqlClause = string.Format(@"
                SELECT {0} 
                    Replace(EQPTO,'.','') AS SIGLA,TIPO_EQPTO,LIMITE,CHAMADAS,TERMINAIS,DATA_EVENTO,DATA_CRIACAO,DATA_VERIFICACAO 
                FROM VW_EVENTOS_MOVEL WHERE LEFT(EQPTO, 2) = '{1}'", incidenceLimit, state);

            ///The command
            SqlCommand command = new SqlCommand(sqlClause, connection);
            
            ///Openning the connection
            connection.Open();

            ///Thr reader
            SqlDataReader reader = command.ExecuteReader();

            ///The pir list
            List<PIR> pirs = new List<PIR>();

            ///For each register
            while (reader.Read())
            {
                ///Creating the pir
                pirs.Add(new PIR()
                {
                    Sigla = reader["Sigla"].ToString(),
                    TipoEquipamento = reader["TIPO_EQPTO"].ToString(),
                    Limite = reader["LIMITE"].ToString(),
                    Chamadas = reader["Chamadas"].ToString(),
                    Terminais = reader["Terminais"].ToString(),
                    DataCriacao = reader["DATA_CRIACAO"].ToString(),
                    DataEvento = reader["DATA_EVENTO"].ToString(),
                    DataVerificacao = reader["DATA_VERIFICACAO"].ToString()
                });
            }

            ///Closing 
            reader.Close();
            connection.Close();

            ///Disposing the command and the connection
            command.Dispose();
            connection.Dispose();

            ///Returning the pirs
            //if (pirs.Count > 0)
            //    return this.JoinOracle(pirs);
            //else
            return pirs;
        }

        /// <summary>
        /// Joins to oracle database
        /// </summary>
        /// <param name="incidences">the incidences</param>
        /// <returns>the pir list</returns>
        public List<PIR> JoinOracle(List<PIR> incidences)
        {
            ///The id string list
            string ids = string.Empty;

            ///For each incidence
            foreach (var incidence in incidences)
                ids += string.Format("'{0}',", incidence.Sigla);

            ///Creating the new sql clause
            string sqlClause = string.Format(@"
                SELECT * FROM (
                SELECT DISTINCT LONGITUDE_DECIMAL, LATITUDE_DECIMAL, UF||SIGLA_ERB AS SIGLA FROM SCIENCE.V_ERB_SETOR5) SCIENCE
                WHERE SIGLA IN ({0})"
            , ids.TrimEnd(','));

            OracleConnection connection = default(OracleConnection);
            connection = new OracleConnection(ConfigurationManager.ConnectionStrings["ORACLE_SCIENCE"].ConnectionString);

            //Conexão com Oracle - Obtem os dados
            connection.Open();

            ///Creating the command
            OracleCommand command = new OracleCommand(sqlClause, connection);
            
            ///Creating the reader
            OracleDataReader reader = command.ExecuteReader();

            ///The pir list
            List<PIR> pirs = new List<PIR>();

            ///For each register
            while (reader.Read())
            {
                ///Creating the new pir
                pirs.Add(new PIR()
                {
                    Sigla = reader["SIGLA"].ToString(),
                    Longitude = reader["LONGITUDE_DECIMAL"].ToString(),
                    Latitude = reader["LATITUDE_DECIMAL"].ToString()

                });
            }

            ///Closing
            reader.Close();
            connection.Close();

            ///Disposing 
            reader.Dispose();
            command.Dispose();
            connection.Dispose();

            ///For each incidence
            foreach (var incidence in incidences)
            {
                ///For each pir
                foreach (var pir in pirs)
                {
                    ///Verify if both have the same sigla
                    if (incidence.Sigla == pir.Sigla)
                    {
                        incidence.Longitude = pir.Longitude;
                        incidence.Latitude = pir.Latitude;
                        break;
                    }
                }
            }

            ///Returning incidencesss
            return incidences;
        }
    }

    #endregion
}

#endregion