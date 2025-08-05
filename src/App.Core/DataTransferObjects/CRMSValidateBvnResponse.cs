using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace App.Core.DataTransferObjects
{
	[XmlRoot(ElementName = "BVNRecord")]
	public class CRMSValidateBvnResponse
	{

		[XmlElement(ElementName = "ResponseCode")]
		public string ResponseCode { get; set; }

		[XmlElement(ElementName = "BVN")]
		public string BVN { get; set; }

		[XmlElement(ElementName = "FirstName")]
		public string FirstName { get; set; }

		[XmlElement(ElementName = "MiddleName")]
		public string MiddleName { get; set; }

		[XmlElement(ElementName = "LastName")]
		public string LastName { get; set; }

		[XmlElement(ElementName = "DateOfBirth")]
		public string DateOfBirth { get; set; }

		[XmlElement(ElementName = "PhoneNumber1")]
		public string PhoneNumber1 { get; set; }

		[XmlElement(ElementName = "PhoneNumber2")]
		public string PhoneNumber2 { get; set; }

		[XmlElement(ElementName = "Gender")]
		public string Gender { get; set; }

		[XmlElement(ElementName = "ResidentialAddress")]
		public string ResidentialAddress { get; set; }
	}

}
