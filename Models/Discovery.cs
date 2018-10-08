﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18046
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.0.30319.17929.
// 

using System.Xml.Serialization;

namespace inReachWebRebuild.Models {


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType=true)]
    [XmlRoot("wopi-discovery", Namespace="", IsNullable=false)]
    public partial class wopidiscovery {
        
        private wopidiscoveryNetzone[] netzoneField;
        
        private wopidiscoveryProofkey proofkeyField;
        
        /// <remarks/>
        [XmlElement("net-zone")]
        public wopidiscoveryNetzone[] netzone {
            get {
                return this.netzoneField;
            }
            set {
                this.netzoneField = value;
            }
        }
        
        /// <remarks/>
        [XmlElement("proof-key")]
        public wopidiscoveryProofkey proofkey {
            get {
                return this.proofkeyField;
            }
            set {
                this.proofkeyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType=true)]
    public partial class wopidiscoveryNetzone {
        
        private wopidiscoveryNetzoneApp[] appField;
        
        private string nameField;
        
        /// <remarks/>
        [XmlElement("app")]
        public wopidiscoveryNetzoneApp[] app {
            get {
                return this.appField;
            }
            set {
                this.appField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType=true)]
    public partial class wopidiscoveryNetzoneApp {
        
        private wopidiscoveryNetzoneAppAction[] actionField;
        
        private string nameField;
        
        private string favIconUrlField;
        
        private bool checkLicenseField;
        
        /// <remarks/>
        [XmlElement("action")]
        public wopidiscoveryNetzoneAppAction[] action {
            get {
                return this.actionField;
            }
            set {
                this.actionField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string favIconUrl {
            get {
                return this.favIconUrlField;
            }
            set {
                this.favIconUrlField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public bool checkLicense {
            get {
                return this.checkLicenseField;
            }
            set {
                this.checkLicenseField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType=true)]
    public partial class wopidiscoveryNetzoneAppAction {
        
        private string nameField;
        
        private string extField;
        
        private bool defaultField;
        
        private bool defaultFieldSpecified;
        
        private string urlsrcField;
        
        private string requiresField;
        
        private string progidField;
        
        private bool useParentField;
        
        private bool useParentFieldSpecified;
        
        private string newprogidField;
        
        private string newextField;
        
        /// <remarks/>
        [XmlAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string ext {
            get {
                return this.extField;
            }
            set {
                this.extField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public bool @default {
            get {
                return this.defaultField;
            }
            set {
                this.defaultField = value;
            }
        }
        
        /// <remarks/>
        [XmlIgnore()]
        public bool defaultSpecified {
            get {
                return this.defaultFieldSpecified;
            }
            set {
                this.defaultFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string urlsrc {
            get {
                return this.urlsrcField;
            }
            set {
                this.urlsrcField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string requires {
            get {
                return this.requiresField;
            }
            set {
                this.requiresField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string progid {
            get {
                return this.progidField;
            }
            set {
                this.progidField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public bool useParent {
            get {
                return this.useParentField;
            }
            set {
                this.useParentField = value;
            }
        }
        
        /// <remarks/>
        [XmlIgnore()]
        public bool useParentSpecified {
            get {
                return this.useParentFieldSpecified;
            }
            set {
                this.useParentFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string newprogid {
            get {
                return this.newprogidField;
            }
            set {
                this.newprogidField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string newext {
            get {
                return this.newextField;
            }
            set {
                this.newextField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType=true)]
    public partial class wopidiscoveryProofkey {
        
        private string oldvalueField;
        
        private string valueField;
        
        /// <remarks/>
        [XmlAttribute()]
        public string oldvalue {
            get {
                return this.oldvalueField;
            }
            set {
                this.oldvalueField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
}