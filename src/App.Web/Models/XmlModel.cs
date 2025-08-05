using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Bank
    {

        private BankBranch[] branchField;

        private uint bANK_ROUTING_NBRField;

        private string nAMEField;

        private string sTREET_ADDRESSField;

        private string cITYField;

        private string sTATE_PROVINCEField;

        private string cOUNTRYField;

        private string pOSTAL_ZIP_CODEField;

        private string cLEARING_STATUS_CODEField;

        private string sERVICE_BRANCH_ROUTING_NBRField;

        private string dESIGNATED_BRANCH_ROUTING_NBRField;

        private string nOTEField;

        private byte sPEED_CLEARINGField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Branch")]
        public BankBranch[] Branch
        {
            get
            {
                return this.branchField;
            }
            set
            {
                this.branchField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint BANK_ROUTING_NBR
        {
            get
            {
                return this.bANK_ROUTING_NBRField;
            }
            set
            {
                this.bANK_ROUTING_NBRField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NAME
        {
            get
            {
                return this.nAMEField;
            }
            set
            {
                this.nAMEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string STREET_ADDRESS
        {
            get
            {
                return this.sTREET_ADDRESSField;
            }
            set
            {
                this.sTREET_ADDRESSField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CITY
        {
            get
            {
                return this.cITYField;
            }
            set
            {
                this.cITYField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string STATE_PROVINCE
        {
            get
            {
                return this.sTATE_PROVINCEField;
            }
            set
            {
                this.sTATE_PROVINCEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string COUNTRY
        {
            get
            {
                return this.cOUNTRYField;
            }
            set
            {
                this.cOUNTRYField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string POSTAL_ZIP_CODE
        {
            get
            {
                return this.pOSTAL_ZIP_CODEField;
            }
            set
            {
                this.pOSTAL_ZIP_CODEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CLEARING_STATUS_CODE
        {
            get
            {
                return this.cLEARING_STATUS_CODEField;
            }
            set
            {
                this.cLEARING_STATUS_CODEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SERVICE_BRANCH_ROUTING_NBR
        {
            get
            {
                return this.sERVICE_BRANCH_ROUTING_NBRField;
            }
            set
            {
                this.sERVICE_BRANCH_ROUTING_NBRField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DESIGNATED_BRANCH_ROUTING_NBR
        {
            get
            {
                return this.dESIGNATED_BRANCH_ROUTING_NBRField;
            }
            set
            {
                this.dESIGNATED_BRANCH_ROUTING_NBRField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NOTE
        {
            get
            {
                return this.nOTEField;
            }
            set
            {
                this.nOTEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte SPEED_CLEARING
        {
            get
            {
                return this.sPEED_CLEARINGField;
            }
            set
            {
                this.sPEED_CLEARINGField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class BankBranch
    {

        private uint bRANCH_ROUTING_NBRField;

        private string nAMEField;

        private string sTREET_ADDRESSField;

        private string cITYField;

        private string sTATE_PROVINCEField;

        private string cOUNTRYField;

        private string pOSTAL_ZIP_CODEField;

        private ushort bRANCH_NUMBERField;

        private string nOTEField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint BRANCH_ROUTING_NBR
        {
            get
            {
                return this.bRANCH_ROUTING_NBRField;
            }
            set
            {
                this.bRANCH_ROUTING_NBRField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NAME
        {
            get
            {
                return this.nAMEField;
            }
            set
            {
                this.nAMEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string STREET_ADDRESS
        {
            get
            {
                return this.sTREET_ADDRESSField;
            }
            set
            {
                this.sTREET_ADDRESSField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CITY
        {
            get
            {
                return this.cITYField;
            }
            set
            {
                this.cITYField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string STATE_PROVINCE
        {
            get
            {
                return this.sTATE_PROVINCEField;
            }
            set
            {
                this.sTATE_PROVINCEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string COUNTRY
        {
            get
            {
                return this.cOUNTRYField;
            }
            set
            {
                this.cOUNTRYField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string POSTAL_ZIP_CODE
        {
            get
            {
                return this.pOSTAL_ZIP_CODEField;
            }
            set
            {
                this.pOSTAL_ZIP_CODEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort BRANCH_NUMBER
        {
            get
            {
                return this.bRANCH_NUMBERField;
            }
            set
            {
                this.bRANCH_NUMBERField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NOTE
        {
            get
            {
                return this.nOTEField;
            }
            set
            {
                this.nOTEField = value;
            }
        }
    }


}