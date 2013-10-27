using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;

/// <summary>
/// Summary description for Tratados
/// </summary>
public class Tratados
{
    private String CAMPO_DATA_VIEW;
    private String USUARIO_ORACLE;
    private String VIEW_ERB;
    private String VIEW_SETOR;

    public Tratados()
    {
        CAMPO_DATA_VIEW = System.Configuration.ConfigurationManager.AppSettings.Get("CAMPO_DATA_VIEW");
        USUARIO_ORACLE = System.Configuration.ConfigurationManager.AppSettings.Get("USUARIO_ORACLE");
        VIEW_ERB = System.Configuration.ConfigurationManager.AppSettings.Get("VIEW_ERB");
        VIEW_SETOR = System.Configuration.ConfigurationManager.AppSettings.Get("VIEW_SETOR");
    }

    public static string TrataData(string data, bool adicionarNomeColuna)
    {
        return TrataData(data, adicionarNomeColuna, null, true);
    }

    public static string TrataData(string data, bool adicionarNomeColuna, bool exibirHora)
    {
        return TrataData(data, adicionarNomeColuna, null, exibirHora);
    }

    public static string TrataData(string data, bool adicionarNomeColuna, string formatoData)
    {
        return TrataData(data, adicionarNomeColuna, null, true);
    }

    public static string TrataData(string data, bool adicionarNomeColuna, string formatoData, bool exibirHora)
    {
        string nomeCampoData = System.Configuration.ConfigurationManager.AppSettings.Get("CAMPO_DATA_VIEW");
        string usuarioPadraoOracle = System.Configuration.ConfigurationManager.AppSettings.Get("USUARIO_PADRAO_ORACLE");

        if (usuarioPadraoOracle.Equals(UsuarioOracle.DBN1.ToString()))
        {
            return string.Format("{0} = '{1}'", (adicionarNomeColuna ? nomeCampoData : nomeCampoData + "="), data);
        }
        else if (usuarioPadraoOracle.Equals(UsuarioOracle.BLRODRIGUES.ToString()))
        {
            string formatData = (string.IsNullOrEmpty(formatoData) ? "YYYY-MM-DD" + (exibirHora ? " HH24" : "") : formatoData);
            return string.Format("{0} = TO_DATE('{1}', '{2}')", (adicionarNomeColuna ? nomeCampoData : nomeCampoData + "="), data, formatData);
        }

        return string.Empty;
    }

    //public static string TrataData(string data, bool adicionarNomeColuna, string formatoData)
    //{
    //    string formatData = (string.IsNullOrEmpty(formatoData) ? "YYYY-MM-DD HH24" : formatoData);
    //    return string.Format((adicionarNomeColuna ? "DATA_HORA=" : "") + "TO_DATE('{0}', '{1}')", data, formatData);
    //}

    public static string TrataInstrucaoSQL(View view, string campos, string complementos)
    {
        string vw_setor = System.Configuration.ConfigurationManager.AppSettings.Get("VW_SETOR");
        string vw_setor_hmm = System.Configuration.ConfigurationManager.AppSettings.Get("VW_SETOR_HMM");

        string vw_erb = System.Configuration.ConfigurationManager.AppSettings.Get("VW_ERB");
        string vw_erb_hmm = System.Configuration.ConfigurationManager.AppSettings.Get("VW_ERB_HMM");

        string usuarioPadraoOracle = System.Configuration.ConfigurationManager.AppSettings.Get("USUARIO_PADRAO_ORACLE");

        string nomeViewSelecionada = String.Empty;

        if (view == View.VW_ERB)
            nomeViewSelecionada = vw_erb;

        else if (view == View.VW_ERB_HMM)
            nomeViewSelecionada = vw_erb_hmm;

        else if (view == View.VW_SETOR)
            nomeViewSelecionada = vw_setor;

        else if (view == View.VW_SETOR_HMM)
            nomeViewSelecionada = vw_setor_hmm;

        return string.Format("SELECT {0} FROM {1}.{2} {3}", campos, usuarioPadraoOracle, nomeViewSelecionada, complementos);
    }

    public static DateTime TrataFormatoData(String pData, String pHora)
    {
        DateTime dataAuxiliar = default(DateTime);
        DateTime.TryParseExact(string.Format("{0} {1}", pData, pHora), "dd/MM/yyyy HH", CultureInfo.CurrentCulture, DateTimeStyles.None, out dataAuxiliar);
        return dataAuxiliar;
    }

}

public enum View { VW_ERB, VW_ERB_HMM, VW_SETOR, VW_SETOR_HMM }
public enum UsuarioOracle { BLRODRIGUES, DBN1 }