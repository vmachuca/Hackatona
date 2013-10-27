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
    public class Problema
    {
        #region "Public Web Attributes"

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Tipo { get; set; }
        [DataMember]
        public string Descricao { get; set; }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// Gets the problems
        /// </summary>
        /// <returns></returns>
        public List<Problema> Get()
        {
            ///The sql clause
            string sqlClause = @"select ID, TIPO_SERV, DESCRICAO from DESEM_PROBLEMA";

            ///The data table
            DataTable dataTable = SqlServer.GetDataTable(sqlClause);

            ///Creating the list
            List<Problema> problems = new List<Problema>();

            ///For each row add the problems 
            foreach (DataRow row in dataTable.Rows)
                problems.Add(new Problema()
                {
                    Id = (int)row["ID"],
                    Tipo = row["TIPO_SERV"].ToString(),
                    Descricao = row["DESCRICAO"].ToString()
                });

            return problems;
        }

        /// <summary>
        /// Inserts the problem
        /// </summary>
        /// <param name="problem">The problem object</param>
        public void Insert(Problema problem)
        {
            ///The sql clause
            string sqlClause = "insert into DESEM_PROBLEMA (TIPO_SERV, DESCRICAO) values (@TIPO_SERV, @DESCRICAO)";

            ///Creating the parameters
            List<SqlServer> parameters = new List<SqlServer>();
            parameters.Add(new SqlServer() { ParameterName = "TIPO_SERV", ParameterValue = problem.Tipo });
            parameters.Add(new SqlServer() { ParameterName = "DESCRICAO", ParameterValue = problem.Descricao });

            ///Inserting
            SqlServer.Execute(sqlClause, parameters);
        }

        /// <summary>
        /// Updates the problem
        /// </summary>
        /// <param name="problem">The problem object</param>
        public void Update(Problema problem)
        {
            ///The sql clause
            string sqlClause = "update DESEM_PROBLEMA set TIPO_SERV=@TIPO_SERV , DESCRICAO=@DESCRICAO where ID=@ID";

            ///Creating the parameters
            List<SqlServer> parameters = new List<SqlServer>();
            parameters.Add(new SqlServer() { ParameterName = "ID", ParameterValue = problem.Id });
            parameters.Add(new SqlServer() { ParameterName = "TIPO_SERV", ParameterValue = problem.Tipo });
            parameters.Add(new SqlServer() { ParameterName = "DESCRICAO", ParameterValue = problem.Descricao });

            ///Inserting
            SqlServer.Execute(sqlClause, parameters);
        }

        /// <summary>
        /// Deletes the problem
        /// </summary>
        /// <param name="problem">The problem object</param>
        public string Delete(Problema problem)
        {
            ///The sql clause
            string sqlClause = @"select * from DESEM_PROBLEMA_METRICA where ID_PROBLEMA=@ID_PROBLEMA";

            ///Creating the parameters
            List<SqlServer> parameters = new List<SqlServer>();
            parameters.Add(new SqlServer() { ParameterName = "ID_PROBLEMA", ParameterValue = problem.Id });

            ///The data table
            DataTable dataTable = SqlServer.GetDataTable(sqlClause, parameters);

            ///Verify if has any row
            if (dataTable.Rows.Count > 0)
                return "Não foi possível excluir o Problema pois existem Métricas relacionadas.";

            ///The sql clause
            sqlClause = "delete from DESEM_PROBLEMA where ID=@ID";

            ///Creating the parameters
            parameters = new List<SqlServer>();
            parameters.Add(new SqlServer() { ParameterName = "ID", ParameterValue = problem.Id });

            ///Inserting
            SqlServer.Execute(sqlClause, parameters);

            return string.Empty;
        }

        #endregion
    }

    #endregion
}

#endregion