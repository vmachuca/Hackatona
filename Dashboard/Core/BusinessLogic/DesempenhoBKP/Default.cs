using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Data.OracleClient;

/////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////
///
/// ANOTAÇÃO:
/// 
/// BTS : ERB EM INGLES
/// BSC : GRUPO DE ERBS
/// CCC : GRUPO DE BSC
/// 
/// HMM : MÉDIA DE 1 DIA
///
/////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////

namespace Core.BusinessLogic.Desempenho
{
    public enum TipoConsulta
    {
        Erb,
        Setor,
        Cobertura
    }

    public class Default
    {
        #region "Private Variables"

        private DropDownList DDL_metrica = new DropDownList();
        private DropDownList DDL_munic= new DropDownList();
        private DropDownList DDL_tecno = new DropDownList();
        private DropDownList DDL_CCC = new DropDownList();
        private DropDownList DDL_bsc = new DropDownList();
        private DropDownList DDL_uf = new DropDownList();

        private TextBox txtERB = new TextBox();
        private CheckBox HF_CHKBOX_HMM = new CheckBox();

        private string DDL_hora = DateTime.Now.ToString("hh");
        private string calendarioctl00 = DateTime.Now.ToString("dd/MM/yyyy");

        TipoConsulta radiobutton_erb = TipoConsulta.Erb;

        #endregion

        private void fillDDL_metrica()
        {
            string elemento = null;
            if (radiobutton_erb.Equals(TipoConsulta.Erb))
            {
                elemento = "BTS";
            }
            else if (radiobutton_erb.Equals(TipoConsulta.Setor))
            {
                elemento = "SETOR";
            }

            string cultura = (IdiomaUtil.SiteEspanhol() ? "_ES" : "");

            SqlConnection conGISDB = new SqlConnection();
            conGISDB.ConnectionString = ConfigurationManager.ConnectionStrings["SQLSERVER"].ConnectionString;

            string producao = (ConfigurationManager.AppSettings["PRODUCAO"].Equals("SIM") ? "ATIVA='SIM' AND PRODUCAO=1 AND" : "");

            SqlCommand cmdGISDB = new SqlCommand();
            cmdGISDB.Connection = conGISDB;
            cmdGISDB.CommandType = CommandType.Text;
            cmdGISDB.CommandText = string.Format("SELECT DISTINCT NOME_METRICA{0}, ID_METRICA FROM DBO.METRICAS_PRINCIPAL where {2} ELEMENTO = '{1}' ORDER BY NOME_METRICA{0} ASC", cultura, elemento, producao);
            //Comentado para aparecer todas a métricas que foram carregadas para teste
            //cmdGISDB.CommandText = "SELECT DISTINCT NOME_METRICA, ID_METRICA FROM DBO.METRICAS_PRINCIPAL where ATIVA = 'SIM' and ELEMENTO = '" & elemento & "' ORDER BY NOME_METRICA ASC"
            conGISDB.Open();

            SqlDataReader drGISDB = default(SqlDataReader);
            drGISDB = cmdGISDB.ExecuteReader(CommandBehavior.CloseConnection);

            this.DDL_metrica.DataSource = drGISDB;
            this.DDL_metrica.DataTextField = "NOME_METRICA" + cultura;
            this.DDL_metrica.DataValueField = "ID_METRICA";
            this.DDL_metrica.DataBind();

            //Me.DDL_metrica.Items.Insert(0, New ListItem(""))

            cmdGISDB.Dispose();
            conGISDB.Dispose();

        }
        private void fillDDL_CCC2()
        {
            string sql = string.Format("SELECT DISTINCT CENTRAL FROM ELEMENTOS_REDE_SIGD WHERE UF='{0}' AND TECNOLOGIA='{1}' ORDER BY CENTRAL", this.DDL_uf.Text, this.DDL_tecno.Text);

            DataTable dt = ConsultaDados.ConsultaSQLServer(sql, "ELEMENTOS_REDE_SIGD", "");

            clsUtilitariosFormulario utilForm = new clsUtilitariosFormulario();
            utilForm.PreencherDropDownListFromSQLServer(dt, DDL_CCC, "CENTRAL");

            fillDDL_BSC2();

            //fillDDL_ERB()
            txtERB.Text = "TODAS";
        }
        private void fillDDL_CCC()
        {

            DateTime maxDate;

            //> Lembrar que estamos utilizando calendario.MaxDate porque o request() não funciona no pageload. 
            string data_filtro = null;
            if (System.DateTime.Now.Hour < 7)
            {
                maxDate = System.DateTime.Today.AddDays(-1);
            }
            else
            {
                maxDate = System.DateTime.Today;
            }
            //data_filtro = this.calendario.MaxDate.Year + "-" + String.Right(("0" + this.calendario.MaxDate.Month), 2) + "-" + String.Right(("0" + this.calendario.MaxDate.Day), 2) + " 01";
            data_filtro = string.Format("{0}-{1}-{2} 01", maxDate.Year, maxDate.Month.ToString().PadLeft(2, '0'), maxDate.Day.ToString().PadLeft(2, '0'));

            //'Resgata a Data selecionada no calendário
            //Dim req As String
            //req = Request("calendario$ctl00")

            //'> Monta a data para ser utilizada no filtro
            //Dim data_filtro As String
            //data_filtro = req.Substring(6, 4) & "-" & req.Substring(3, 2) & "-" & req.Substring(0, 2) & " " & Request("DDL_hora") 'Me.DDL_hora.SelectedValue

            //Dim TEC As String
            //If Me.DDL_tecno.SelectedValue = "GPRS" Or Me.DDL_tecno.SelectedValue = "EDGE" Then
            //    TEC = "GSM"
            //Else
            //    TEC = Me.DDL_tecno.SelectedValue
            //End If

            OracleConnection ORACLEConnection = default(OracleConnection);
            string sConnString = ConfigurationManager.ConnectionStrings["ORACLE"].ConnectionString;
            ORACLEConnection = new OracleConnection(sConnString);

            try
            {
                OracleDataReader dr = default(OracleDataReader);
                OracleCommand mycommandO = new OracleCommand();
                mycommandO.Connection = ORACLEConnection;
                mycommandO.CommandTimeout = 200;
                mycommandO.CommandType = CommandType.Text;
                mycommandO.CommandText = "select distinct central from dbn1.vw09_arcgis where DATA = '" + data_filtro + "' AND UF='" + this.DDL_uf.SelectedValue + "' AND tecnologia='" + this.DDL_tecno.SelectedValue + "' order by central";
                ORACLEConnection.Open();

                dr = mycommandO.ExecuteReader(CommandBehavior.CloseConnection);

                clsUtilitariosFormulario utilForm = new clsUtilitariosFormulario();
                utilForm.PreencherDropDownList(dr, DDL_CCC, "CENTRAL");

                //DDL_CCC.DataSource = dr
                //DDL_CCC.DataTextField = "CENTRAL"
                //DDL_CCC.DataValueField = "CENTRAL"
                //DDL_CCC.DataBind()

                mycommandO.Dispose();
                ORACLEConnection.Dispose();

                //If Me.DDL_CCC.Items.Count <> 0 Then
                //    Me.DDL_CCC.Items.Add(Me.DDL_CCC.Items.Count)
                //    Me.DDL_CCC.Items(Me.DDL_CCC.Items.Count - 1).Text = "TODAS"
                //    Me.DDL_CCC.Items(Me.DDL_CCC.Items.Count - 1).Value = "TODAS"
                //    Me.DDL_CCC.Items(Me.DDL_CCC.Items.Count - 1).Selected = True
                //    Me.DDL_CCC.Items(Me.DDL_CCC.Items.Count - 1).Enabled = True
                //End If

                fillDDL_BSC2();

            }
            catch
            {
                ///
                throw new Exception("Exceção");
            }
            finally
            {
                ORACLEConnection.Dispose();
            }

        }
        private void fillDDL_MUNIC()
        {
            SqlConnection conGISDB = new SqlConnection();
            conGISDB.ConnectionString = "Server=10.126.111.203; Database=GISDB;uid=sde;pwd=sde";

            SqlCommand cmdGISDB = new SqlCommand();
            cmdGISDB.Connection = conGISDB;
            cmdGISDB.CommandType = CommandType.Text;
            cmdGISDB.CommandText = "SELECT DISTINCT NOME_MUNIC FROM SDE.A_0105_PAIS_MUNIC_BRASIL WHERE UF = '" + this.DDL_uf.Text + "' ORDER BY NOME_MUNIC";
            conGISDB.Open();

            SqlDataReader drGISDB = default(SqlDataReader);
            drGISDB = cmdGISDB.ExecuteReader(CommandBehavior.CloseConnection);

            DDL_munic.DataSource = drGISDB;
            DDL_munic.DataTextField = "NOME_MUNIC";
            DDL_munic.DataValueField = "NOME_MUNIC";
            DDL_munic.DataBind();

            cmdGISDB.Dispose();
            conGISDB.Dispose();

            if (this.DDL_munic.Items.Count != 0)
            {
                this.DDL_munic.Items.Add(this.DDL_munic.Items.Count.ToString());
                this.DDL_munic.Items[this.DDL_munic.Items.Count - 1].Text = "TODOS";
                this.DDL_munic.Items[this.DDL_munic.Items.Count - 1].Value = "TODOS";
                this.DDL_munic.Items[this.DDL_munic.Items.Count - 1].Selected = true;
                this.DDL_munic.Items[this.DDL_munic.Items.Count - 1].Enabled = true;
            }

        }
        private void fillDDL_BSC2()
        {
            string sql = "";
            if (this.DDL_CCC.SelectedValue != "TODAS")
            {
                sql = string.Format("SELECT DISTINCT BSC FROM ELEMENTOS_REDE_SIGD WHERE CENTRAL='{0}' ORDER BY BSC", this.DDL_CCC.Text);
            }
            else
            {
                sql = string.Format("SELECT DISTINCT BSC FROM ELEMENTOS_REDE_SIGD WHERE UF='{0}' AND TECNOLOGIA='{1}' ORDER BY BSC", this.DDL_uf.Text, this.DDL_tecno.Text);
            }

            DataTable dt = ConsultaDados.ConsultaSQLServer(sql, "ELEMENTOS_REDE_SIGD", "");

            clsUtilitariosFormulario utilForm = new clsUtilitariosFormulario();
            utilForm.PreencherDropDownListFromSQLServer(dt, DDL_bsc, "BSC");

            //fillDDL_ERB()
            txtERB.Text = "TODAS";
        }
        private void fillDDL_BSC()
        {
            DateTime maxDate;

            //> Lembrar que estamos utilizando calendario.MaxDate porque o request() não funciona no pageload.
            string data_filtro = null;
            if (System.DateTime.Now.Hour < 7)
            {
                maxDate = System.DateTime.Today.AddDays(-1);
            }
            else
            {
                maxDate = System.DateTime.Today;
            }
            //data_filtro = this.calendario.MaxDate.Year + "-" + String.Right(("0" + this.calendario.MaxDate.Month), 2) + "-" + String.Right(("0" + this.calendario.MaxDate.Day), 2) + " 01";
            data_filtro = string.Format("{0}-{1}-{2} 01", maxDate.Year, maxDate.Month.ToString().PadLeft(2, '0'), maxDate.Day.ToString().PadLeft(2, '0'));

            //'Resgata a Data selecionada no calendário
            //Dim req As String
            //req = Request("calendario$ctl00")

            //'> Monta a data para ser utilizada no filtro
            //Dim data_filtro As String
            //data_filtro = req.Substring(6, 4) & "-" & req.Substring(3, 2) & "-" & req.Substring(0, 2) & " " & Request("DDL_hora") 'Me.DDL_hora.SelectedValue

            //Dim TEC As String
            //If Me.DDL_tecno.SelectedValue = "GPRS" Or Me.DDL_tecno.SelectedValue = "EDGE" Then
            //    TEC = "GSM"
            //Else
            //    TEC = Me.DDL_tecno.SelectedValue
            //End If

            OracleConnection ORACLEConnection = default(OracleConnection);
            string sConnString = ConfigurationManager.ConnectionStrings["ORACLE"].ConnectionString;
            ORACLEConnection = new OracleConnection(sConnString);

            OracleDataReader dr = default(OracleDataReader);
            OracleCommand mycommandO = new OracleCommand();
            mycommandO.Connection = ORACLEConnection;
            mycommandO.CommandTimeout = 200;
            mycommandO.CommandType = CommandType.Text;

            //VvVvVvV NOVAS_METRICAS (modificar aqui) VvVvVvV

            if (this.DDL_CCC.SelectedValue != "TODAS")
            {
                //mycommandO.CommandText = "SELECT DISTINCT BSC FROM DBN1.VW09_ARCGIS WHERE CENTRAL = '" & Me.DDL_CCC.Text & "' AND DATA='" & data_filtro & "' ORDER BY BSC"
                mycommandO.CommandText = Tratados.TrataInstrucaoSQL(View.VW_ERB, "DISTINCT BSC", string.Format("WHERE {0} AND CENTRAL='{1}' ORDER BY BSC", Tratados.TrataData(data_filtro, true), this.DDL_CCC.Text));
            }
            else
            {
                //mycommandO.CommandText = "SELECT DISTINCT BSC FROM DBN1.VW09_ARCGIS WHERE UF = '" & Me.DDL_uf.Text & "' AND TECNOLOGIA='" & Me.DDL_tecno.SelectedValue & "' AND DATA='" & data_filtro & "' ORDER BY BSC"
                mycommandO.CommandText = Tratados.TrataInstrucaoSQL(View.VW_ERB, "DISTINCT BSC", string.Format("WHERE {0} AND UF='{1}' AND TECNOLOGIA='{2}' ORDER BY BSC", Tratados.TrataData(data_filtro, true), this.DDL_uf.Text, this.DDL_tecno.SelectedValue));
            }

            ORACLEConnection.Open();

            dr = mycommandO.ExecuteReader(CommandBehavior.CloseConnection);

            clsUtilitariosFormulario utilForm = new clsUtilitariosFormulario();
            utilForm.PreencherDropDownList(dr, DDL_bsc, "BSC");

            //DDL_bsc.DataSource = dr
            //DDL_bsc.DataTextField = "BSC"
            //DDL_bsc.DataValueField = "BSC"
            //DDL_bsc.DataBind()

            mycommandO.Dispose();
            ORACLEConnection.Dispose();

            //If Me.DDL_bsc.Items.Count <> 0 Then
            //    Me.DDL_bsc.Items.Add(Me.DDL_bsc.Items.Count)
            //    Me.DDL_bsc.Items(Me.DDL_bsc.Items.Count - 1).Text = "TODAS"
            //    Me.DDL_bsc.Items(Me.DDL_bsc.Items.Count - 1).Value = "TODAS"
            //    Me.DDL_bsc.Items(Me.DDL_bsc.Items.Count - 1).Selected = True
            //    Me.DDL_bsc.Items(Me.DDL_bsc.Items.Count - 1).Enabled = True
            //End If

            //fillDDL_ERB()
            txtERB.Text = Resource.lbl_Todas;
        }

