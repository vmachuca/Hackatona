#region "Usings"

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Core.Web.Utils;
using System.Data;
using System.Configuration;

#endregion

#region "Implementation"

namespace Core.BusinessLogic
{
    #region "Public Class"

    [DataContract]
    public class EventoCritico
    {
        #region "Internal Web Attributes"

        [DataMember]
        public string SGL_UF { get; set; }
        [DataMember]
        public int TOTAL { get; set; }

        #endregion
    }

    [DataContract]
    public class EventoJobDate
    {
        #region "Internal Web Attributes"

        [DataMember]
        public string Date { get; set; }
        [DataMember]
        public string Time { get; set; }

        [DataMember]
        public string DateMask = "dd/MM/yyyy";
        [DataMember]
        public string TimeMask = "HH:mm:ss";

        [DataMember]
        public string CurrentDate { get; set; }
        [DataMember]  
        public string CurrentTime { get; set; }
                      
        [DataMember]  
        public string CurrentDateMask = "dd/MM/yyyy";
        [DataMember]  
        public string CurrentTimeMask = "HH:mm:ss";

        #endregion
    }

    [DataContract]
    public class Evento
    {
        #region "Internal Web Attributes"

        [DataMember]
        public int COD_EVENTO { get; set; }
        [DataMember]
        public string SGL_UF { get; set; }
        [DataMember]
        public string TIPO_ENTIDADE { get; set; }
        [DataMember]
        public string SIGLA { get; set; }
        [DataMember]
        public string STR_IMPACTO { get; set; }
        [DataMember]
        public string DES_AREA_TECNICA_CAUSA { get; set; }
        [DataMember]
        public string DES_TIPO_EVENTO_CAUSA { get; set; }
        [DataMember]
        public string DES_DEFEITO { get; set; }
        [DataMember]
        public string TEMPO_RESOLUCAO_EQP_HH_MM { get; set; }
        [DataMember]
        public string DAT_ALARME_EQP_EVENTO { get; set; }
        [DataMember]
        public string DAT_ENTREGA { get; set; }
        [DataMember]
        public string DAT_FECHAMENTO { get; set; }
        [DataMember]
        public string DES_RESOLUCAO_EVENTO { get; set; }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// Gets the event
        /// </summary>
        /// <param name="state">the state</param>
        /// <param name="date">the date</param>
        /// <param name="hour">the hour</param>
        /// <returns>the event list</returns>
        public List<EventoCritico> GetCriticalCounts()
        {
            ///The sql clause
            string sqlClause = @"
                SELECT
                UF,
                COUNT(UF) AS TOTAL
                FROM
                ERBS_SIG_A
                WHERE
                EXISTS(SELECT NULL FROM SMAPREL_EVENTOS WHERE STR_IMPACTO = 'TOTAL' AND SGL_UF = UF AND SIGLA = (CASE WHEN SIG_LOGICA IS NULL THEN SIG_ERB ELSE SIG_LOGICA END))
                GROUP BY
                UF";

            ///The event list
            List<EventoCritico> events = new List<EventoCritico>();

            ///Getting the data table
            DataTable dataTable = SqlServer.GetDataTable(sqlClause, ConfigurationManager.ConnectionStrings["SQLSERVER_DERBE"].ConnectionString);

            ///For each row
            foreach (DataRow row in dataTable.Rows)
                ///Adding the event
                events.Add(new EventoCritico()
                {
                    SGL_UF = row["UF"].ToString(),
                    TOTAL = int.Parse(row["TOTAL"].ToString())
                });

            ///Returning the eventos
            return events;   
        }

        /// <summary>
        /// Gets the job date
        /// </summary>
        /// <returns>The event job date object</returns>
        public EventoJobDate GetJobDate()
        {
            ///The sql clause
            string sqlClause = @"
                                SELECT TOP 1
                                CONVERT(DATETIME, RTRIM(
                                    [msdb].[dbo].[sysjobhistory].[run_date])) + 
                                (([msdb].[dbo].[sysjobhistory].[run_time]/10000 * 3600) + 
                                (([msdb].[dbo].[sysjobhistory].[run_time]%10000)/100*60) + 
                                    ([msdb].[dbo].[sysjobhistory].[run_time]%10000)%100) / (86399.9964 ) as [run_datetime],
                                    GETDATE() as [current_datetime]
                                    FROM 
	                                [msdb].[dbo].[sysjobhistory]
	                                INNER JOIN
		                                [msdb].[dbo].[sysjobs] 
		                                ON
			                                [msdb].[dbo].[sysjobs].[job_id] = [msdb].[dbo].[sysjobhistory].[job_id]
                                    WHERE 
                                    [msdb].[dbo].[sysjobhistory].[step_id] = 0 AND 
                                    [msdb].[dbo].[sysjobhistory].[run_status] = 1 AND
                                    [msdb].[dbo].[sysjobs].[name] = 'SMAPREL_EVENTOS_EXEC'
                                    ORDER BY 
                                    [run_datetime] DESC
                                ";

            ///The event list
            List<Evento> events = new List<Evento>();

            ///Getting the data table
            DataTable dataTable = SqlServer.GetDataTable(sqlClause, ConfigurationManager.ConnectionStrings["SQLSERVER_DERBE"].ConnectionString);
            
            ///Returning the eventos
            return new EventoJobDate()
            {
                Date = DateTime.Parse(dataTable.Rows[0]["run_datetime"].ToString()).ToString(new EventoJobDate().DateMask),
                Time = DateTime.Parse(dataTable.Rows[0]["run_datetime"].ToString()).ToString(new EventoJobDate().TimeMask),
                CurrentDate = DateTime.Parse(dataTable.Rows[0]["current_datetime"].ToString()).ToString(new EventoJobDate().CurrentDateMask),
                CurrentTime = DateTime.Parse(dataTable.Rows[0]["current_datetime"].ToString()).ToString(new EventoJobDate().CurrentTimeMask)

            };
        }

