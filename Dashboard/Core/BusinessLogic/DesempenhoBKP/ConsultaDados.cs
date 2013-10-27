using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Data.Common;


/// <summary>
/// Summary description for ConsultaDados
/// </summary>
public class ConsultaDados
{
    public ConsultaDados()
    {

    }

    public static DataTable ConsultaSQLServer(String pSQL, String pTableName, String pListaParseDouble)
    {
        try
        {
            SqlConnection conexao = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLSERVER_GILEADE"].ConnectionString);

            SqlCommand command = new SqlCommand();
            command.Connection = conexao;
            command.CommandType = CommandType.Text;
            command.CommandText = pSQL;
            conexao.Open();

            SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection);
            DataTable DT = DataReaderTODataTable(dr, pTableName, pListaParseDouble);

            dr.Dispose();
            command.Dispose();
            conexao.Close();
            conexao.Dispose();

            return DT;
        }
        catch { }

        return null;
    }

    public static DataTable ConsultaOracle(String pSQL, String pTableName, String pListaParseDouble)
    {
        OracleConnection ORACLEConnection = default(OracleConnection);
        String sConnString = ConfigurationManager.ConnectionStrings["ORACLE"].ConnectionString;
        ORACLEConnection = new OracleConnection(sConnString);

        //Conexão com Oracle - Obtem os dados
        ORACLEConnection.Open();

        OracleCommand mycommandO = new OracleCommand();
        mycommandO.Connection = ORACLEConnection;
        mycommandO.CommandTimeout = 200;
        mycommandO.CommandType = CommandType.Text;

        mycommandO.CommandText = String.Format(pSQL);

        OracleDataReader dr = mycommandO.ExecuteReader(CommandBehavior.CloseConnection);
        DataTable DT = DataReaderTODataTable(dr, pTableName, pListaParseDouble);

        dr.Dispose();
        mycommandO.Dispose();
        ORACLEConnection.Dispose();

        return DT;
    }

    public static DataTable DataReaderTODataTable(DbDataReader dr, String dataTableName, String pListaParseDouble)
    {
        DataTable DT = new DataTable(dataTableName);
        int i = 0;

        for (i = 0; i <= dr.FieldCount - 1; i++)
        {
            DT.Columns.Add(dr.GetName(i));

            if (pListaParseDouble.Contains(DT.Columns[i].ColumnName.ToString()))
                DT.Columns[i].DataType = System.Type.GetType("System.Double");
        }

        int j = 0;
        while (dr.Read())
        {
            DataRow drTemp = null;
            drTemp = DT.NewRow();

            for (j = 0; j <= DT.Columns.Count - 1; j++)
            {
                drTemp[j] = dr[j];
            }
            DT.Rows.Add(drTemp);
        }

        return DT;
    }


}
