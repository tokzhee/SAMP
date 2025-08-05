using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace App.Core.DataTransferObjects
{
    [XmlRoot(ElementName = "CreditCheck")]
    public class CRMSCreditCheckResponse
    {

        [XmlElement(ElementName = "Credit")]
        public List<CRMSCredit> CRMSCredits { get; set; }

        [XmlElement(ElementName = "Summary")]
        public string Summary { get; set; }
    }
}
