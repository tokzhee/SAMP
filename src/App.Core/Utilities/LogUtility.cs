using log4net;
using System;

namespace App.Core.Utilities
{
    public static class LogUtility
    {
        public static void LogInfo(string callerFormName, string callerFormMethod, string callerIpAddress,
            string infoMessage)
        {
            var myLog = LogManager.GetLogger(callerFormName + "|" + callerFormMethod + "|" + callerIpAddress);
            myLog.Info(infoMessage);
            myLog.Info("==================================================================================");

        }

        public static void LogWarning(string callerFormName, string callerFormMethod, string callerIpAddress,
            string waningMessage)
        {
            var myLog = LogManager.GetLogger(callerFormName + "|" + callerFormMethod + "|" + callerIpAddress);
            myLog.Warn(waningMessage);
            myLog.Warn("==================================================================================");
        }

        public static void LogError(string callerFormName, string callerFormMethod, string callerIpAddress,
            Exception ex)
        {
            var myLog = LogManager.GetLogger(callerFormName + "|" + callerFormMethod + "|" + callerIpAddress);
            myLog.Debug("Message: " + ex.Message + "|StackTrace: " + ex.StackTrace);
            myLog.Debug("==================================================================================");
        }

        public static void LogError(string callerFormName, string callerFormMethod, string callerIpAddress,
            string errorMessage)
        {
            var myLog = LogManager.GetLogger(callerFormName + "|" + callerFormMethod + "|" + callerIpAddress);
            myLog.Debug(errorMessage);
            myLog.Debug("==================================================================================");
        }
    }
}