using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;

/// <summary>
/// Summary description for clsMetricas
/// </summary>
public class clsMetricas
{
    public clsMetricas() { }

    public void TratarMetricas(String pIdMetricaSelecionada, String pNomeMetricaSelecionada, ref String[] pUnidadeMetricas, ref String pGrupoMetricas, ref DataTable pDadosMetricas, ref Int32 pQtdeMetricas, ref String[] pIdMetricas, ref String[] pNomeMetricas, String pElemento)
    {

        string cultura = (IdiomaUtil.SiteEspanhol() ? "_ES" : "");
        try
        {
            //>>>> Recebe as informações da Métrica selecionada
            SqlConnection conGISDB = new SqlConnection();
            conGISDB.ConnectionString = ConfigurationManager.ConnectionStrings["SQLSERVER"].ConnectionString; ;
            conGISDB.Open();

            //>>> DADOS DOS GRUPOS DA METRICA
            SqlCommand cmdGISDB_grupo = new SqlCommand();
            cmdGISDB_grupo.Connection = conGISDB;
            cmdGISDB_grupo.CommandType = CommandType.Text;

            cmdGISDB_grupo.CommandText = "SELECT distinct qryDadosMetrica.ID_METRICA, qryDadosMetrica.NOME_METRICA, qryDadosMetrica.UNIDADE" + cultura +
                                        " FROM Metricas_Principal " +
                                        "LEFT JOIN Metricas_Grupo " +
                                        "ON Metricas_Principal.Grupo = Metricas_Grupo.Grupo " +
                                        "LEFT JOIN Metricas_Principal as qryDadosMetrica " +
                                        "ON qryDadosMetrica.id_Metrica = ISNULL(Metricas_Grupo.id_Metrica, Metricas_Principal.id_Metrica) " +
                                        "WHERE Metricas_Principal.id_Metrica = '" + pIdMetricaSelecionada.ToString() + "' AND Metricas_Principal.ELEMENTO='" + pElemento + "'ORDER BY NOME_METRICA";

            SqlDataReader drGISDB_grupo = cmdGISDB_grupo.ExecuteReader();

            //> Verificar se a metrica possui grupo
            if (drGISDB_grupo.HasRows)
            {
                while (drGISDB_grupo.Read())
                {
                    pIdMetricas[pQtdeMetricas] = drGISDB_grupo["ID_METRICA"].ToString().Trim();
                    pNomeMetricas[pQtdeMetricas] = drGISDB_grupo["NOME_METRICA"].ToString().Trim();
                    pGrupoMetricas = pGrupoMetricas + ", " + drGISDB_grupo["ID_METRICA"].ToString().Trim();
                    pUnidadeMetricas[pQtdeMetricas] = drGISDB_grupo["UNIDADE" + cultura].ToString().Trim();
                    pQtdeMetricas++;
                }
                pGrupoMetricas = pGrupoMetricas.Substring(1, pGrupoMetricas.Length - 1);
            }
            else
            {
                pIdMetricas[0] = pIdMetricaSelecionada;
                pNomeMetricas[0] = pNomeMetricaSelecionada;
                pGrupoMetricas = pIdMetricaSelecionada;
                pQtdeMetricas = 1;
            }

            cmdGISDB_grupo.Dispose();
            drGISDB_grupo.Close();

            //>>> DADOS PRINCIPAIS DA METRICA
            SqlCommand cmdGISDB = new SqlCommand();
            cmdGISDB.Connection = conGISDB;
            cmdGISDB.CommandType = CommandType.Text;
            cmdGISDB.CommandText = "SELECT ORDEM, ABSOLUTO, RANG_00, RANG_01, RANG_02, RANG_03, RANG_04, RANG_05, UNIDADE" + cultura + " as UNIDADE FROM DBO.METRICAS_PRINCIPAL where ELEMENTO = '" + pElemento + "' and ID_METRICA = '" + pIdMetricaSelecionada.ToString() + "'";
            //ATIVA = 'SIM'
            SqlDataReader drGISDB = cmdGISDB.ExecuteReader(CommandBehavior.CloseConnection);

            pDadosMetricas.TableName = "dados_metrica";
            pDadosMetricas.Columns.Add("ORDEM");
            pDadosMetricas.Columns.Add("ABSOLUTO");
            pDadosMetricas.Columns.Add("RANG_00");
            pDadosMetricas.Columns.Add("RANG_01");
            pDadosMetricas.Columns.Add("RANG_02");
            pDadosMetricas.Columns.Add("RANG_03");
            pDadosMetricas.Columns.Add("RANG_04");
            pDadosMetricas.Columns.Add("RANG_05");
            pDadosMetricas.Columns.Add("UNIDADE");

            while (drGISDB.Read())
            {
                DataRow drTemp = pDadosMetricas.NewRow();
                for (int j = 0; j < pDadosMetricas.Columns.Count; j++)
                {
                    drTemp[j] = drGISDB[j];
                }

                pDadosMetricas.Rows.Add(drTemp);
            }
        }
        catch
        {
        }
    }
}
