
using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
 
namespace TopNav
{
    /// <summary>
    /// Used for logging into Uls in 2010
    /// </summary>
    public class ULSLog2013 : SPDiagnosticsServiceBase
    {
        public const string PRODUCT_NAME = "SharePointCustomSolution";
        private static ULSLog2013 _Current;

        public static ULSLog2013 Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new ULSLog2013();
                }
                return _Current;
            }
        }

        private ULSLog2013()
            : base(PRODUCT_NAME, SPFarm.Local)
        {
        }
 
        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>        
            {            
                new SPDiagnosticsArea(PRODUCT_NAME, new List<SPDiagnosticsCategory>            
                {                
                    new SPDiagnosticsCategory("Error", TraceSeverity.High, EventSeverity.Error),
                    new SPDiagnosticsCategory("Warning", TraceSeverity.Medium, EventSeverity.Warning),
                    new SPDiagnosticsCategory("Logging", TraceSeverity.Verbose, EventSeverity.Verbose),
                    new SPDiagnosticsCategory("Debugging", TraceSeverity.Verbose, EventSeverity.Verbose)
                })        
            };
            return areas;
        }
 
        private string MapTraceSeverity(TraceSeverity traceSeverity)
        {
            switch (traceSeverity)
            {
                case TraceSeverity.High: return "Error";
 
                case TraceSeverity.Medium: return "Warning";
 
                default:
                case TraceSeverity.Verbose:
                    return "Debugging";
            }
        }
 
        public static void Log(TraceSeverity traceSeverity, Exception ex)
        {
            SPDiagnosticsCategory category = ULSLog2013.Current.Areas[PRODUCT_NAME].Categories["Error"];
            ULSLog2013.Current.WriteTrace(0, category, TraceSeverity.High, ex.Message);
            ULSLog2013.Current.WriteTrace(0, category, TraceSeverity.High, ex.ToString());
        }
 
        public static void Log(TraceSeverity traceSeverity, string message, Exception ex)
        {
            SPDiagnosticsCategory category = ULSLog2013.Current.Areas[PRODUCT_NAME].Categories["Error"];
            ULSLog2013.Current.WriteTrace(0, category, TraceSeverity.High, ex.Message);
            ULSLog2013.Current.WriteTrace(0, category, TraceSeverity.High, ex.ToString());
        }
 
        public static void LogError(Exception ex)
        {
            SPDiagnosticsCategory category = ULSLog2013.Current.Areas[PRODUCT_NAME].Categories["Error"];
            ULSLog2013.Current.WriteTrace(0, category, TraceSeverity.High, ex.Message);
            ULSLog2013.Current.WriteTrace(0, category, TraceSeverity.High, ex.ToString());
        }
 
        public static void LogError(Exception ex, string message)
        {
            SPDiagnosticsCategory category = ULSLog2013.Current.Areas[PRODUCT_NAME].Categories["Error"];
            ULSLog2013.Current.WriteTrace(0, category, TraceSeverity.High, ex.Message);
            ULSLog2013.Current.WriteTrace(0, category, TraceSeverity.High, ex.ToString());
        }
 
        public static void LogError(string message, string stackTrace)
        {
            SPDiagnosticsCategory category = ULSLog2013.Current.Areas[PRODUCT_NAME].Categories["Error"];
            ULSLog2013.Current.WriteTrace(0, category, TraceSeverity.High, message);
        }
 
        public static void LogWarning(string message)
        {
            SPDiagnosticsCategory category = ULSLog2013.Current.Areas[PRODUCT_NAME].Categories["Warning"];
            ULSLog2013.Current.WriteTrace(1, category, TraceSeverity.Medium, message);
        }
 
        public static void LogMessage(string message)
        {
            SPDiagnosticsCategory category = ULSLog2013.Current.Areas[PRODUCT_NAME].Categories["Logging"];
            ULSLog2013.Current.WriteTrace(1, category, TraceSeverity.Verbose, message);
        }
 
        public static void LogDebug(string message)
        {
            SPDiagnosticsCategory category = ULSLog2013.Current.Areas[PRODUCT_NAME].Categories["Debugging"];
            ULSLog2013.Current.WriteTrace(1, category, TraceSeverity.Verbose, message);
        }
 
    }
}