        private DateTime TrataData(string pData, string separador, Int32 hora)
        {
            string[] aux = null;

            try
            {
                aux = pData.Split(separador.ToCharArray());
                if (aux.Length > 0)
                {
                    return new DateTime(Int32.Parse(aux.GetValue(2).ToString()), Int32.Parse(aux.GetValue(1).ToString()), Int32.Parse(aux.GetValue(0).ToString()), hora, 0, 0);
                }
                else
                {
                    return new DateTime();
                }
            }
            catch
            {
                throw (new Exception("Parametro 'Data' não compatível. O valor deve ser passado dia/mês/ano"));
            }

            return new DateTime();

        }
        private void personaliza()
        {
                SqlConnection conGISDB = new SqlConnection();
                conGISDB.ConnectionString = ConfigurationManager.ConnectionStrings["SQLSERVER"].ConnectionString;

                SqlCommand cmdGISDB = new SqlCommand();
                cmdGISDB.Connection = conGISDB;
                cmdGISDB.CommandType = CommandType.Text;
                cmdGISDB.CommandText = string.Format("SELECT ABSOLUTO, RANG_00, RANG_01, RANG_02, RANG_03, RANG_04, RANG_05, ORDEM from DBO.METRICAS_PRINCIPAL where ID_METRICA = '{0}'", this.DDL_metrica.SelectedValue);
                conGISDB.Open();

                SqlDataReader drGISDB = default(SqlDataReader);
                drGISDB = cmdGISDB.ExecuteReader(CommandBehavior.CloseConnection);

                if (drGISDB.Read())
                {
                    if (drGISDB["ABSOLUTO"].ToString() == "SIM")
                    {
                        // TODO verificar o que fazer com essa variáveis comentadas
                        //this.CBOX_quebra1.Checked = true;
                        //this.TBOX_quebra11.Text = "";
                        //this.TBOX_quebra12.Text = "";
                        //this.DDL_COR1.SelectedIndex = 5;
                        //
                        //this.CBOX_quebra2.Checked = true;
                        //this.TBOX_quebra21.Text = "";
                        //this.TBOX_quebra22.Text = "";
                        //this.DDL_COR2.SelectedIndex = 15;
                        //
                        //this.CBOX_quebra3.Checked = true;
                        //this.TBOX_quebra31.Text = "";
                        //this.TBOX_quebra32.Text = "";
                        //this.DDL_COR3.SelectedIndex = 10;
                        //
                        //this.CBOX_quebra4.Checked = false;
                        //this.CBOX_quebra5.Checked = false;
                        //this.TBOX_quebra41.Text = "";
                        //this.TBOX_quebra42.Text = "";
                        //this.TBOX_quebra51.Text = "";
                        //this.TBOX_quebra52.Text = "";
                        //this.DDL_COR4.SelectedIndex = 0;
                        //this.DDL_COR5.SelectedIndex = 0;
                    }
                    else
                    {
                        if (drGISDB["ORDEM"].ToString() == "ASC")
                        {
                            ////> Range1
                            //this.CBOX_quebra1.Checked = true;
                            //this.TBOX_quebra11.Text = drGISDB["RANG_05"].ToString();
                            //this.TBOX_quebra12.Text = drGISDB["RANG_04"].ToString();
                            //this.DDL_COR1.SelectedIndex = 5;
                            ////Verde
                            ////> Range2
                            //this.CBOX_quebra2.Checked = true;
                            //this.TBOX_quebra21.Text = drGISDB["RANG_03"].ToString();
                            //this.TBOX_quebra22.Text = drGISDB["RANG_02"].ToString();
                            //this.DDL_COR2.SelectedIndex = 15;
                            ////Laranja
                            ////> Range3
                            //this.CBOX_quebra3.Checked = true;
                            //this.TBOX_quebra31.Text = drGISDB["RANG_01"].ToString();
                            //this.TBOX_quebra32.Text = drGISDB["RANG_00"].ToString();
                            //this.DDL_COR3.SelectedIndex = 10;
                            ////Vermelho
                            ////> Limpa o restante
                            //this.CBOX_quebra4.Checked = false;
                            //this.CBOX_quebra5.Checked = false;
                            //this.TBOX_quebra41.Text = "";
                            //this.TBOX_quebra42.Text = "";
                            //this.TBOX_quebra51.Text = "";
                            //this.TBOX_quebra52.Text = "";
                            //this.DDL_COR4.SelectedIndex = 0;
                            //this.DDL_COR5.SelectedIndex = 0;
                        }
                        else
                        {
                            //> Range1
                            //this.CBOX_quebra1.Checked = true;
                            //this.TBOX_quebra11.Text = drGISDB["RANG_00"].ToString();
                            //this.TBOX_quebra12.Text = drGISDB["RANG_01"].ToString();
                            //this.DDL_COR1.SelectedIndex = 10;
                            ////Vermelho
                            ////> Range2
                            //this.CBOX_quebra2.Checked = true;
                            //this.TBOX_quebra21.Text = drGISDB["RANG_02"].ToString();
                            //this.TBOX_quebra22.Text = drGISDB["RANG_03"].ToString();
                            //this.DDL_COR2.SelectedIndex = 15;
                            ////Laranja
                            ////> Range3
                            //this.CBOX_quebra3.Checked = true;
                            //this.TBOX_quebra31.Text = drGISDB["RANG_04"].ToString();
                            //this.TBOX_quebra32.Text = drGISDB["RANG_05"].ToString();
                            //this.DDL_COR3.SelectedIndex = 5;
                            ////Verde
                            ////> Limpa o restante
                            //this.CBOX_quebra4.Checked = false;
                            //this.CBOX_quebra5.Checked = false;
                            //this.TBOX_quebra41.Text = "";
                            //this.TBOX_quebra42.Text = "";
                            //this.TBOX_quebra51.Text = "";
                            //this.TBOX_quebra52.Text = "";
                            //this.DDL_COR4.SelectedIndex = 0;
                            //this.DDL_COR5.SelectedIndex = 0;
                        }
                    }
                }
                cmdGISDB.Dispose();
                conGISDB.Dispose();
        }
        private void prepararanges()
        {
            // TODO verificar o que fazer com essa variáveis
            //this.TBOX_quebra11.Text = this.TBOX_quebra11.Text.Replace(",", ".");
            //this.TBOX_quebra12.Text = this.TBOX_quebra12.Text.Replace(",", ".");
            //this.TBOX_quebra21.Text = this.TBOX_quebra21.Text.Replace(",", ".");
            //this.TBOX_quebra22.Text = this.TBOX_quebra22.Text.Replace(",", ".");
            //this.TBOX_quebra31.Text = this.TBOX_quebra31.Text.Replace(",", ".");
            //this.TBOX_quebra32.Text = this.TBOX_quebra32.Text.Replace(",", ".");
            //this.TBOX_quebra41.Text = this.TBOX_quebra41.Text.Replace(",", ".");
            //this.TBOX_quebra42.Text = this.TBOX_quebra42.Text.Replace(",", ".");
            //this.TBOX_quebra51.Text = this.TBOX_quebra51.Text.Replace(",", ".");
            //this.TBOX_quebra52.Text = this.TBOX_quebra52.Text.Replace(",", ".");
        }
        private void GardaDadosFiltroSessao()
        {
            clsControleDadosFiltroFormulario controleDadosFormulario = new clsControleDadosFiltroFormulario();

            try
            {
                controleDadosFormulario.BSC = this.DDL_bsc.SelectedValue;
                controleDadosFormulario.CCC = this.DDL_CCC.SelectedValue;
                controleDadosFormulario.Data = TrataData(Request["calendario$ctl00"], "/", Int32.Parse(Request["DDL_hora"]));
                controleDadosFormulario.DescricaoMetrica = this.DDL_metrica.SelectedItem.Text;
                controleDadosFormulario.ValorMetrica = this.DDL_metrica.SelectedItem.Value.ToUpper();

                if ((radiobutton_erb.Equals(TipoConsulta.Erb)))
                {
                    controleDadosFormulario.Elemento = "BTS";
                }
                else if ((radiobutton_erb.Equals(TipoConsulta.Setor)))
                {
                    controleDadosFormulario.Elemento = "SETOR";
                }
                else if ((radiobutton_erb.Equals(TipoConsulta.Cobertura)))
                {
                    controleDadosFormulario.Elemento = "COBERTURA";
                }

                controleDadosFormulario.ERB = this.txtERB.Text;
                controleDadosFormulario.HMM = bool.Parse(this.HF_CHKBOX_HMM.Value.ToLower());
                controleDadosFormulario.Municipio = this.DDL_munic.SelectedValue;
                controleDadosFormulario.Tecnologia = this.DDL_tecno.SelectedValue;
                controleDadosFormulario.Uf = this.DDL_uf.SelectedValue;
                controleDadosFormulario.Zoom = false;

                controleDadosFormulario.DadosSessao = controleDadosFormulario;

            }
            catch
            {
            }
        }
        private void GardaDadosFiltroSessao(clsControleDadosFiltroFormulario dadosRF)
        {
            clsControleDadosFiltroFormulario controleDadosFormulario = new clsControleDadosFiltroFormulario();
            try
            {
                controleDadosFormulario.Data = dadosRF.Data;
                controleDadosFormulario.DescricaoMetrica = dadosRF.DescricaoMetrica;
                controleDadosFormulario.ValorMetrica = dadosRF.ValorMetrica.ToUpper();
                controleDadosFormulario.Elemento = dadosRF.Elemento;
                controleDadosFormulario.ERB = dadosRF.ERB;
                controleDadosFormulario.Tecnologia = dadosRF.Tecnologia;
                controleDadosFormulario.Uf = dadosRF.Tecnologia;

                controleDadosFormulario.DadosSessao = controleDadosFormulario;
            }
            catch
            {
            }
        }

