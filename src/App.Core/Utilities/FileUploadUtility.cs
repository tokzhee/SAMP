using System;
using System.Globalization;
using System.IO;
using System.Web;

namespace App.Core.Utilities
{
    public static class FileUploadUtility
    {
        private static readonly string ImagePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        //public static string SaveFile(HttpPostedFileBase fileUpload, string fileUploadPurpose, string callerFormName, string callerFormMethod, string callerIpAddress)
        //{
        //    var savedFileUrl = "";

        //    try
        //    {
        //        //get details of file to upload
        //        var fileInfo = new FileInfo(fileUpload.FileName);
        //        var fileName = fileInfo.Name;
        //        var fileExtension = fileInfo.Extension;
                
        //        //what is the name of the folder where i want to save the file?
        //        const string saveFolder = "FileUploads";
                
        //        //update file name and file path
        //        var saveFileName = $"{fileUploadPurpose.Replace("/", "-").Replace(@"\", "-")}-{DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace("/", "").Replace(" ", "").Replace(":", "")}{fileExtension}";
                
        //        //if folder does not already exist then create folder
        //        var saveDirectory = ImagePath + saveFolder;
        //        if (!Directory.Exists(saveDirectory))
        //        {
        //            Directory.CreateDirectory(saveDirectory);
        //        }
        //        var savePath = $"{saveDirectory}/{saveFileName}";
                
        //        //save file and return
        //        fileUpload.SaveAs(savePath);
        //        savedFileUrl = $"~/{ saveFolder}/{saveFileName}";
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
        //    }

        //    return savedFileUrl;
        //}
    }
}