        /// <summary>
        /// Gets the event
        /// </summary>
        /// <param name="state">the state</param>
        /// <param name="date">the date</param>
        /// <param name="hour">the hour</param>
        /// <returns>the event list</returns>
        public List<Evento> Get(string state, string acronyms, string type)
        {
            ///The where clause
            string whereClause = string.Empty;

            ///For each acronym
            foreach( string acronym in acronyms.Split(','))
            {
                if (whereClause.Equals(string.Empty))
                    whereClause = string.Concat("'", acronym, "'");
                else
                    whereClause += string.Concat(",'", acronym, "'");
            }

            whereClause = string.Format("SGL_UF = '{0}' AND SIGLA in ({1})",state , whereClause);

            ///Verify if the type is null or empty
            if (type != null && !type.Equals(string.Empty))
                whereClause += string.Format(" AND STR_IMPACTO IN ({0})", (type.ToUpper().Equals("PARCIAL") ? "'PARCIAL','INTERMITENTE'" : "'TOTAL'"));
            ///The sql clause
            string sqlClause = string.Format(
                                @"SELECT
                COD_EVENTO                ,  
                SGL_UF                    ,
                TIPO_ENTIDADE             ,
                SIGLA                     ,
                STR_IMPACTO               ,
                DES_AREA_TECNICA_CAUSA    ,
                DES_TIPO_EVENTO_CAUSA     ,
                DES_DEFEITO               ,
                TEMPO_RESOLUCAO_EQP_HH_MM ,
                DAT_ALARME_EQP_EVENTO     ,
                DAT_ENTREGA               ,
                DAT_FECHAMENTO            ,
                DES_RESOLUCAO_EVENTO      ,
                FONTE_ID                  ,
                DES_ESTADO
                
                FROM 
                SMAPREL_EVENTOS
                
                WHERE   
                {0}", whereClause);

            ///The event list
            List<Evento> events = new List<Evento>();

            ///Getting the data table
            DataTable dataTable = SqlServer.GetDataTable(sqlClause, ConfigurationManager.ConnectionStrings["SQLSERVER_DERBE"].ConnectionString);

            ///For each row
            foreach (DataRow row in dataTable.Rows)
            {
                ///Adding the event
                Evento evt = new Evento()
                {
                    COD_EVENTO = int.Parse(row["COD_EVENTO"].ToString()),
                    SGL_UF = row["SGL_UF"].ToString(),
                    TIPO_ENTIDADE = row["TIPO_ENTIDADE"].ToString(),
                    SIGLA = row["SIGLA"].ToString(),
                    STR_IMPACTO = row["STR_IMPACTO"].ToString(),
                    DES_AREA_TECNICA_CAUSA = row["DES_AREA_TECNICA_CAUSA"].ToString(),
                    DES_TIPO_EVENTO_CAUSA = row["DES_TIPO_EVENTO_CAUSA"].ToString(),
                    DES_DEFEITO = row["DES_DEFEITO"].ToString(),
                    TEMPO_RESOLUCAO_EQP_HH_MM = row["TEMPO_RESOLUCAO_EQP_HH_MM"].ToString(),
                    DES_RESOLUCAO_EVENTO = row["DES_RESOLUCAO_EVENTO"].ToString()
                };

                DateTime DAT_ALARME_EQP_EVENTO;
                DateTime DAT_ENTREGA;
                DateTime DAT_FECHAMENTO; ;

                evt.DAT_ALARME_EQP_EVENTO = DateTime.TryParse(row["DAT_ALARME_EQP_EVENTO"].ToString(), out DAT_ALARME_EQP_EVENTO) ? DAT_ALARME_EQP_EVENTO.ToString("dd/MM/yyyy hh:mm") : "";
                evt.DAT_ENTREGA = DateTime.TryParse(row["DAT_ENTREGA"].ToString(), out DAT_ENTREGA) ? DAT_ENTREGA.ToString("dd/MM/yyyy hh:mm") : "";
                evt.DAT_FECHAMENTO = DateTime.TryParse(row["DAT_FECHAMENTO"].ToString(), out DAT_FECHAMENTO) ? DAT_FECHAMENTO.ToString("dd/MM/yyyy hh:mm") : "";

                events.Add(evt);
            }

            ///Returning the eventos
            return events;
        }

        #endregion
    }

    #endregion
}

#endregion