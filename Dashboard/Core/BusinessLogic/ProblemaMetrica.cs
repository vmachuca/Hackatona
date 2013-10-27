#region "Usings"

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Core.Web.Utils;

#endregion

#region "Implementation"

namespace Core.BusinessLogic
{
    #region "Public Class"

    [DataContract]
    public class ProblemaMetrica
    {
        #region "Public Web Attributes"

        [DataMember]
        public int IdProblema { get; set; }
        [DataMember]
        public string Problema { get; set; }
        [DataMember]
        public int IdMetrica { get; set; }
        [DataMember]
        public string Metrica { get; set; }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// Gets the problem metrics
        /// </summary>
        /// <returns></returns>
        public List<ProblemaMetrica> Get()
        {
            ///The sql clause
            string sqlClause = @"select 
                                 
                                 DESEM_PROBLEMA_METRICA.ID_PROBLEMA ,
                                 DESEM_PROBLEMA.DESCRICAO           ,
                                 DESEM_PROBLEMA_METRICA.ID_METRICA  ,
                                 DESEM_METRICA.ALIAS

                                 from DESEM_PROBLEMA_METRICA
                                 inner join DESEM_PROBLEMA on DESEM_PROBLEMA.ID = DESEM_PROBLEMA_METRICA.ID_PROBLEMA
                                 inner join DESEM_METRICA on DESEM_METRICA.ID = DESEM_PROBLEMA_METRICA.ID_METRICA";

            ///The data table
            DataTable dataTable = SqlServer.GetDataTable(sqlClause);

            ///Creating the list
            List<ProblemaMetrica> problemMetrics = new List<ProblemaMetrica>();

            ///For each row add the problems 
            foreach (DataRow row in dataTable.Rows)
                problemMetrics.Add(new ProblemaMetrica()
                {
                    IdProblema = (int)row["ID_PROBLEMA"],
                    Problema = row["DESCRICAO"].ToString(),
                    IdMetrica = (int)row["ID_METRICA"],
                    Metrica = row["ALIAS"].ToString()
                });

            return problemMetrics;
        }

        /// <summary>
        /// Inserts the problem metric
        /// </summary>
        /// <param name="problemMetric">The problem metric object</param>
        /// <returns>A message</returns>
        public string Insert(ProblemaMetrica problemMetric)
        {
            ///The sql clause
            string sqlClause = "select * from DESEM_PROBLEMA_METRICA where ID_PROBLEMA = @ID_PROBLEMA and ID_METRICA = @ID_METRICA";

            ///Creating the parameters
            List<SqlServer> parameters = new List<SqlServer>();
            parameters.Add(new SqlServer() { ParameterName = "ID_PROBLEMA", ParameterValue = problemMetric.IdProblema });
            parameters.Add(new SqlServer() { ParameterName = "ID_METRICA", ParameterValue = problemMetric.IdMetrica });

            ///Searching
            DataTable dataTable = SqlServer.GetDataTable(sqlClause, parameters);

            ///Verify if has any row
            if (dataTable.Rows.Count > 1)
                return "Essa relação já existe.";

            sqlClause = "insert into DESEM_PROBLEMA_METRICA (ID_PROBLEMA, ID_METRICA) values (@ID_PROBLEMA, @ID_METRICA)";

            ///Creating the parameters
            parameters = new List<SqlServer>();
            parameters.Add(new SqlServer() { ParameterName = "ID_PROBLEMA", ParameterValue = problemMetric.IdProblema});
            parameters.Add(new SqlServer() { ParameterName = "ID_METRICA", ParameterValue = problemMetric.IdMetrica });

            ///Inserting
            SqlServer.Execute(sqlClause, parameters);

            return string.Empty;
        }

        /// <summary>
        /// Deletes the problem
        /// </summary>
        /// <param name="problemMetrica">The problem object</param>
        public string Delete(ProblemaMetrica problemMetrica)
        {
            ///The sql clause
            string sqlClause = "delete from DESEM_PROBLEMA_METRICA where ID_PROBLEMA = @ID_PROBLEMA and ID_METRICA = @ID_METRICA";

            ///Creating the parameters
            List<SqlServer> parameters = new List<SqlServer>();
            parameters.Add(new SqlServer() { ParameterName = "ID_METRICA", ParameterValue = problemMetrica.IdMetrica });
            parameters.Add(new SqlServer() { ParameterName = "ID_PROBLEMA", ParameterValue = problemMetrica.IdProblema });

            ///Inserting
            SqlServer.Execute(sqlClause, parameters);

            return string.Empty;
        }

        #endregion
    }

    #endregion
}

#endregion