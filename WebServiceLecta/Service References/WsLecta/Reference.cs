﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebServiceLecta.WsLecta {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:WebServiceEcaIrreg", ConfigurationName="WsLecta.WebServiceEcaIrregPortType")]
    public interface WebServiceEcaIrregPortType {
        
        // CODEGEN: Se está generando un contrato de mensaje, ya que el espacio de nombres de contenedor (urn:obtenerRegistro) del mensaje WebServiceEcaIrregRequest no coincide con el valor predeterminado (urn:WebServiceEcaIrreg)
        [System.ServiceModel.OperationContractAttribute(Action="urn:obtenerRegistro#Registro", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        WebServiceLecta.WsLecta.WebServiceEcaIrregResponse WebServiceEcaIrreg(WebServiceLecta.WsLecta.WebServiceEcaIrregRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:obtenerRegistro#Registro", ReplyAction="*")]
        System.Threading.Tasks.Task<WebServiceLecta.WsLecta.WebServiceEcaIrregResponse> WebServiceEcaIrregAsync(WebServiceLecta.WsLecta.WebServiceEcaIrregRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="urn:WebServiceEcaIrreg")]
    public partial class Registro : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string gUIAField;
        
        private string eSTADOField;
        
        private string cAUSALField;
        
        private string fECHAGESTIONField;
        
        private string rUTAIMAGENField;
        
        /// <remarks/>
        public string GUIA {
            get {
                return this.gUIAField;
            }
            set {
                this.gUIAField = value;
                this.RaisePropertyChanged("GUIA");
            }
        }
        
        /// <remarks/>
        public string ESTADO {
            get {
                return this.eSTADOField;
            }
            set {
                this.eSTADOField = value;
                this.RaisePropertyChanged("ESTADO");
            }
        }
        
        /// <remarks/>
        public string CAUSAL {
            get {
                return this.cAUSALField;
            }
            set {
                this.cAUSALField = value;
                this.RaisePropertyChanged("CAUSAL");
            }
        }
        
        /// <remarks/>
        public string FECHAGESTION {
            get {
                return this.fECHAGESTIONField;
            }
            set {
                this.fECHAGESTIONField = value;
                this.RaisePropertyChanged("FECHAGESTION");
            }
        }
        
        /// <remarks/>
        public string RUTAIMAGEN {
            get {
                return this.rUTAIMAGENField;
            }
            set {
                this.rUTAIMAGENField = value;
                this.RaisePropertyChanged("RUTAIMAGEN");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="WebServiceEcaIrreg", WrapperNamespace="urn:obtenerRegistro", IsWrapped=true)]
    public partial class WebServiceEcaIrregRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string RADICADO;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=1)]
        public string FECHA;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=2)]
        public string DELEGACION;
        
        public WebServiceEcaIrregRequest() {
        }
        
        public WebServiceEcaIrregRequest(string RADICADO, string FECHA, string DELEGACION) {
            this.RADICADO = RADICADO;
            this.FECHA = FECHA;
            this.DELEGACION = DELEGACION;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="WebServiceEcaIrregResponse", WrapperNamespace="urn:obtenerRegistro", IsWrapped=true)]
    public partial class WebServiceEcaIrregResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public WebServiceLecta.WsLecta.Registro @return;
        
        public WebServiceEcaIrregResponse() {
        }
        
        public WebServiceEcaIrregResponse(WebServiceLecta.WsLecta.Registro @return) {
            this.@return = @return;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface WebServiceEcaIrregPortTypeChannel : WebServiceLecta.WsLecta.WebServiceEcaIrregPortType, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WebServiceEcaIrregPortTypeClient : System.ServiceModel.ClientBase<WebServiceLecta.WsLecta.WebServiceEcaIrregPortType>, WebServiceLecta.WsLecta.WebServiceEcaIrregPortType {
        
        public WebServiceEcaIrregPortTypeClient() {
        }
        
        public WebServiceEcaIrregPortTypeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WebServiceEcaIrregPortTypeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WebServiceEcaIrregPortTypeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WebServiceEcaIrregPortTypeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        WebServiceLecta.WsLecta.WebServiceEcaIrregResponse WebServiceLecta.WsLecta.WebServiceEcaIrregPortType.WebServiceEcaIrreg(WebServiceLecta.WsLecta.WebServiceEcaIrregRequest request) {
            return base.Channel.WebServiceEcaIrreg(request);
        }
        
        public WebServiceLecta.WsLecta.Registro WebServiceEcaIrreg(string RADICADO, string FECHA, string DELEGACION) {
            WebServiceLecta.WsLecta.WebServiceEcaIrregRequest inValue = new WebServiceLecta.WsLecta.WebServiceEcaIrregRequest();
            inValue.RADICADO = RADICADO;
            inValue.FECHA = FECHA;
            inValue.DELEGACION = DELEGACION;
            WebServiceLecta.WsLecta.WebServiceEcaIrregResponse retVal = ((WebServiceLecta.WsLecta.WebServiceEcaIrregPortType)(this)).WebServiceEcaIrreg(inValue);
            return retVal.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WebServiceLecta.WsLecta.WebServiceEcaIrregResponse> WebServiceLecta.WsLecta.WebServiceEcaIrregPortType.WebServiceEcaIrregAsync(WebServiceLecta.WsLecta.WebServiceEcaIrregRequest request) {
            return base.Channel.WebServiceEcaIrregAsync(request);
        }
        
        public System.Threading.Tasks.Task<WebServiceLecta.WsLecta.WebServiceEcaIrregResponse> WebServiceEcaIrregAsync(string RADICADO, string FECHA, string DELEGACION) {
            WebServiceLecta.WsLecta.WebServiceEcaIrregRequest inValue = new WebServiceLecta.WsLecta.WebServiceEcaIrregRequest();
            inValue.RADICADO = RADICADO;
            inValue.FECHA = FECHA;
            inValue.DELEGACION = DELEGACION;
            return ((WebServiceLecta.WsLecta.WebServiceEcaIrregPortType)(this)).WebServiceEcaIrregAsync(inValue);
        }
    }
}
