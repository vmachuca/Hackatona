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
/// Summary description for clsControleDadosFiltro
/// </summary>
public class clsControleDadosFiltroFormulario
{
    #region Atributos

    private String elemento;
    private String valorMetrica;
    private String descricaoMetrica;
    private String uf;
    private String municipio;
    private String tecnologia;
    private String ccc;
    private String bsc;
    private String erb;
    private DateTime data;
    private Boolean hmm;
    private Boolean zoom;

    private bool isFalhas;

    private List<Uf_Erb> _UF_ERB;

    #endregion

    #region Propriedades

    public List<Uf_Erb> UF_ERB
    {
        get { return _UF_ERB; }
        set { _UF_ERB = value; }
    }

    public String Elemento
    {
        get { return elemento; }
        set { elemento = value; }
    }

    public String ValorMetrica
    {
        get { return valorMetrica; }
        set { valorMetrica = value; }
    }

    public String DescricaoMetrica
    {
        get { return descricaoMetrica; }
        set { descricaoMetrica = value; }
    }

    public String Uf
    {
        get { return uf; }
        set { uf = value; }
    }

    public String Municipio
    {
        get { return municipio; }
        set { municipio = value; }
    }

    public String Tecnologia
    {
        get { return tecnologia; }
        set { tecnologia = value; }
    }

    public String CCC
    {
        get { return ccc; }
        set { ccc = value; }
    }

    public String BSC
    {
        get { return bsc; }
        set { bsc = value; }
    }

    public String ERB
    {
        get { return erb; }
        set { erb = value; }
    }

    public DateTime Data
    {
        get { return data; }
        set { data = value; }
    }

    public Boolean HMM
    {
        get { return hmm; }
        set { hmm = value; }
    }

    public Boolean Zoom
    {
        get { return zoom; }
        set { zoom = value; }
    }

    public clsControleDadosFiltroFormulario DadosSessao
    {
        get { return HttpContext.Current.Session["ControleDadosFiltroFormulario"] as clsControleDadosFiltroFormulario; }
        set { HttpContext.Current.Session["ControleDadosFiltroFormulario"] = value; }
    }

    public bool IsFalhas
    {
        get { return isFalhas; }
        set { isFalhas = value; }
    }

    #endregion

    #region Contrutor

    public clsControleDadosFiltroFormulario() { }

    #endregion

    #region Métodos
    public Boolean MultiplasERBs()
    {
        if (UF_ERB != null)
            return UF_ERB.Count > 0;

        return false;
    }
    #endregion
}


