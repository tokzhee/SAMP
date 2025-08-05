using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace App.Core.Utilities
{
    public class XmlSerializerUtility
    {
        public T Deserialize<T>(string input, string callerFormName, string callerFormMethod, string callerIpAddress) where T : class
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                using (var stringReader = new StringReader(input))
                {
                    return (T)xmlSerializer.Deserialize(stringReader);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return null;
        }
        public string Serialize<T>(T objectToSerialize, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(objectToSerialize.GetType());
                using (var stringWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(stringWriter, objectToSerialize);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return "";
        }
    }
}
