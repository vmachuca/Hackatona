﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.239.
// 
#pragma warning disable 1591

namespace Core.CoberturaService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="SOAPServiceSoap", Namespace="http://tempuri.org/")]
    public partial class SOAPService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback BuscarOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public SOAPService() {
            this.Url = global::Core.Properties.Settings.Default.Core_CoberturaService_SOAPService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event BuscarCompletedEventHandler BuscarCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Buscar", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Cobertura Buscar(double x, double y) {
            object[] results = this.Invoke("Buscar", new object[] {
                        x,
                        y});
            return ((Cobertura)(results[0]));
        }
        
        /// <remarks/>
        public void BuscarAsync(double x, double y) {
            this.BuscarAsync(x, y, null);
        }
        
        /// <remarks/>
        public void BuscarAsync(double x, double y, object userState) {
            if ((this.BuscarOperationCompleted == null)) {
                this.BuscarOperationCompleted = new System.Threading.SendOrPostCallback(this.OnBuscarOperationCompleted);
            }
            this.InvokeAsync("Buscar", new object[] {
                        x,
                        y}, this.BuscarOperationCompleted, userState);
        }
        
        private void OnBuscarOperationCompleted(object arg) {
            if ((this.BuscarCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.BuscarCompleted(this, new BuscarCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.225")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class Cobertura {
        
        private JavaScriptSerializer jssField;
        
        private bool erroField;
        
        private string tipoErroField;
        
        private string nomeMunicipioField;
        
        private string iBGEField;
        
        private string ufField;
        
        private bool municipio_Movel_Voz_GSMField;
        
        private bool municipio_Movel_Dados_3GField;
        
        private bool movel_Voz_GSMField;
        
        private bool movel_Fixo_FWTField;
        
        private bool movel_Web_FWTField;
        
        private bool movel_Dados_3GField;
        
        private bool movel_Dados_2GField;
        
        private bool fixa_VozField;
        
        private bool fixa_DadosField;
        
        private string aTCNLField;
        
        private double distanciaField;
        
        private double velocidade_Fixa_DadosField;
        
        private bool tVA_VozField;
        
        private bool tVA_Dados_JatoField;
        
        private bool tVA_TVField;
        
        private bool fibra_VozField;
        
        private bool fibra_DadosField;
        
        private bool fibra_TVField;
        
        private bool dTHField;
        
        /// <remarks/>
        public JavaScriptSerializer jss {
            get {
                return this.jssField;
            }
            set {
                this.jssField = value;
            }
        }
        
        /// <remarks/>
        public bool Erro {
            get {
                return this.erroField;
            }
            set {
                this.erroField = value;
            }
        }
        
        /// <remarks/>
        public string TipoErro {
            get {
                return this.tipoErroField;
            }
            set {
                this.tipoErroField = value;
            }
        }
        
        /// <remarks/>
        public string NomeMunicipio {
            get {
                return this.nomeMunicipioField;
            }
            set {
                this.nomeMunicipioField = value;
            }
        }
        
        /// <remarks/>
        public string IBGE {
            get {
                return this.iBGEField;
            }
            set {
                this.iBGEField = value;
            }
        }
        
        /// <remarks/>
        public string UF {
            get {
                return this.ufField;
            }
            set {
                this.ufField = value;
            }
        }
        
        /// <remarks/>
        public bool Municipio_Movel_Voz_GSM {
            get {
                return this.municipio_Movel_Voz_GSMField;
            }
            set {
                this.municipio_Movel_Voz_GSMField = value;
            }
        }
        
        /// <remarks/>
        public bool Municipio_Movel_Dados_3G {
            get {
                return this.municipio_Movel_Dados_3GField;
            }
            set {
                this.municipio_Movel_Dados_3GField = value;
            }
        }
        
        /// <remarks/>
        public bool Movel_Voz_GSM {
            get {
                return this.movel_Voz_GSMField;
            }
            set {
                this.movel_Voz_GSMField = value;
            }
        }
        
        /// <remarks/>
        public bool Movel_Fixo_FWT {
            get {
                return this.movel_Fixo_FWTField;
            }
            set {
                this.movel_Fixo_FWTField = value;
            }
        }
        
        /// <remarks/>
        public bool Movel_Web_FWT {
            get {
                return this.movel_Web_FWTField;
            }
            set {
                this.movel_Web_FWTField = value;
            }
        }
        
        /// <remarks/>
        public bool Movel_Dados_3G {
            get {
                return this.movel_Dados_3GField;
            }
            set {
                this.movel_Dados_3GField = value;
            }
        }
        
        /// <remarks/>
        public bool Movel_Dados_2G {
            get {
                return this.movel_Dados_2GField;
            }
            set {
                this.movel_Dados_2GField = value;
            }
        }
        
        /// <remarks/>
        public bool Fixa_Voz {
            get {
                return this.fixa_VozField;
            }
            set {
                this.fixa_VozField = value;
            }
        }
        
        /// <remarks/>
        public bool Fixa_Dados {
            get {
                return this.fixa_DadosField;
            }
            set {
                this.fixa_DadosField = value;
            }
        }
        
        /// <remarks/>
        public string ATCNL {
            get {
                return this.aTCNLField;
            }
            set {
                this.aTCNLField = value;
            }
        }
        
        /// <remarks/>
        public double Distancia {
            get {
                return this.distanciaField;
            }
            set {
                this.distanciaField = value;
            }
        }
        
        /// <remarks/>
        public double Velocidade_Fixa_Dados {
            get {
                return this.velocidade_Fixa_DadosField;
            }
            set {
                this.velocidade_Fixa_DadosField = value;
            }
        }
        
        /// <remarks/>
        public bool TVA_Voz {
            get {
                return this.tVA_VozField;
            }
            set {
                this.tVA_VozField = value;
            }
        }
        
        /// <remarks/>
        public bool TVA_Dados_Jato {
            get {
                return this.tVA_Dados_JatoField;
            }
            set {
                this.tVA_Dados_JatoField = value;
            }
        }
        
        /// <remarks/>
        public bool TVA_TV {
            get {
                return this.tVA_TVField;
            }
            set {
                this.tVA_TVField = value;
            }
        }
        
        /// <remarks/>
        public bool Fibra_Voz {
            get {
                return this.fibra_VozField;
            }
            set {
                this.fibra_VozField = value;
            }
        }
        
        /// <remarks/>
        public bool Fibra_Dados {
            get {
                return this.fibra_DadosField;
            }
            set {
                this.fibra_DadosField = value;
            }
        }
        
        /// <remarks/>
        public bool Fibra_TV {
            get {
                return this.fibra_TVField;
            }
            set {
                this.fibra_TVField = value;
            }
        }
        
        /// <remarks/>
        public bool DTH {
            get {
                return this.dTHField;
            }
            set {
                this.dTHField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.225")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class JavaScriptSerializer {
        
        private int maxJsonLengthField;
        
        private int recursionLimitField;
        
        /// <remarks/>
        public int MaxJsonLength {
            get {
                return this.maxJsonLengthField;
            }
            set {
                this.maxJsonLengthField = value;
            }
        }
        
        /// <remarks/>
        public int RecursionLimit {
            get {
                return this.recursionLimitField;
            }
            set {
                this.recursionLimitField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void BuscarCompletedEventHandler(object sender, BuscarCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class BuscarCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal BuscarCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Cobertura Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Cobertura)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591