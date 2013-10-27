#region "Usings"

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

#endregion

#region "Implementation"

namespace Core.Web.Utils
{
    #region "Public Class"

    public class SqlServer
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
            return GetDataTable(sqlClause, ConfigurationManager.ConnectionStrings["SQLSERVER"].ConnectionString);
        }
        public static DataTable GetDataTable(string sqlClause, string connectionString)
        {
            ///Returning the data table
            return GetDataTable(sqlClause, connectionString, null);
        }
        public static DataTable GetDataTable(string sqlClause, List<SqlServer> parameters)
        {
            ///Returning the data table
            return GetDataTable(sqlClause, ConfigurationManager.ConnectionStrings["SQLSERVER"].ConnectionString, parameters);
        }
        public static DataTable GetDataTable(string sqlClause, string connectionString, List<SqlServer> parameters)
        {
            ///The return data table
            DataTable dataTable = new DataTable();

            ///Creating the connection
            SqlConnection connection = new SqlConnection(connectionString);

            ///Creating the data adapter
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlClause, connection);

            ///If the parameters are not null
            if (parameters != null)
                ///For each parameter
                foreach (SqlServer parameter in parameters)
                    dataAdapter.SelectCommand.Parameters.AddWithValue(parameter.ParameterName, parameter.ParameterValue);

            ///Openning the connection
            connection.Open();

            ///Filling the data table
            dataAdapter.Fill(dataTable);

            ///Closing the connection
            connection.Close();

            ///Returning the data table
            return dataTable;
        }

        /// <summary>
        /// Inserts the clause with the parameters
        /// </summary>
        /// <param name="sqlClause"></param>
        /// <param name="parameters"></param>
        /// <param name="connectionString"></param>
        public static void Execute(string sqlClause){
            ///Returning the data table
            Execute(sqlClause, null);
        }
        public static void Execute(string sqlClause, List<SqlServer> parameters)
        {
            ///Returning the data table
            Execute(sqlClause, parameters, ConfigurationManager.ConnectionStrings["SQLSERVER"].ConnectionString);
        }
        public static void Execute(string sqlClause, List<SqlServer> parameters, string connectionString)
        {
            ///The return data table
            DataTable dataTable = new DataTable();

            ///Creating the connection
            SqlConnection connection = new SqlConnection(connectionString);

            ///Creating command
            SqlCommand command = new SqlCommand(sqlClause, connection);

            ///Addinf the parameters
            if (parameters != null)
                foreach (SqlServer parameter in parameters)
                    command.Parameters.AddWithValue(parameter.ParameterName, parameter.ParameterValue == null ? DBNull.Value : parameter.ParameterValue);

            ///Openning the connection
            connection.Open();

            ///Executing the command
            command.ExecuteNonQuery();

            ///Closing the connection
            connection.Close();
        }

        #endregion
    }

    #endregion
}

#endregion