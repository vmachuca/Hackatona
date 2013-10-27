using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (bool.Parse(ConfigurationManager.AppSettings["PRODUCAO"].ToString()))
            {
                string result = string.Empty;
                bool validou = false;

                if (Request.QueryString["k"] != null)
                {
                    string token = Request.QueryString["k"].ToString() ?? "1";
                    validou = ValidarAcessoToken(token, "sig-di");
                }

                if (!validou)
                    Response.Redirect("login.htm");
            }
        }
    }

    private bool ValidarAcessoToken(string pToken, string pSistema)
    {
        try
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(pToken);
            if (cookie != null)
            {
                string ip = HttpContext.Current.Request.UserHostAddress;
                string ipToken = cookie["valor"].Split('|').GetValue(0).ToString();
                string idSistemaToken = cookie["valor"].Split('|').GetValue(1).ToString();

                return (ip == ipToken) && (pSistema == idSistemaToken);
            }
        }
        catch (Exception)
        {
            return false;
        }

        return false;
    }
}