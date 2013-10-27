#region "Usings"

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Core.DataObjects;
using Core.Web.Utils;
using System.Globalization;

#endregion

#region "Implementation"

namespace Core.BusinessLogic
{
    #region "Public Class"

    [DataContract]
    public class Cliente
    {
        #region "Internal Web Attributes"

        [DataMember]
        internal double Latitude { get; set; }
        [DataMember]
        internal double Longitude { get; set; }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// Gets the client by the phone number
        /// </summary>
        /// <param name="phoneNumber">The phone number</param>
        /// <returns>The client with the lat and long</returns>
        public Cliente GetByPhoneNumber(string phoneNumber)
        {
            ///The sql clause
            string sqlClause = @"SELECT LAT, LONG FROM CLIENTES_FWT where MSISDN = @MSISDN";

            ///The paraeter list
            List<SqlServer> parameters = new List<SqlServer>();
            parameters.Add(new SqlServer() { ParameterName = "MSISDN", ParameterValue = phoneNumber.Trim().Replace("(",string.Empty).Replace(")", string.Empty).Replace("-", string.Empty)});

            ///Getting the data table
            DataTable dataTable = SqlServer.GetDataTable(sqlClause, parameters);

            ///The list of clients
            List<Cliente> erbs = new List<Cliente>();

            ///For each row add the client
            foreach (DataRow row in dataTable.Rows)
                erbs.Add(new Cliente()
                {
                    Latitude = !Utils.VerifyEmpty(row["LAT"]) ? double.Parse(row["LAT"].ToString(), CultureInfo.InvariantCulture.NumberFormat) : 0,
                    Longitude = !Utils.VerifyEmpty(row["LONG"]) ? double.Parse(row["LONG"].ToString(), CultureInfo.InvariantCulture.NumberFormat) : 0
                });

            ///Returning
            return erbs.Count > 0 ? erbs[0] : null;
        }

        #endregion
    }

    #endregion
}

#endregion