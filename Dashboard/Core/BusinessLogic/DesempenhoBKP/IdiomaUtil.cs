using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;
using System.Globalization;

/// <summary>
/// Summary description for IdiomaUtil
/// </summary>
public class IdiomaUtil
{
    public const String LANG_PORTUGUES = "pt-BR";
    public const String LANG_ESPANHOL = "es-ES";

    public static String SeparadorDouble = String.Empty;
    public static String Replace_Double = String.Empty;

    public IdiomaUtil()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static bool SiteEspanhol()
    {
        return Thread.CurrentThread.CurrentCulture.ToString() == LANG_ESPANHOL;
    }

    public static void InitializeCulture(string selectedLanguage)
    {
        switch (selectedLanguage)
        {
            case LANG_ESPANHOL:
                selectedLanguage = IdiomaUtil.LANG_ESPANHOL;
                break;

            default:
                selectedLanguage = IdiomaUtil.LANG_PORTUGUES;
                break;
        }

        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(selectedLanguage);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguage);


    }
}
