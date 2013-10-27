#region "Usings"

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;

#endregion

#region "Implementation"

namespace Core.Web.Utils
{
    #region "Public Class"

    public class OracleAcess
    {
        #region "Public Attributes"

        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }

        #endregion

        #region "Public Static Methods"

        /// <summary>
        /// Gets the data table from the sql clause
        /// </summary>
        /// <param name="sqlClause">The sql Clause</param>
        /// <param name="connectionString">The conenction string</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sqlClause)
        {
            ///Returning the data table
            return GetDataTable(sqlClause, ConfigurationManager.ConnectionStrings["ORACLE"].ConnectionString);
        }
        public static DataTable GetDataTable(string sqlClause, string connectionString)
        {
            ///Returning the data table
            return GetDataTable(sqlClause, connectionString, null);
        }
        public static DataTable GetDataTable(string sqlClause, List<OracleAcess> parameters)
        {
            ///Returning the data table
            return GetDataTable(sqlClause, ConfigurationManager.ConnectionStrings["ORACLE"].ConnectionString, parameters);
        }
        public static DataTable GetDataTable(string sqlClause, string connectionString, List<OracleAcess> parameters)
        {
            ///The return data table
            DataTable dataTable = new DataTable();

            ///Creating the connection
            OracleConnection connection = new OracleConnection(connectionString);

            ///Creating the data adapter
            OracleDataAdapter dataAdapter = new OracleDataAdapter(sqlClause, connection);

            ///If the parameters are not null
            if (parameters != null)
                ///For each parameter
                foreach (OracleAcess parameter in parameters)
                    dataAdapter.SelectCommand.Parameters.Add(parameter.ParameterName, parameter.ParameterValue);

            ///Openning the connection
            connection.Open();

            ///Filling the data table
            dataAdapter.Fill(dataTable);

            ///Closing the connection
            connection.Close();

            ///Returning the data table
            return dataTable;
        }


        #endregion
    }

    #endregion
}

#endregion