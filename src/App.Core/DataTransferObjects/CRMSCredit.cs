using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace App.Core.DataTransferObjects
{
	[XmlRoot(ElementName = "Credit")]
	public class CRMSCredit
	{

		[XmlElement(ElementName = "CRMSRefNumber")]
		public string CRMSRefNumber { get; set; }

		[XmlElement(ElementName = "CreditType")]
		public string CreditType { get; set; }

		[XmlElement(ElementName = "CreditLimit")]
		public string CreditLimit { get; set; }

		[XmlElement(ElementName = "OutstandingAmount")]
		public string OutstandingAmount { get; set; }

		[XmlElement(ElementName = "EffectiveDate")]
		public string EffectiveDate { get; set; }

		[XmlElement(ElementName = "Tenor")]
		public string Tenor { get; set; }

		[XmlElement(ElementName = "ExpiryDate")]
		public string ExpiryDate { get; set; }

		[XmlElement(ElementName = "GrantingInstitution")]
		public string GrantingInstitution { get; set; }

		[XmlElement(ElementName = "PerformanceStatus")]
		public string PerformanceStatus { get; set; }
	}
}
