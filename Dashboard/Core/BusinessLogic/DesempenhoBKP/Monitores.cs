using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Monitore
/// </summary>
public static class Monitores
{
    public static void Log(int pModulo, int pFuncao)
    {
        string ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
    }
}