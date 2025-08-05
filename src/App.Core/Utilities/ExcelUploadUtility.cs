using Excel;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;

namespace App.Core.Utilities
{
    public static class ExcelUploadUtility
    {
        private static readonly string ImagePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        private static DataSet ReadExcelFile(string filePath)
        {
            IExcelDataReader excelReader;

            var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            var myFile = filePath.Split('.');
            var myData = myFile[myFile.GetUpperBound(0)];

            DataSet result = null;

            if (myData.ToUpper() == "XLS")
            {
                excelReader = ExcelReaderFactory.CreateBinaryReader(fileStream);
                result = excelReader.AsDataSet();
                excelReader.IsFirstRowAsColumnNames = true;
                excelReader.Close();
            }
            else if (myData.ToUpper() == "XLSX")
            {
                //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                result = excelReader.AsDataSet();
                excelReader.IsFirstRowAsColumnNames = true;
                excelReader.Close();
            }
            
            return result;
        }

        public static DataSet Read(HttpPostedFileBase fileUpload, string fileUploadPurpose, ref string savedFileUrl, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            DataSet result = null;

            try
            {
                //get details of file to upload
                var fileInfo = new FileInfo(fileUpload.FileName);
                var fileName = fileInfo.Name;
                var fileExtension = fileInfo.Extension;

                //what is the name of the folder where i want to save the file?
                const string saveFolder = "ExcelUploads";

                //update file name and file path
                var saveFileName = $"{fileUploadPurpose.Replace("/", "-").Replace(@"\", "-")}-{DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace("/", "").Replace(" ", "").Replace(":", "")}{fileExtension}";

                //if folder does not already exist then create folder
                var saveDirectory = ImagePath + saveFolder;
                if (!Directory.Exists(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }
                var savePath = $"{saveDirectory}/{saveFileName}";

                //save file and return
                fileUpload.SaveAs(savePath);
                result = ReadExcelFile(savePath);
                savedFileUrl = $"{saveDirectory}/{saveFileName}";
                
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }

    }
}
