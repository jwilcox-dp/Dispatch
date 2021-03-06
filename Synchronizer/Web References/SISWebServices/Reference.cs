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

namespace DNACircSynchronizer.Processes.SISWebServices {
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
    [System.Web.Services.WebServiceBindingAttribute(Name="ServiceSoap", Namespace="http://tempuri.org/")]
    public partial class Service : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback NewEmailAddressOperationCompleted;
        
        private System.Threading.SendOrPostCallback UpdateEmailAddressOperationCompleted;
        
        private System.Threading.SendOrPostCallback DeleteEmailAddressOperationCompleted;
        
        private System.Threading.SendOrPostCallback CreateComplaintInSISOnlyOperationCompleted;
        
        private System.Threading.SendOrPostCallback CreateMemoInSISOnlyOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public Service() {
            this.Url = global::DNACircSynchronizer.Processes.Properties.Settings.Default.Synchronizer_SISWebServices_Service;
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
        public event NewEmailAddressCompletedEventHandler NewEmailAddressCompleted;
        
        /// <remarks/>
        public event UpdateEmailAddressCompletedEventHandler UpdateEmailAddressCompleted;
        
        /// <remarks/>
        public event DeleteEmailAddressCompletedEventHandler DeleteEmailAddressCompleted;
        
        /// <remarks/>
        public event CreateComplaintInSISOnlyCompletedEventHandler CreateComplaintInSISOnlyCompleted;
        
        /// <remarks/>
        public event CreateMemoInSISOnlyCompletedEventHandler CreateMemoInSISOnlyCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/NewEmailAddress", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string NewEmailAddress(string AccountNumber, string EmailAddress, string AcceptsPromotions, string CanSendEmail) {
            object[] results = this.Invoke("NewEmailAddress", new object[] {
                        AccountNumber,
                        EmailAddress,
                        AcceptsPromotions,
                        CanSendEmail});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void NewEmailAddressAsync(string AccountNumber, string EmailAddress, string AcceptsPromotions, string CanSendEmail) {
            this.NewEmailAddressAsync(AccountNumber, EmailAddress, AcceptsPromotions, CanSendEmail, null);
        }
        
        /// <remarks/>
        public void NewEmailAddressAsync(string AccountNumber, string EmailAddress, string AcceptsPromotions, string CanSendEmail, object userState) {
            if ((this.NewEmailAddressOperationCompleted == null)) {
                this.NewEmailAddressOperationCompleted = new System.Threading.SendOrPostCallback(this.OnNewEmailAddressOperationCompleted);
            }
            this.InvokeAsync("NewEmailAddress", new object[] {
                        AccountNumber,
                        EmailAddress,
                        AcceptsPromotions,
                        CanSendEmail}, this.NewEmailAddressOperationCompleted, userState);
        }
        
        private void OnNewEmailAddressOperationCompleted(object arg) {
            if ((this.NewEmailAddressCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.NewEmailAddressCompleted(this, new NewEmailAddressCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/UpdateEmailAddress", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string UpdateEmailAddress(string AccountNumber, [System.Xml.Serialization.XmlElementAttribute("NewEmailAddress")] string NewEmailAddress1, string OldEmailAddress, string AcceptsPromotions, string CanSendEmail) {
            object[] results = this.Invoke("UpdateEmailAddress", new object[] {
                        AccountNumber,
                        NewEmailAddress1,
                        OldEmailAddress,
                        AcceptsPromotions,
                        CanSendEmail});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void UpdateEmailAddressAsync(string AccountNumber, string NewEmailAddress1, string OldEmailAddress, string AcceptsPromotions, string CanSendEmail) {
            this.UpdateEmailAddressAsync(AccountNumber, NewEmailAddress1, OldEmailAddress, AcceptsPromotions, CanSendEmail, null);
        }
        
        /// <remarks/>
        public void UpdateEmailAddressAsync(string AccountNumber, string NewEmailAddress1, string OldEmailAddress, string AcceptsPromotions, string CanSendEmail, object userState) {
            if ((this.UpdateEmailAddressOperationCompleted == null)) {
                this.UpdateEmailAddressOperationCompleted = new System.Threading.SendOrPostCallback(this.OnUpdateEmailAddressOperationCompleted);
            }
            this.InvokeAsync("UpdateEmailAddress", new object[] {
                        AccountNumber,
                        NewEmailAddress1,
                        OldEmailAddress,
                        AcceptsPromotions,
                        CanSendEmail}, this.UpdateEmailAddressOperationCompleted, userState);
        }
        
        private void OnUpdateEmailAddressOperationCompleted(object arg) {
            if ((this.UpdateEmailAddressCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.UpdateEmailAddressCompleted(this, new UpdateEmailAddressCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/DeleteEmailAddress", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string DeleteEmailAddress(string AccountNumber, string EmailAddress) {
            object[] results = this.Invoke("DeleteEmailAddress", new object[] {
                        AccountNumber,
                        EmailAddress});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void DeleteEmailAddressAsync(string AccountNumber, string EmailAddress) {
            this.DeleteEmailAddressAsync(AccountNumber, EmailAddress, null);
        }
        
        /// <remarks/>
        public void DeleteEmailAddressAsync(string AccountNumber, string EmailAddress, object userState) {
            if ((this.DeleteEmailAddressOperationCompleted == null)) {
                this.DeleteEmailAddressOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDeleteEmailAddressOperationCompleted);
            }
            this.InvokeAsync("DeleteEmailAddress", new object[] {
                        AccountNumber,
                        EmailAddress}, this.DeleteEmailAddressOperationCompleted, userState);
        }
        
        private void OnDeleteEmailAddressOperationCompleted(object arg) {
            if ((this.DeleteEmailAddressCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DeleteEmailAddressCompleted(this, new DeleteEmailAddressCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CreateComplaintInSISOnly", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string CreateComplaintInSISOnly(bool inChargeCarrier, string inComment, string inComplaintCode, int inCreditDays, System.DateTime inDateEntered, System.DateTime inEffectiveDate, string inAddressKey, string inPubCode, string inEscalate, string inSubscriptionNumber) {
            object[] results = this.Invoke("CreateComplaintInSISOnly", new object[] {
                        inChargeCarrier,
                        inComment,
                        inComplaintCode,
                        inCreditDays,
                        inDateEntered,
                        inEffectiveDate,
                        inAddressKey,
                        inPubCode,
                        inEscalate,
                        inSubscriptionNumber});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void CreateComplaintInSISOnlyAsync(bool inChargeCarrier, string inComment, string inComplaintCode, int inCreditDays, System.DateTime inDateEntered, System.DateTime inEffectiveDate, string inAddressKey, string inPubCode, string inEscalate, string inSubscriptionNumber) {
            this.CreateComplaintInSISOnlyAsync(inChargeCarrier, inComment, inComplaintCode, inCreditDays, inDateEntered, inEffectiveDate, inAddressKey, inPubCode, inEscalate, inSubscriptionNumber, null);
        }
        
        /// <remarks/>
        public void CreateComplaintInSISOnlyAsync(bool inChargeCarrier, string inComment, string inComplaintCode, int inCreditDays, System.DateTime inDateEntered, System.DateTime inEffectiveDate, string inAddressKey, string inPubCode, string inEscalate, string inSubscriptionNumber, object userState) {
            if ((this.CreateComplaintInSISOnlyOperationCompleted == null)) {
                this.CreateComplaintInSISOnlyOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCreateComplaintInSISOnlyOperationCompleted);
            }
            this.InvokeAsync("CreateComplaintInSISOnly", new object[] {
                        inChargeCarrier,
                        inComment,
                        inComplaintCode,
                        inCreditDays,
                        inDateEntered,
                        inEffectiveDate,
                        inAddressKey,
                        inPubCode,
                        inEscalate,
                        inSubscriptionNumber}, this.CreateComplaintInSISOnlyOperationCompleted, userState);
        }
        
        private void OnCreateComplaintInSISOnlyOperationCompleted(object arg) {
            if ((this.CreateComplaintInSISOnlyCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CreateComplaintInSISOnlyCompleted(this, new CreateComplaintInSISOnlyCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CreateMemoInSISOnly", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string CreateMemoInSISOnly(string inMemoText, bool inDispatchFlag, bool inSendToCarrierFlag, bool inSendToDMFlag, string inSOSEscalateFlag, System.DateTime inDateEntered, System.DateTime inEffectiveDate, string inAddressKey, string inOperatorInitials) {
            object[] results = this.Invoke("CreateMemoInSISOnly", new object[] {
                        inMemoText,
                        inDispatchFlag,
                        inSendToCarrierFlag,
                        inSendToDMFlag,
                        inSOSEscalateFlag,
                        inDateEntered,
                        inEffectiveDate,
                        inAddressKey,
                        inOperatorInitials});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void CreateMemoInSISOnlyAsync(string inMemoText, bool inDispatchFlag, bool inSendToCarrierFlag, bool inSendToDMFlag, string inSOSEscalateFlag, System.DateTime inDateEntered, System.DateTime inEffectiveDate, string inAddressKey, string inOperatorInitials) {
            this.CreateMemoInSISOnlyAsync(inMemoText, inDispatchFlag, inSendToCarrierFlag, inSendToDMFlag, inSOSEscalateFlag, inDateEntered, inEffectiveDate, inAddressKey, inOperatorInitials, null);
        }
        
        /// <remarks/>
        public void CreateMemoInSISOnlyAsync(string inMemoText, bool inDispatchFlag, bool inSendToCarrierFlag, bool inSendToDMFlag, string inSOSEscalateFlag, System.DateTime inDateEntered, System.DateTime inEffectiveDate, string inAddressKey, string inOperatorInitials, object userState) {
            if ((this.CreateMemoInSISOnlyOperationCompleted == null)) {
                this.CreateMemoInSISOnlyOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCreateMemoInSISOnlyOperationCompleted);
            }
            this.InvokeAsync("CreateMemoInSISOnly", new object[] {
                        inMemoText,
                        inDispatchFlag,
                        inSendToCarrierFlag,
                        inSendToDMFlag,
                        inSOSEscalateFlag,
                        inDateEntered,
                        inEffectiveDate,
                        inAddressKey,
                        inOperatorInitials}, this.CreateMemoInSISOnlyOperationCompleted, userState);
        }
        
        private void OnCreateMemoInSISOnlyOperationCompleted(object arg) {
            if ((this.CreateMemoInSISOnlyCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CreateMemoInSISOnlyCompleted(this, new CreateMemoInSISOnlyCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void NewEmailAddressCompletedEventHandler(object sender, NewEmailAddressCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class NewEmailAddressCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal NewEmailAddressCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void UpdateEmailAddressCompletedEventHandler(object sender, UpdateEmailAddressCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class UpdateEmailAddressCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal UpdateEmailAddressCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void DeleteEmailAddressCompletedEventHandler(object sender, DeleteEmailAddressCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DeleteEmailAddressCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal DeleteEmailAddressCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void CreateComplaintInSISOnlyCompletedEventHandler(object sender, CreateComplaintInSISOnlyCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CreateComplaintInSISOnlyCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CreateComplaintInSISOnlyCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void CreateMemoInSISOnlyCompletedEventHandler(object sender, CreateMemoInSISOnlyCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CreateMemoInSISOnlyCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CreateMemoInSISOnlyCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591