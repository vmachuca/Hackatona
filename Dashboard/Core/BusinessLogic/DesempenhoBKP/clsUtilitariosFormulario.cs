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

/// <summary>
/// Summary description for UtilitariosFormulario
/// </summary>
public class clsUtilitariosFormulario
{
    public clsUtilitariosFormulario()
    {

    }

    public void PreencherDropDownList(OracleDataReader pdr, DropDownList pList, String pTextValue)
    {
        pList.Items.Clear();
        Int32 contador = 0;
        while (pdr.Read())
        {
            pList.Items.Add(new ListItem(pdr[pTextValue].ToString()));
            contador++;
        }

        pList.Items.Insert(0, new ListItem("TODAS"));

    }

    public void PreencherDropDownListFromSQLServer(DataTable pDt, DropDownList pList, String pTextValue)
    {
        pList.Items.Clear();
        Int32 contador = 0;

        if (pDt != null)
        {
            foreach (DataRow row in pDt.Rows)
            {
                pList.Items.Add(new ListItem(row[pTextValue].ToString()));
                contador++;
            }
        }
        pList.Items.Insert(0, new ListItem("TODAS"));
    }
}