        private void Executa(
            clsControleDadosFiltroFormulario dadosFormularioAlerta,
            bool HF_CHKBOX_HMM,
            string selectedDate, 
            TipoConsulta tipoConsulta)
    {
        Monitores.Log(5, 1);

        ///clausula sql
        string sqlClause = string.Empty;

        string grupo_metricas = string.Empty;
        DataTable dados_metrica = new DataTable();
        int qtd_metricas = 0;

        //> Limitado a 20 metricas por grupo
        string[] id_metricas = new string[21];
        string[] nome_metricas = new string[21];
        string[] unidade_metricas = new string[21];

        string elemento = null;
        string sel_erb = null;
        string sel_bsc = null;
        string sel_ccc = null;

        #region Filtros

        if ((dadosFormularioAlerta == null))
        {
            string data_calendario = null;
            data_calendario = selectedDate;

            string data_hoje = string.Empty;
            data_hoje = DateTime.Today.ToString("ddMMyyyy");

            if (HF_CHKBOX_HMM && data_calendario == data_hoje)
            {

                ///Limpa o resultado anterior

                // TODO arrumar MapaUtil e Map1
                //MapaUtil mapaUtil = new MapaUtil(Map1);
                //mapaUtil.ObterMapResource(GisResources.PONTOS).Graphics.Tables.Clear();
            }

            //Resgata a Data selecionada no calendário

            string req = null;
            req = selectedDate;

            //> Prepara os ranges da personalização
            personaliza();
            prepararanges();

            //> Prepara a variável elemento (ERB ou SETOR)

            if (tipoConsulta.Equals(TipoConsulta.Erb))
            {
                elemento = "BTS";
            }
            else
            {
                elemento = "SETOR";
            }

            //guarda os dados filtrados na sessao do usuário
            GardaDadosFiltroSessao();

            //>>>>>>>>>>>>>>>>>   Popula o DATATABLE do ORACLE (VW09_ARCGIS)   >>>>>>>>>>>

            //> Verifica se a tecnologia e define qual campo de sigla adotar
            string campo_sigla = null;
            if (this.DDL_tecno.Text == "GSM" | this.DDL_tecno.Text == "TDMA" | this.DDL_tecno.Text == "GPRS" | this.DDL_tecno.Text == "EDGE" | this.DDL_tecno.Text == "WCDMA")
            {
                campo_sigla = "BTS";
            }
            else
            {
                campo_sigla = "SIGLA";
            }

            //> Verifica se Dropdownlist de CCC, BSC e ERB estão como TODAS ou selecionados.

            //VvVvVvV NOVAS_METRICAS (modificar aqui) VvVvVvV
            //> CCC
            if (this.DDL_CCC.SelectedValue == Resource.lbl_Todas | string.IsNullOrEmpty(this.DDL_CCC.SelectedValue))
            {
                sel_ccc = "CENTRAL <> ' '";
            }
            else
            {
                sel_ccc = "CENTRAL = '" + this.DDL_CCC.SelectedValue.Trim() + "'";
            }

            //> BSC
            if (this.DDL_bsc.SelectedValue == Resource.lbl_Todas | string.IsNullOrEmpty(this.DDL_bsc.SelectedValue))
            {
                sel_bsc = "BSC <> ' '";
            }
            else
            {
                sel_bsc = "BSC = '" + this.DDL_bsc.SelectedValue.Trim() + "'";
            }

            //> ERB
            if (radiobutton_erb.Checked)
            {
                if (this.txtERB.Text == Resource.lbl_Todas | string.IsNullOrEmpty(this.txtERB.Text))
                {
                    sel_erb = "AND " + campo_sigla + " <> ' '";
                }
                else
                {
                    sel_erb = "AND " + campo_sigla + " = '" + this.txtERB.Text.Trim() + "'";
                }
            }

            //> Monta a data para ser utilizada no filtro
            string data_filtro = string.Empty;
            DateTime data = Tratados.TrataFormatoData(req, Request["DDL_hora"]);

            if (this.HF_CHKBOX_HMM.Value != "true")
            {
                data_filtro = Tratados.TrataData(data.ToString("yyyy-MM-dd HH"),true);
                //data_filtro = "DATA = '" + req.Substring(6, 4) + "-" + req.Substring(3, 2) + "-" + req.Substring(0, 2) + " " + Request["DDL_hora"] + "'";
            }
            else
            {
                //data_filtro = Tratados.TrataData(data.ToString("yyyy-MM-dd"), true, false);
                data_filtro = "DIA = '" + req.Substring(6, 4) + "-" + req.Substring(3, 2) + "-" + req.Substring(0, 2) + "'";
            }

            //> Verificar se checkbox_hmm está selecionada através do Hiddenfield HF_CHKBOX_HMM e defini qual view será utilizada
            bool _HMM = (bool.Parse(this.HF_CHKBOX_HMM.Value.ToLower()) == true);
            //If Me.HF_CHKBOX_HMM.Value = "true" Then
            //    _HMM = "_HMM"
            //Else
            //    _HMM = ""
            //End If

            clsMetricas metrica = new clsMetricas();

            //> Prepara os dados da Metrica
            if ((dadosFormularioAlerta != null))
            {
                metrica.TratarMetricas(dadosFormularioAlerta.DescricaoMetrica, dadosFormularioAlerta.DescricaoMetrica, ref unidade_metricas, ref grupo_metricas, ref dados_metrica, ref qtd_metricas, ref id_metricas, ref nome_metricas,
                elemento);
            }
            else
            {
                metrica.TratarMetricas(DDL_metrica.SelectedItem.Value, DDL_metrica.SelectedItem.Text, ref unidade_metricas, ref grupo_metricas, ref dados_metrica, ref qtd_metricas, ref id_metricas, ref nome_metricas,
                elemento);
            }

            //Valida se a métrica selecionada possui dado ou viculo com a base do oracle
            if (dados_metrica.Rows.Count == 0)
            {
                return;
            }
            else
            {
            }

            if ((radiobutton_erb.Equals(TipoConsulta.Setor) || radiobutton_erb.Equals(TipoConsulta.Cobertura)))
            {
                //sql = "Select CHAVE, UF, BTS, LATITUDE_DEC, LONGITUDE_DEC, SETOR, AZIMUTE," & grupo_metricas & " from DBN1.VW11_ARCGIS" & _HMM & " WHERE " & data_filtro & " AND UF='" & Me.DDL_uf.Text & "' AND TECNOLOGIA = '" & Me.DDL_tecno.Text & "' AND CHAVE not like '%*%' AND " & sel_ccc & " AND " & sel_bsc & " AND " & sel_erb & " ORDER BY " & Me.DDL_metrica.SelectedValue & " " & dados_metrica.Rows(0).Item("ORDEM")

                sqlClause = Tratados.TrataInstrucaoSQL((_HMM ? View.VW_SETOR_HMM : View.VW_SETOR), string.Format("CHAVE, UF, BTS, LATITUDE_DEC, LONGITUDE_DEC, SETOR, AZIMUTE,{0}", grupo_metricas), string.Format("WHERE {0} AND UF='{1}' AND TECNOLOGIA='{2}' AND CHAVE not like '%*%' AND {3} AND {4} {5} ORDER BY {6} {7}", data_filtro, this.DDL_uf.Text, this.DDL_tecno.Text, sel_ccc, sel_bsc, sel_erb, this.DDL_metrica.SelectedValue, dados_metrica.Rows[0]["ORDEM"]));

            }
            else if (radiobutton_erb.Equals(TipoConsulta.Erb))
            {
                //sql = "Select CHAVE, UF, BTS, LATITUDE_DEC, LONGITUDE_DEC, " & grupo_metricas & " from DBN1.VW09_ARCGIS" & _HMM & " WHERE " & data_filtro & " AND UF='" & Me.DDL_uf.Text & "' AND TECNOLOGIA = '" & Me.DDL_tecno.Text & "' AND CHAVE not like '%*%' AND " & sel_ccc & " AND " & sel_bsc & " AND " & sel_erb & " ORDER BY " & Me.DDL_metrica.SelectedValue & " " & dados_metrica.Rows(0).Item("ORDEM")

                sqlClause = Tratados.TrataInstrucaoSQL((_HMM ? View.VW_ERB_HMM : View.VW_ERB), string.Format("CHAVE,UF,BTS,LATITUDE_DEC,LONGITUDE_DEC,{0}", grupo_metricas), string.Format("WHERE {0} AND UF='{1}' AND TECNOLOGIA='{2}' AND CHAVE not like '%*%' AND {3} AND {4} {5} ORDER BY {6} {7}", data_filtro, this.DDL_uf.Text, this.DDL_tecno.Text, sel_ccc, sel_bsc, sel_erb, this.DDL_metrica.SelectedValue, dados_metrica.Rows[0]["ORDEM"]));

            }

        }
        //filtros

        #endregion

        //guarda os dados filtrados na sessao do usuário
        GardaDadosFiltroSessao(dadosFormularioAlerta);

        clsMetricas metricas = new clsMetricas();


        if ((dadosFormularioAlerta != null))
        {
            //Integração RF
            AlteraUF(dadosFormularioAlerta.Uf);
            AlteraMetrica(dadosFormularioAlerta.ValorMetrica);
            calendario.Date = dadosFormularioAlerta.Data;

            fillDDL_CCC2();
            fillDDL_MUNIC();

            //Atualiza o filtro para o autocomplete das ERB'S
            atualizaFiltroAutoComplete2();

            //Altera a hora da seleção
            this.HF_HORA.Value = dadosFormularioAlerta.Data.Hour.ToString();

            //> Prepara os dados da Metrica
            metricas.TratarMetricas(Page, L_ERRO_SEM_DADOS, dadosFormularioAlerta.ValorMetrica.ToUpper(), dadosFormularioAlerta.DescricaoMetrica.ToUpper(), ref unidade_metricas, ref grupo_metricas, ref dados_metrica, ref qtd_metricas, ref id_metricas, ref nome_metricas,
            "BTS");

            //constroi a instrução sql como os dados vindo externos (objeto com dados)
            sqlClause = ObterInstrucaoSQL(dadosFormularioAlerta, grupo_metricas, dados_metrica.Rows[0]["ORDEM"].ToString());
        }

        try
        {
            //Obtem a lista dos campos que devem ser double
            //Excluído da lista [LATITUDE_DEC, LONGITUDE_DEC]
            string listaParseDouble = grupo_metricas;

            //Obtem os dados da base Oracle
            DataTable DT = ConsultaDados.ConsultaOracle(sqlClause, "DT", listaParseDouble);

            #region Comentado [Abertura de instância ao oracle]
            //OracleConnection ORACLEConnection = default(OracleConnection);
            //string sConnString = ConfigurationManager.ConnectionStrings["ORACLE"].ConnectionString;
            //ORACLEConnection = new OracleConnection(sConnString);

            ////Conexão com Oracle - Obtem os dados
            //ORACLEConnection.Open();

            //OracleDataReader dr = default(OracleDataReader);
            //OracleCommand mycommandO = new OracleCommand();
            //mycommandO.Connection = ORACLEConnection;
            //mycommandO.CommandTimeout = 200;
            //mycommandO.CommandType = CommandType.Text;

            //mycommandO.CommandText = sql;
            ////constroi a instrução sql como os dados do formulário

            //dr = mycommandO.ExecuteReader(CommandBehavior.CloseConnection);
            #endregion

            //> Verifica se a consulta não retornou nenhum registro.
            //VvVvVvV NOVAS_METRICAS (modificar aqui) VvVvVvV
            if (DT.Rows.Count > 0)
            {
                #region Comentado [Consulta a Base Oracle]
                //DataTable DT = new DataTable();
                //DT.TableName = "DT";
                //int c = 0;

                //for (c = 0; c <= dr.FieldCount - 1; c++)
                //{
                //    DT.Columns.Add(dr.GetName(c));

                //    if (grupo_metricas.Contains(DT.Columns[c].ColumnName) || DT.Columns[c].ColumnName == "LATITUDE_DEC" || DT.Columns[c].ColumnName == "LONGITUDE_DEC")
                //        DT.Columns[c].DataType = System.Type.GetType("System.Double");
                //}

                //int j = 0;
                //int x = 0;
                //while (dr.Read())
                //{
                //    DataRow drTemp = null;
                //    drTemp = DT.NewRow();
                //    for (j = 0; j <= DT.Columns.Count - 1; j++)
                //    {
                //        x = 0;
                //        drTemp[j] = dr[j];
                //    }
                //    DT.Rows.Add(drTemp);
                //}

                //mycommandO.Dispose();
                //ORACLEConnection.Dispose();
                #endregion

                //Abre instancia para a biblioteca MapaUtil
                MapaUtil mapaUtil = new MapaUtil(Map1);

                if (radiobutton_cobertura.Checked)
                {
                    string intrucaoSQL_Chave = string.Empty;
                    string intrucaoSQL_UFBTS = string.Empty;

                    TrataInstrucaoSQLtoBuscaPredicao(DT, ref intrucaoSQL_Chave, ref intrucaoSQL_UFBTS);

                    //Busca no resource "PredicaoCobertura" com o filtro erb
                    DataTable dtPredicao = mapaUtil.ObterFeaturesGIS("NameBusca in (" + intrucaoSQL_UFBTS + ")", "PredicaoCobertura", DDL_uf.SelectedItem.Value, null, MapaObjects.Enums.enumTipoFeaturesBusca.GISResouce);

                    Identificacao.clsExibirPredicaoCobertura controlePredicaoCobertura = new Identificacao.clsExibirPredicaoCobertura();
                    controlePredicaoCobertura.MostraPredicaoCobertura(dtPredicao, intrucaoSQL_Chave);
                    return;
                }

                if ((mapaUtil.ObterMapResource(GisResources.DESTAQUE) == null))
                {
                    //Cria um novo ResourceItem para mostrar a figura gráfica
                    IMapResource mapResourceHighLight = mapaUtil.CriarGraphicsResource(MapResourceManager1, GisResources.DESTAQUE, 1);
                }

                #region GraphicLayer

                //>>>>>>>>>>>>>>>>>>>>      GRAPHICS_LAYER    >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                List<Atributos> listaAtributos = new List<Atributos>();
                listaAtributos.Add(new Atributos("ATRIBUTO", typeof(double)));
                listaAtributos.Add(new Atributos("CHAVE", typeof(string)));
                listaAtributos.Add(new Atributos("SETOR", typeof(string)));

                listaAtributos.Add(new Atributos("ERB", typeof(string)));
                listaAtributos.Add(new Atributos("UF", typeof(string)));

                List<Atributos> listaAtributosMarcador = new List<Atributos>();
                listaAtributosMarcador.Add(new Atributos("ID", typeof(string)));

                //Obtem o resouce "DESEMPENHO"
                ESRI.ArcGIS.ADF.Web.DataSources.Graphics.MapResource gResource = mapaUtil.ObterMapResource(GisResources.PONTOS);

                gResource.Graphics.Tables.Clear();

                //>>Cria a featureGraphicLayer e prepara para receber os dados
                string tabelaGrafica = string.Empty;
                if ((dadosFormularioAlerta != null))
                {
                    tabelaGrafica = ObterDescricaoMetrica(new clsControleDadosFiltroFormulario().DadosSessao.ValorMetrica);
                }
                else
                {
                    tabelaGrafica = this.DDL_metrica.SelectedItem.Text;
                }

                //Cria os FeatureGraphicsLayer
                FeatureGraphicsLayer fglayer = mapaUtil.CriarFeatureGraphicLayer(tabelaGrafica, GisResources.PONTOS, listaAtributos);
                FeatureGraphicsLayer fglayer_Marcador = mapaUtil.CriarFeatureGraphicLayer("Destaque", GisResources.DESTAQUE, listaAtributosMarcador);

                DataRow Drow = null;
                DataRow dataRowFeatureGraphicLayer = null;
                DataRow dataRowFeatureGraphicLayer_Marcador = null;
                int cont = 0;

                string valorMetrica = null;
                if ((dadosFormularioAlerta == null))
                {
                    valorMetrica = this.DDL_metrica.SelectedItem.Value.ToUpper();
                }
                else
                {
                    valorMetrica = dadosFormularioAlerta.ValorMetrica.ToUpper();
                }

                extensaoMapa = new Envelope();
                ESRI.ArcGIS.ADF.Web.Geometry.Geometry geomAux = null;
                Point pontoAux = null;


                foreach (DataRow Drow_loopVariable in DT.Rows)
                {
                    Drow = Drow_loopVariable;
                    //if ((Drow["LONGITUDE_DEC"].ToString() == "*" || Drow["LATITUDE_DEC"].ToString().Equals("*")))
                    //{
                    //    continue;
                    //}

                    dataRowFeatureGraphicLayer = fglayer.NewRow();



                    if (this.radiobutton_setor.Checked)
                    {
                        //>>>>>>>>>>>>> SETOR >>>>>>>>>>>>>>>>>>>>>>

                        try
                        {
                            if ((Drow["AZIMUTE"] == null || Drow["AZIMUTE"].ToString().Equals("*") || Drow["LONGITUDE_DEC"].ToString().Equals("*") || Drow["LATITUDE_DEC"].ToString().Equals("*")))
                            {
                                continue;
                            }

                            ESRI.ArcGIS.ADF.Web.Geometry.Point pPoint1 = default(ESRI.ArcGIS.ADF.Web.Geometry.Point);
                            pPoint1 = new ESRI.ArcGIS.ADF.Web.Geometry.Point();
                            pPoint1.SpatialReference = Map1.SpatialReference;



                            pPoint1.X = Double.Parse(Drow["LONGITUDE_DEC"].ToString().Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat);
                            pPoint1.Y = Double.Parse(Drow["LATITUDE_DEC"].ToString().Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat);

                            ESRI.ArcGIS.ADF.Web.Geometry.Point pPoint2 = default(ESRI.ArcGIS.ADF.Web.Geometry.Point);
                            pPoint2 = new ESRI.ArcGIS.ADF.Web.Geometry.Point();
                            pPoint2.SpatialReference = Map1.SpatialReference;

                            //Tamanho da linha que representa o Setor
                            double D = 0.0009;

                            pPoint2.X = pPoint1.X + (D * Math.Sin(double.Parse(Drow["AZIMUTE"].ToString(), CultureInfo.InvariantCulture.NumberFormat) * (Math.PI / 180)));
                            pPoint2.Y = pPoint1.Y + (D * Math.Cos(double.Parse(Drow["AZIMUTE"].ToString(), CultureInfo.InvariantCulture.NumberFormat) * (Math.PI / 180)));

                            ESRI.ArcGIS.ADF.Web.Geometry.Path pPath = new ESRI.ArcGIS.ADF.Web.Geometry.Path();
                            pPath.Points.Add(pPoint1);
                            pPath.Points.Add(pPoint2);

                            ESRI.ArcGIS.ADF.Web.Geometry.Polyline pLine = new ESRI.ArcGIS.ADF.Web.Geometry.Polyline();
                            pLine.Paths.Add(pPath);

                            dataRowFeatureGraphicLayer[fglayer.GeometryColumnIndex] = pLine;

                            //Trata o valor da métrica a ser colorido
                            double valorResultadoMetrica; // utilizada somente para converter
                            if (!double.TryParse(Drow[valorMetrica].ToString(), out valorResultadoMetrica))
                                valorResultadoMetrica = 0;
                            //Trata o valor da métrica a ser colorido

                            dataRowFeatureGraphicLayer["ATRIBUTO"] = valorResultadoMetrica;

                            dataRowFeatureGraphicLayer["CHAVE"] = Drow["CHAVE"].ToString();
                            dataRowFeatureGraphicLayer["SETOR"] = Drow["SETOR"].ToString();

                            dataRowFeatureGraphicLayer["ERB"] = Drow["BTS"].ToString();
                            dataRowFeatureGraphicLayer["UF"] = Drow["UF"].ToString();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        
                    }
                    else if ((this.radiobutton_erb.Checked))
                    {
                        
                        //>>>>>>>>>>>>> ERB >>>>>>>>>>>>>>>>>>>>>>
                        ESRI.ArcGIS.ADF.Web.Geometry.Point pPoint = new ESRI.ArcGIS.ADF.Web.Geometry.Point();

                        pPoint.X = double.Parse(Drow["LONGITUDE_DEC"].ToString());
                        pPoint.Y = double.Parse(Drow["LATITUDE_DEC"].ToString());
                        pPoint.SpatialReference = Map1.SpatialReference;

                        dataRowFeatureGraphicLayer[fglayer.GeometryColumnIndex] = pPoint;

                        dataRowFeatureGraphicLayer["CHAVE"] = Drow["CHAVE"].ToString();

                        dataRowFeatureGraphicLayer["ERB"] = Drow["BTS"].ToString();
                        dataRowFeatureGraphicLayer["UF"] = Drow["UF"].ToString();

                        //Trata o valor da métrica a ser colorido
                        double valorResultadoMetrica; // utilizada somente para converter
                        if (!double.TryParse(Drow[valorMetrica].ToString(), out valorResultadoMetrica))
                            valorResultadoMetrica = 0;
                        //Trata o valor da métrica a ser colorido

                        dataRowFeatureGraphicLayer["ATRIBUTO"] = valorResultadoMetrica;

                        //valida qual erb o usuario passou por parametro para dar zoom

                        if ((dadosFormularioAlerta != null))
                        {
                            //valida se a 'uf+erb' existe na lista de erbs passadas como parametro

                            if ((Uf_Erb.ExisteErb(dadosFormularioAlerta.UF_ERB, Drow["UF"].ToString(), Drow["BTS"].ToString()) | Drow["BTS"].ToString().Equals(dadosFormularioAlerta.ERB)))
                            {
                                dataRowFeatureGraphicLayer_Marcador = fglayer_Marcador.NewRow();

                                //Setar atributos para a feature graphics layer (marcador)
                                dataRowFeatureGraphicLayer_Marcador[fglayer_Marcador.GeometryColumnIndex] = pPoint;
                                dataRowFeatureGraphicLayer_Marcador["ID"] = "0";

                                fglayer_Marcador.Rows.Add(dataRowFeatureGraphicLayer_Marcador);

                                //Ponto auxiliar para dar zoom quando for uma única erb
                                pontoAux = pPoint;
                                pontoAux.SpatialReference = Map1.SpatialReference;

                                //Guarda os valores para dar zoom nas feições selecionadas
                                geomAux = pPoint;
                                extensaoMapa.Union(geomAux);

                                cont = cont + 1;

                            }
                        }
                    }

                    fglayer.Rows.Add(dataRowFeatureGraphicLayer);

                }

                #endregion

                if ((dadosFormularioAlerta != null))
                {
                    if (dadosFormularioAlerta.MultiplasERBs())
                    {
                        Map1.Extent = (cont == 1 ? pontoAux.Expand(3, Map1.Extent) : extensaoMapa.Expand(7));
                    }
                }

                #region Simbologia

                
                //>>>>>>> Instancia a lista de valores do range que será configurado pelo usuário >>>>>>>>>>>>>>>>>>
                List<clsControleSimbologiaRange> listaValoresRange = new List<clsControleSimbologiaRange>();

                //>>>>>>>>>>>>>>>         VALUE RANGE         >>>>>>>>>>>>>>>>>>>>>>
                ValueRange<double> valueRange1 = new ValueRange<double>();
                ValueRange<double> valueRange2 = new ValueRange<double>();
                ValueRange<double> valueRange3 = new ValueRange<double>();
                ValueRange<double> valueRange4 = new ValueRange<double>();
                ValueRange<double> valueRange5 = new ValueRange<double>();

                //>>>> Simbolos   >>>>>>

                //>> Pontos (ERBs)
                SimpleMarkerSymbol pFS1 = new SimpleMarkerSymbol(COR(this.DDL_COR1.Value), 8, MarkerSymbolType.Star);
                pFS1.OutlineColor = System.Drawing.Color.Black;
                SimpleMarkerSymbol pFS2 = new SimpleMarkerSymbol(COR(this.DDL_COR2.Value), 8, MarkerSymbolType.Square);
                pFS2.OutlineColor = System.Drawing.Color.Black;
                SimpleMarkerSymbol pFS3 = new SimpleMarkerSymbol(COR(this.DDL_COR3.Value), 8, MarkerSymbolType.Circle);
                pFS3.OutlineColor = System.Drawing.Color.Black;
                SimpleMarkerSymbol pFS4 = new SimpleMarkerSymbol(COR(this.DDL_COR4.Value), 8, MarkerSymbolType.Triangle);
                pFS4.OutlineColor = System.Drawing.Color.Black;
                SimpleMarkerSymbol pFS5 = new SimpleMarkerSymbol(COR(this.DDL_COR5.Value), 8, MarkerSymbolType.Cross);
                pFS5.OutlineColor = System.Drawing.Color.Black;

                //>> Linhas (Setores)

                SimpleLineSymbol lFS1 = new SimpleLineSymbol(COR(this.DDL_COR1.Value), 6);
                SimpleLineSymbol lFS2 = new SimpleLineSymbol(COR(this.DDL_COR2.Value), 6);
                SimpleLineSymbol lFS3 = new SimpleLineSymbol(COR(this.DDL_COR3.Value), 6);
                SimpleLineSymbol lFS4 = new SimpleLineSymbol(COR(this.DDL_COR4.Value), 6);
                SimpleLineSymbol lFS5 = new SimpleLineSymbol(COR(this.DDL_COR5.Value), 6);

                if (this.radiobutton_erb.Checked)
                {
                    valueRange1.Symbol = pFS1;
                    valueRange2.Symbol = pFS2;
                    valueRange3.Symbol = pFS3;
                    valueRange4.Symbol = pFS4;
                    valueRange5.Symbol = pFS5;
                }
                else
                {
                    valueRange1.Symbol = lFS1;
                    valueRange2.Symbol = lFS2;
                    valueRange3.Symbol = lFS3;
                    valueRange4.Symbol = lFS4;
                    valueRange5.Symbol = lFS5;
                }

                //>> SIMBOLO DEFAULT
                SimpleMarkerSymbol pontoDefault = new SimpleMarkerSymbol();
                pontoDefault.Color = System.Drawing.Color.Black;
                pontoDefault.Width = 8;

                SimpleLineSymbol linhaDefault = new SimpleLineSymbol();
                linhaDefault.Color = System.Drawing.Color.Black;
                linhaDefault.Width = 8;
                //>>>>>>>>>>>>>>>>>>>>>>>  METRICAS   >>>>>>>>>>>>>>>>>

                //Obtem a listagem da formatação definida pelo usuário

                //>>>>>>> EFICIENCIA <<<<<<<

                if (
                    (dados_metrica.Rows[0]["ABSOLUTO"].ToString() == "NAO" && this.CBOX_person.Checked == false) || 
                    (dados_metrica.Rows[0]["ABSOLUTO"].ToString() == "NAO" && this.CBOX_person.Checked) || 
                    (dados_metrica.Rows[0]["ABSOLUTO"].ToString() == "SIM" && this.CBOX_person.Checked)
                    )
                {
                    string symbolLabel = string.Empty;

                    if (this.CBOX_quebra1.Checked)
                    {
                        //>>>> ValueRange1
                        symbolLabel = this.TBOX_quebra11.Text + dados_metrica.Rows[0]["UNIDADE"] + " a " + this.TBOX_quebra12.Text + dados_metrica.Rows[0]["UNIDADE"];

                        valueRange1.MinValue = double.Parse(this.TBOX_quebra11.Text);
                        valueRange1.MaxValue = double.Parse(this.TBOX_quebra12.Text);
                        valueRange1.Bounds = RangeBounds.All;
                        valueRange1.SymbolLabel = symbolLabel;

                        //Preenche a lista de range
                        listaValoresRange.Add(new clsControleSimbologiaRange(new double[] {
						    double.Parse(this.TBOX_quebra11.Text),
						    double.Parse(this.TBOX_quebra12.Text)
					    }, symbolLabel, COR(this.DDL_COR1.Value)));

                    }
                    if (this.CBOX_quebra2.Checked)
                    {
                        //>>>> ValueRange2
                        symbolLabel = this.TBOX_quebra21.Text + dados_metrica.Rows[0]["UNIDADE"] + " a " + this.TBOX_quebra22.Text + dados_metrica.Rows[0]["UNIDADE"];

                        valueRange2.MinValue = double.Parse(this.TBOX_quebra21.Text);
                        valueRange2.MaxValue = double.Parse(this.TBOX_quebra22.Text);
                        valueRange2.Bounds = RangeBounds.All;
                        valueRange2.SymbolLabel = symbolLabel;
                        //<<<<

                        //Preenche a lista de range
                        listaValoresRange.Add(new clsControleSimbologiaRange(new double[] {
						    double.Parse(this.TBOX_quebra21.Text),
						    double.Parse(this.TBOX_quebra22.Text)
					    }, symbolLabel, COR(this.DDL_COR2.Value)));

                    }
                    if (this.CBOX_quebra3.Checked)
                    {
                        //>>>> ValueRange3

                        symbolLabel = this.TBOX_quebra31.Text + dados_metrica.Rows[0]["UNIDADE"] + " a " + this.TBOX_quebra32.Text + dados_metrica.Rows[0]["UNIDADE"];

                        valueRange3.MinValue = double.Parse(this.TBOX_quebra31.Text);
                        valueRange3.MaxValue = double.Parse(this.TBOX_quebra32.Text);
                        valueRange3.Bounds = RangeBounds.All;
                        valueRange3.SymbolLabel = symbolLabel;
                        //<<<<

                        //Preenche a lista de range
                        listaValoresRange.Add(new clsControleSimbologiaRange(new double[] {
						    double.Parse(this.TBOX_quebra31.Text),
						    double.Parse(this.TBOX_quebra32.Text)
					    }, symbolLabel, COR(this.DDL_COR3.Value)));


                    }
                    if (this.CBOX_quebra4.Checked)
                    {
                        //>>>> ValueRange4

                        symbolLabel = this.TBOX_quebra41.Text + dados_metrica.Rows[0]["UNIDADE"] + " a " + this.TBOX_quebra42.Text + dados_metrica.Rows[0]["UNIDADE"];

                        valueRange4.MinValue = double.Parse(this.TBOX_quebra41.Text);
                        valueRange4.MaxValue = double.Parse(this.TBOX_quebra42.Text);
                        valueRange4.Bounds = RangeBounds.All;
                        valueRange4.SymbolLabel = this.TBOX_quebra41.Text + dados_metrica.Rows[0]["UNIDADE"] + " a " + this.TBOX_quebra42.Text + dados_metrica.Rows[0]["UNIDADE"];
                        //<<<<

                        //Preenche a lista de range
                        listaValoresRange.Add(new clsControleSimbologiaRange(new double[] {
						    double.Parse(this.TBOX_quebra41.Text),
						    double.Parse(this.TBOX_quebra42.Text)
					    }, symbolLabel, COR(this.DDL_COR4.Value)));

                    }
                    if (this.CBOX_quebra5.Checked)
                    {
                        //>>>> ValueRange3

                        symbolLabel = this.TBOX_quebra51.Text + dados_metrica.Rows[0]["UNIDADE"] + " a " + this.TBOX_quebra52.Text + dados_metrica.Rows[0]["UNIDADE"];

                        valueRange5.MinValue = double.Parse(this.TBOX_quebra51.Text);
                        valueRange5.MaxValue = double.Parse(this.TBOX_quebra52.Text);
                        valueRange5.Bounds = RangeBounds.All;
                        valueRange5.SymbolLabel = this.TBOX_quebra51.Text + dados_metrica.Rows[0]["UNIDADE"] + " a " + this.TBOX_quebra52.Text + dados_metrica.Rows[0]["UNIDADE"];
                        //<<<<

                        //Preenche a lista de range
                        listaValoresRange.Add(new clsControleSimbologiaRange(new double[] {
						    double.Parse(this.TBOX_quebra51.Text),
						    double.Parse(this.TBOX_quebra52.Text)
					    }, symbolLabel, COR(this.DDL_COR5.Value)));

                    }

                    ValueMapRenderer<double> vmrender = new ValueMapRenderer<double>();
                    vmrender.ValueColumnName = "ATRIBUTO";
                    if (this.CBOX_person_defsymb.Checked)
                    {
                        if (this.radiobutton_erb.Checked)
                            vmrender.DefaultSymbol = pontoDefault;
                        else
                            vmrender.DefaultSymbol = linhaDefault;

                        vmrender.DefaultLabel = "Sem Dados";
                    }

                    ValueCollection<double> valcoll = vmrender.Values;

                    if (this.CBOX_quebra1.Checked)
                    {
                        valcoll.Add(valueRange1);
                    }
                    if (this.CBOX_quebra2.Checked)
                    {
                        valcoll.Add(valueRange2);
                    }
                    if (this.CBOX_quebra3.Checked)
                    {
                        valcoll.Add(valueRange3);
                    }
                    if (this.CBOX_quebra4.Checked)
                    {
                        valcoll.Add(valueRange4);
                    }
                    if (this.CBOX_quebra5.Checked)
                    {
                        valcoll.Add(valueRange5);
                    }

                    fglayer.Renderer = vmrender;
                }
                else
                {
                    int classcount = 0;
                    IClassifyGEN classify = default(IClassifyGEN);
                    ESRI.ArcGIS.Carto.ITableHistogram tablehistogram = default(ESRI.ArcGIS.Carto.ITableHistogram);
                    ESRI.ArcGIS.Carto.IBasicHistogram histogram = default(ESRI.ArcGIS.Carto.IBasicHistogram);

                    tablehistogram = new BasicTableHistogramClass();
                    tablehistogram.Field = valorMetrica;

                    tablehistogram.Table = CreateITable(DT);

                    object datavalues = null;
                    object datafrequencies = null;
                    datavalues = null;
                    datafrequencies = null;

                    histogram = tablehistogram as IBasicHistogram;
                    histogram.GetHistogram(out datavalues, out datafrequencies);

                    classify = new NaturalBreaks();
                    //classify.SetHistogramData(datavalues, datafrequencies)

                    classcount = 3;
                    classify.Classify(datavalues, datafrequencies, ref classcount);

                    double[] classbreaksarray = null;
                    classbreaksarray = classify.ClassBreaks as double[];

                    if (classbreaksarray.Length != 0)
                    {
                        // Estava classbreaksarray.length < 3

                        if (classbreaksarray[0] == classbreaksarray[1] && classbreaksarray.Length <= 3)
                        {
                            

                            ValueMapRenderer<double> vmrender = new ValueMapRenderer<double>();
                            vmrender.DefaultLabel = "Sem Dados";
                            vmrender.ValueColumnName = "ATRIBUTO";

                            if (this.radiobutton_erb.Checked)
                                vmrender.DefaultSymbol = pontoDefault;
                            else
                                vmrender.DefaultSymbol = linhaDefault;

                            ValueCollection<double> valcoll = default(ValueCollection<double>);
                            valcoll = vmrender.Values;

                            fglayer.Renderer = vmrender;
                        }
                        else
                        {

                            //String auxiliar para preencher com o symbolLabel do range
                            string symbolLabel = string.Empty;

                            //>>>> ValueRange3
                            symbolLabel = classbreaksarray[0].ToString() + " a " + classbreaksarray[1].ToString() + " " + dados_metrica.Rows[0]["UNIDADE"].ToString();

                            valueRange3.MinValue = classbreaksarray[0];
                            valueRange3.MaxValue = classbreaksarray[1];

                            SimpleMarkerSymbol markerFS3 = new SimpleMarkerSymbol((DDL_metrica.SelectedItem.Text.Contains("Throughput") ? System.Drawing.Color.Red : System.Drawing.Color.Green), 8, MarkerSymbolType.Cross);
                            SimpleLineSymbol lineFS3 = new SimpleLineSymbol((DDL_metrica.SelectedItem.Text.Contains("Throughput") ? System.Drawing.Color.Red : System.Drawing.Color.Green), 8, LineType.Solid);
                            markerFS3.OutlineColor = System.Drawing.Color.Black;

                            valueRange3.Bounds = RangeBounds.All;
                            valueRange3.Symbol = (this.radiobutton_erb.Checked ? markerFS3 as FeatureSymbol : lineFS3 as FeatureSymbol);
                            valueRange3.SymbolLabel = symbolLabel;

                            listaValoresRange.Add(new clsControleSimbologiaRange(new double[] {
							    classbreaksarray[0],
							    classbreaksarray[1]
						    }, symbolLabel, COR(this.DDL_COR1.Value)));

                            
                            //>>>> ValueRange2
                            symbolLabel = classbreaksarray[1].ToString() + " a " + classbreaksarray[2].ToString() + " " + dados_metrica.Rows[0]["UNIDADE"];

                            valueRange2.MinValue = classbreaksarray[1];
                            valueRange2.MaxValue = classbreaksarray[2];

                            //SimpleMarkerSymbol FS2 = new SimpleMarkerSymbol(System.Drawing.Color.Orange, 8, MarkerSymbolType.Triangle);
                            //FS2.OutlineColor = System.Drawing.Color.Black;
                            
                            //valueRange2.Bounds = RangeBounds.All;
                            //valueRange2.Symbol = FS2;
                            //valueRange2.SymbolLabel = symbolLabel;

                            SimpleMarkerSymbol markerFS2 = new SimpleMarkerSymbol(System.Drawing.Color.Orange, 8, MarkerSymbolType.Triangle);
                            SimpleLineSymbol lineFS2 = new SimpleLineSymbol(System.Drawing.Color.Orange, 8, LineType.Solid);
                            markerFS2.OutlineColor = System.Drawing.Color.Black;

                            valueRange2.Bounds = RangeBounds.All;
                            valueRange2.Symbol = (this.radiobutton_erb.Checked ? markerFS2 as FeatureSymbol : lineFS2 as FeatureSymbol);
                            valueRange2.SymbolLabel = symbolLabel;


                            listaValoresRange.Add(new clsControleSimbologiaRange(new double[] {
							    classbreaksarray[1],
							    classbreaksarray[2]
						    }, symbolLabel, COR(this.DDL_COR2.Value)));
                            //<<<<

                            //>>>> ValueRange1

                            symbolLabel = classbreaksarray[2].ToString() + " a " + classbreaksarray[3].ToString() + " " + dados_metrica.Rows[0]["UNIDADE"];

                            valueRange1.MinValue = classbreaksarray[2];
                            valueRange1.MaxValue = classbreaksarray[3];

                            //Validação de Throughput
                            //SimpleMarkerSymbol FS1 = new SimpleMarkerSymbol((DDL_metrica.SelectedItem.Text.Contains("Throughput") ? System.Drawing.Color.Green : System.Drawing.Color.Red), 8, MarkerSymbolType.Circle);
                            //FS1.OutlineColor = System.Drawing.Color.Black;

                            //valueRange1.Bounds = RangeBounds.All;
                            //valueRange1.Symbol = FS1;
                            //valueRange1.SymbolLabel = symbolLabel;

                            SimpleMarkerSymbol markerFS1 = new SimpleMarkerSymbol((DDL_metrica.SelectedItem.Text.Contains("Throughput") ? System.Drawing.Color.Green : System.Drawing.Color.Red), 8, MarkerSymbolType.Circle);
                            SimpleLineSymbol lineFS1 = new SimpleLineSymbol((DDL_metrica.SelectedItem.Text.Contains("Throughput") ? System.Drawing.Color.Green : System.Drawing.Color.Red), 8, LineType.Solid);
                            markerFS1.OutlineColor = System.Drawing.Color.Black;

                            valueRange1.Bounds = RangeBounds.All;
                            valueRange1.Symbol = (this.radiobutton_erb.Checked ? markerFS1 as FeatureSymbol : lineFS1 as FeatureSymbol);
                            valueRange1.SymbolLabel = symbolLabel;

                            listaValoresRange.Add(new clsControleSimbologiaRange(new double[] {
							    classbreaksarray[2],
							    classbreaksarray[3]
						    }, symbolLabel, COR(this.DDL_COR3.Value)));
                            //<<<<

                            ValueMapRenderer<double> vmrender = new ValueMapRenderer<double>();
                            vmrender.ValueColumnName = "ATRIBUTO";
                            vmrender.DefaultLabel = "Sem Dados";

                            if (this.radiobutton_erb.Checked)
                                vmrender.DefaultSymbol = pontoDefault;
                            else
                                vmrender.DefaultSymbol = linhaDefault;

                            ValueCollection<double> valcoll = default(ValueCollection<double>);
                            valcoll = vmrender.Values;
                            valcoll.Add(valueRange1);
                            valcoll.Add(valueRange2);
                            valcoll.Add(valueRange3);

                            fglayer.Renderer = vmrender;
                        }
                    }
                    else
                    {
                        ValueMapRenderer<double> vmrender = new ValueMapRenderer<double>();
                        vmrender.ValueColumnName = "ATRIBUTO";
                        vmrender.DefaultLabel = "Sem Dados";

                        if (this.radiobutton_erb.Checked)
                            vmrender.DefaultSymbol = pontoDefault;
                        else
                            vmrender.DefaultSymbol = linhaDefault;

                        ValueCollection<double> valcoll = default(ValueCollection<double>);
                        valcoll = vmrender.Values;

                        fglayer.Renderer = vmrender;
                    }
                }

                #endregion

                //Atribui a sessão a lista de valores configurados pelo usuário do range
                this.Page.Session["DadosRange"] = listaValoresRange;

                //gResource.Graphics.Tables.Clear()
                //gResource.Graphics.Tables.Add(fglayer)

                //> Limpa graphiclayer "DESEMPENHO" no MapResourceManager1
                //mapaUtil.ObterMapResourceItem(MapResourceManager1, "DESEMPENHO").LayerDefinitions.Nodes.Clear()

                //Exclui o resource de HighLight
                ControlarExistenciaResources();

                //Gera HIGHLIGHT no ponto destacado
                if ((dadosFormularioAlerta != null))
                {
                    //Valida a existência do resourceItem HIGHLIGHT e a deleta se encontrar

                    if ((mapaUtil.ObterMapResource(GisResources.DESTAQUE) == null))
                    {
                        //Cria um novo ResourceItem para mostrar a figura gráfica
                        IMapResource mapResourceHighLight = mapaUtil.CriarGraphicsResource(MapResourceManager1, GisResources.DESTAQUE, 1);
                    }
                    else
                    {
                        //Limpa as tabelas já inseridas.
                        //mapaUtil.ObterMapResource(NOME_RESOURCE_DESTAQUE_RF).Graphics.Tables.Clear()
                    }

                    List<SimbologiaGeometria> listaSimbologiaMarcador = new List<SimbologiaGeometria>();

                    listaSimbologiaMarcador.Add(new SimbologiaGeometria("0", System.Drawing.Color.Aqua, enumTipoSimbologia.Ponto, 20));
                    fglayer_Marcador.Renderer = SimbologiaUtil.ObtemRenderizacaoSimbologias(listaSimbologiaMarcador, "ID");

                    //Dim geometry As Geometry = erbZoom
                    //Dim graphicElement As GraphicElement = Nothing

                    //Dim simpleMarkerSymbol As New SimpleMarkerSymbol(Drawing.Color.Aqua, 20, MarkerSymbolType.Circle)
                    //simpleMarkerSymbol.OutlineColor = Drawing.Color.Black

                    //graphicElement = New GraphicElement(geometry, simpleMarkerSymbol)
                    //mapaUtil.AdicionarGraphicsElements(graphicElement, NOME_RESOURCE_DESTAQUE_RF)

                    //Dim elementGraphicsLayer As ElementGraphicsLayer = New ElementGraphicsLayer()
                    //elementGraphicsLayer.Add(graphicElement)

                    //mapaUtil.AdicionarElementGraphicsLayer(elementGraphicsLayer, NOME_RESOURCE_DESTAQUE_RF)

                }

                //Remove o painel com as informações das ERB's
                Map1.CallbackResults.Add(CallbackResult.CreateJavaScript("preencheInformacoesSite(null, 0)"));

                Map1.RefreshResource(gResource.Name);
                Map1.RefreshResource(GisResources.DESTAQUE);

                //>>>>>>>>>>  Código para colocar a legenda no TOC >>>>>>>>>>>>>>>>>>
                Toc adfToc = (Toc)Toc1;
                adfToc.Refresh();

                //>>>>>>>>>> Varre os Itens >>>>>>>>>>>>>>>>>>
                foreach (TreeViewPlusNode item in Toc1.Nodes)
                {
                    if (item.Value.Equals(GisResources.PONTOS))
                    {
                        //Expande o item encontrado
                        item.Expanded = true;
                        item.Nodes[0].Expanded = true;
                        item.Nodes[0].ShowCheckBox = false;

                        break; // TODO: might not be correct. Was : Exit For
                    }
                }

                fglayer.Dispose();
            }
            else
            {
                this.L_ERRO_SEM_DADOS.Height = 70;
                this.L_ERRO_SEM_DADOS.Width = 200;
                this.L_ERRO_SEM_DADOS.Text = Resource.msg_ErroConsulta;
                this.L_ERRO_SEM_DADOS.Visible = true;

                //> Limpa o resultado anterior
                //Abre instancia para a biblioteca MapaUtil
                MapaUtil mapaUtil = new MapaUtil(Map1);

                //Obtem o resouce "DESEMPENHO"
                ESRI.ArcGIS.ADF.Web.DataSources.Graphics.MapResource gResource = mapaUtil.ObterMapResource(GisResources.PONTOS);
                gResource.Graphics.Tables.Clear();
            }
        }
        catch
        {
            this.L_ERRO_SEM_DADOS.Height = 40;
            this.L_ERRO_SEM_DADOS.Width = 200;
            this.L_ERRO_SEM_DADOS.Text = Resource.msg_ErroAltaia;
            this.L_ERRO_SEM_DADOS.Visible = true;
        }
    }

    }
}
