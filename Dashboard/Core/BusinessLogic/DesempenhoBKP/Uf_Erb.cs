using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

/// <summary>
/// Summary description for Uf_Erb
/// </summary>
public class Uf_Erb
{
    private string _Uf;
    private string _Erb;

    public string Uf
    {
        get { return _Uf; }
        set { _Uf = value; }
    }

    public string Erb
    {
        get { return _Erb; }
        set { _Erb = value; }
    }

    public Uf_Erb() { }

    public Uf_Erb(string uf, string erb)
    {
        Uf = uf;
        Erb = erb;
    }

    public static Boolean ExisteErb(List<Uf_Erb> ufErb, string uf, string erb)
    {
        if (ufErb == null || ufErb.Count.Equals(0)) return false;

        foreach (Uf_Erb item in ufErb)
        {
            if (item.Uf.ToLower().Equals(uf.ToLower()) && item.Erb.ToLower().Equals(erb.ToLower()))
            {
                return true;
            }
        }
        return false;
    }
}
