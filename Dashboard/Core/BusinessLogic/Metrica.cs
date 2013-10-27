#region "Usings"

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
    public class Metrica
    {
        #region "Public Attributes"

        public List<Setor> Sectors = new List<Setor>();

        #endregion

        #region "Public Web Attributes"

        [DataMember]
        public string Nome_SQL { get; set; }
        [DataMember]
        public string Alias { get; set; }
        [DataMember]
        public string Tecnologia { get; set; }
        
        [DataMember]
        public bool Alert { get; set; }
        [DataMember]
        public bool Critical { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Nivel { get; set; }
        [DataMember]
        public string Codigo_SQL { get; set; }
        [DataMember]
        public string Descricao { get; set; }
        [DataMember]
        public double? Ok1 { get; set; }
        [DataMember]
        public double? Ok2 { get; set; }
        [DataMember] 
        public double? Alerta1 { get; set; }
        [DataMember] 
        public double? Alerta2 { get; set; }
        [DataMember] 
        public double? Critico1 { get; set; }
        [DataMember] 
        public double? Critico2 { get; set; }
        [DataMember]
        public string Unit { get; set; }
        [DataMember]
        public string Ativa { get; set; }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// Gets the metric list by problem
        /// </summary>
        /// <param name="problemName">The metric name</param>
        /// <returns></returns>
        public List<Metrica> GetByProblem(string problemName)
        {
            ///Setting the sql clause
            string sqlClause = string.Format(@"select DESEM_METRICA.*
                                               from DESEM_METRICA
                                               inner join DESEM_PROBLEMA_METRICA on DESEM_PROBLEMA_METRICA.ID_METRICA = DESEM_METRICA.ID
                                               inner join DESEM_PROBLEMA on DESEM_PROBLEMA.ID = DESEM_PROBLEMA_METRICA.ID_PROBLEMA
                                               where
                                               DESEM_METRICA.ATIVA = 'SIM' and
                                               UPPER(DESEM_PROBLEMA.DESCRICAO) = UPPER('{0}')", problemName);

            ///Getting the data table
            DataTable dataTable = SqlServer.GetDataTable(sqlClause);

            ///The metric list
            List<Metrica> metrics = new List<Metrica>();

            ///For each row
            foreach(DataRow row in dataTable.Rows)

            {
                ///Creating the metric
                Metrica metric = new Metrica()
                {
                    Id = (int)row["ID"],
                    Nome_SQL = row["NOME_SQL"].ToString(),
                    Nivel = row["NIVEL"].ToString(),
                    Tecnologia = row["TECNOLOGIA"].ToString(),
                    Alias = row["ALIAS"].ToString(),
                    Codigo_SQL = row["CODIGO_SQL"].ToString(),
                    Descricao = row["DESCRICAO"].ToString(),
                    Ok1 = !Utils.VerifyEmpty(row["OK1"]) ? (double)row["OK1"] : 0,
                    Ok2 = !Utils.VerifyEmpty(row["OK2"]) ? (double)row["OK2"] : 0,
                    Alerta1 = !Utils.VerifyEmpty(row["ALERTA1"]) ? (double)row["ALERTA1"] : -1,
                    Alerta2 = !Utils.VerifyEmpty(row["ALERTA2"]) ? (double)row["ALERTA2"] : -1,
                    Critico1 = !Utils.VerifyEmpty(row["CRITICO1"]) ? (double)row["CRITICO1"] : -1,
                    Critico2 = !Utils.VerifyEmpty(row["CRITICO2"]) ? (double)row["CRITICO2"] : -1,
                    Unit = row["UNIDADE"].ToString().Trim(),
                    Ativa = row["ATIVA"].ToString()
                };

                ///Adding the metrics
                metrics.Add(metric);
            }

            ///Returning the metrics
            return metrics;
        }

        /// <summary>
        /// Gets the metrics
        /// </summary>
        /// <returns></returns>
        public List<Metrica> Get()
        {
            ///The sql clause
            string sqlClause = @"select ID, 
                                 NOME_SQL    ,  NIVEL      ,  TECNOLOGIA ,  ALIAS      ,
                                 CODIGO_SQL  ,  DESCRICAO  ,  OK1        ,  OK2        ,
                                 ALERTA1     ,  ALERTA2    ,  CRITICO1   ,  CRITICO2   ,
                                 UNIDADE     ,  ATIVA      
                                 from DESEM_METRICA";

            ///The data table
            DataTable dataTable = SqlServer.GetDataTable(sqlClause);

            ///Creating the list
            List<Metrica> metricss = new List<Metrica>();

            ///For each row add the metricss 
            foreach (DataRow row in dataTable.Rows)
                metricss.Add(new Metrica()
                {
                    Id = (int)row["ID"],
                    Nome_SQL = row["NOME_SQL"].ToString(),
                    Nivel = row["NIVEL"].ToString(),
                    Tecnologia = row["TECNOLOGIA"].ToString(),
                    Alias = row["ALIAS"].ToString(),
                    Codigo_SQL = row["CODIGO_SQL"].ToString(),
                    Descricao = row["DESCRICAO"].ToString(),
                    Ok1 = !Utils.VerifyEmpty(row["OK1"]) ? (double)row["OK1"] : 0,
                    Ok2 = !Utils.VerifyEmpty(row["OK2"]) ? (double)row["OK2"] : 0,
                    Alerta1 = !Utils.VerifyEmpty(row["ALERTA1"]) ? (double)row["ALERTA1"] : -1,
                    Alerta2 = !Utils.VerifyEmpty(row["ALERTA2"]) ? (double)row["ALERTA2"] : -1,
                    Critico1 = !Utils.VerifyEmpty(row["CRITICO1"]) ? (double)row["CRITICO1"] : -1,
                    Critico2 = !Utils.VerifyEmpty(row["CRITICO2"]) ? (double)row["CRITICO2"] : -1,
                    Unit = row["UNIDADE"].ToString().Trim(),
                    Ativa = row["ATIVA"].ToString()
                });

            return metricss;
        }

        /// <summary>
        /// Clones the object
        /// </summary>
        /// <returns>the cloned object</returns>
        public Metrica Clone()
        {
            ///Returning the metric
            return new Metrica()
            {
                Alias = this.Alias,
                Nome_SQL = this.Nome_SQL,
                Ok1 = this.Ok1,
                Ok2 = this.Ok2,
                Alerta1 = this.Alerta1,
                Alerta2 = this.Alerta2,
                Critico1 = this.Critico1,
                Critico2 = this.Critico2,
                Alert = this.Alert,
                Critical = this.Critical
            };
        }

        /// <summary>
        /// Inserts the metric
        /// </summary>
        /// <param name="metric">The metric object</param>
        public void Insert(Metrica metric)
        {
            ///The sql clause
            string sqlClause = @"insert into DESEM_METRICA (
                                 NOME_SQL    ,  NIVEL      ,  TECNOLOGIA ,  ALIAS      ,
                                 CODIGO_SQL  ,  DESCRICAO  ,  OK1        ,  OK2        ,
                                 ALERTA1     ,  ALERTA2    ,  CRITICO1   ,  CRITICO2   ,
                                 UNIDADE     ,  ATIVA      )
                                 values (
                                 @NOME_SQL   , @NIVEL      , @TECNOLOGIA , @ALIAS      ,
                                 @CODIGO_SQL , @DESCRICAO  , @OK1        , @OK2        ,
                                 @ALERTA1    , @ALERTA2    , @CRITICO1   , @CRITICO2   ,
                                 @UNIDADE    , @ATIVA      )";

            ///Creating the parameters
            List<SqlServer> parameters = new List<SqlServer>();
            parameters.Add(new SqlServer() { ParameterName = "NOME_SQL", ParameterValue = metric.Nome_SQL });
            parameters.Add(new SqlServer() { ParameterName = "NIVEL", ParameterValue = metric.Nivel });
            parameters.Add(new SqlServer() { ParameterName = "TECNOLOGIA", ParameterValue = metric.Tecnologia });
            parameters.Add(new SqlServer() { ParameterName = "ALIAS", ParameterValue = metric.Alias });
            parameters.Add(new SqlServer() { ParameterName = "CODIGO_SQL", ParameterValue = metric.Codigo_SQL });
            parameters.Add(new SqlServer() { ParameterName = "DESCRICAO", ParameterValue = metric.Descricao });
            parameters.Add(new SqlServer() { ParameterName = "OK1", ParameterValue = metric.Ok1 });
            parameters.Add(new SqlServer() { ParameterName = "OK2", ParameterValue = metric.Ok2 });
            parameters.Add(new SqlServer() { ParameterName = "ALERTA1", ParameterValue = metric.Alerta1 });
            parameters.Add(new SqlServer() { ParameterName = "ALERTA2", ParameterValue = metric.Alerta2 });
            parameters.Add(new SqlServer() { ParameterName = "CRITICO1", ParameterValue = metric.Critico1 });
            parameters.Add(new SqlServer() { ParameterName = "CRITICO2", ParameterValue = metric.Critico2 });
            parameters.Add(new SqlServer() { ParameterName = "UNIDADE", ParameterValue = metric.Unit });
            parameters.Add(new SqlServer() { ParameterName = "ATIVA", ParameterValue = metric.Ativa });

            ///Inserting
            SqlServer.Execute(sqlClause, parameters);
        }

        /// <summary>
        /// Updates the metric
        /// </summary>
        /// <param name="metric">The metric object</param>
        public void Update(Metrica metric)
        {
            ///The sql clause
            string sqlClause = @"update DESEM_METRICA set 
                                 NOME_SQL   = @NOME_SQL   ,
                                 NIVEL      = @NIVEL      ,
                                 TECNOLOGIA = @TECNOLOGIA ,
                                 ALIAS      = @ALIAS      ,
                                 CODIGO_SQL = @CODIGO_SQL ,
                                 DESCRICAO  = @DESCRICAO  ,
                                 OK1        = @OK1        ,
                                 OK2        = @OK2        ,
                                 ALERTA1    = @ALERTA1    ,
                                 ALERTA2    = @ALERTA2    ,
                                 CRITICO1   = @CRITICO1   ,
                                 CRITICO2   = @CRITICO2   ,
                                 UNIDADE    = @UNIDADE    ,  
                                 ATIVA      = @ATIVA
                                 where 
                                 ID         = @ID";

            ///Creating the parameters
            List<SqlServer> parameters = new List<SqlServer>();
            parameters.Add(new SqlServer() { ParameterName = "ID", ParameterValue = metric.Id });
            parameters.Add(new SqlServer() { ParameterName = "NOME_SQL", ParameterValue = metric.Nome_SQL });
            parameters.Add(new SqlServer() { ParameterName = "NIVEL", ParameterValue = metric.Nivel });
            parameters.Add(new SqlServer() { ParameterName = "TECNOLOGIA", ParameterValue = metric.Tecnologia });
            parameters.Add(new SqlServer() { ParameterName = "ALIAS", ParameterValue = metric.Alias });
            parameters.Add(new SqlServer() { ParameterName = "CODIGO_SQL", ParameterValue = metric.Codigo_SQL });
            parameters.Add(new SqlServer() { ParameterName = "DESCRICAO", ParameterValue = metric.Descricao });
            parameters.Add(new SqlServer() { ParameterName = "OK1", ParameterValue = metric.Ok1 });
            parameters.Add(new SqlServer() { ParameterName = "OK2", ParameterValue = metric.Ok2 });
            parameters.Add(new SqlServer() { ParameterName = "ALERTA1", ParameterValue = metric.Alerta1 });
            parameters.Add(new SqlServer() { ParameterName = "ALERTA2", ParameterValue = metric.Alerta2 });
            parameters.Add(new SqlServer() { ParameterName = "CRITICO1", ParameterValue = metric.Critico1 });
            parameters.Add(new SqlServer() { ParameterName = "CRITICO2", ParameterValue = metric.Critico2 });
            parameters.Add(new SqlServer() { ParameterName = "UNIDADE", ParameterValue = metric.Unit });
            parameters.Add(new SqlServer() { ParameterName = "ATIVA", ParameterValue = metric.Ativa });

            ///Inserting
            SqlServer.Execute(sqlClause, parameters);
        }

        /// <summary>
        /// Deletes the metric
        /// </summary>
        /// <param name="metric">The metric object</param>
        public string Delete(Metrica metric)
        {
            ///The sql clause
            string sqlClause = @"select * from DESEM_PROBLEMA_METRICA where ID_METRICA=@ID_METRICA";

            ///Creating the parameters
            List<SqlServer> parameters = new List<SqlServer>();
            parameters.Add(new SqlServer() { ParameterName = "ID_METRICA", ParameterValue = metric.Id });

            ///The data table
            DataTable dataTable = SqlServer.GetDataTable(sqlClause, parameters);

            ///Verify if has any row
            if (dataTable.Rows.Count > 0)
                return "Não foi possível excluir a Métrica pois existem Problemas relacionados.";

            ///The sql clause
            sqlClause = "delete from DESEM_METRICA where ID=@ID";

            ///Creating the parameters
            parameters = new List<SqlServer>();
            parameters.Add(new SqlServer() { ParameterName = "ID", ParameterValue = metric.Id });

            ///Inserting
            SqlServer.Execute(sqlClause, parameters);

            return string.Empty;
        }

        #endregion
    }

    #endregion
}

#endregion