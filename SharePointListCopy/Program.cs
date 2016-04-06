using System;
using System.Net;
using Microsoft.SharePoint.Client;
using log4net;
using log4net.Config;
using System.Linq;
using System.Data;
using ClosedXML.Excel;
using System.Text;
using System.Configuration;
using System.Data.OleDb;
using Excel;
using System.IO;

namespace SharePointListCopy
{
    /// <summary>
    /// SharePoint list copy will allow the backup of the current pipeline list
    /// 
    /// Assumptions:
    ///   We do not have access to the sharepoint server so we need to use client context
    ///   We are using a console app so we can can add in a scheduled event
    /// </summary>
    class Program
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        
       // private static object yesnoarray;

        /// <summary>
        /// entry point into the backup list program
        /// </summary>
        /// <param name="args">Not really used</param>
        static void Main(string[] args)
        {
            try
            {
                string clientContextWeb = ConfigurationManager.AppSettings["clientContextWeb"] ?? @"https://bi.sharepoint.hpe.com/teams/USPS_CandI_CapturePortal/";
                string backupListTarget = ConfigurationManager.AppSettings["backupListTarget"] ?? @"BradTestPipeline";
                string backupListSource = ConfigurationManager.AppSettings["backupListSource"] ?? @"Pipeline";
                string pipelineBackupDocLib = ConfigurationManager.AppSettings["pipelineBackupDocLib"] ?? @"PipelineBackup";
                string updateList = ConfigurationManager.AppSettings["updateList"] ?? @"PipelineMirror";


                log.Debug(string.Format("context web:{0}  target backup:{1}  source backup {2}", clientContextWeb, backupListTarget, backupListSource));

                Console.WriteLine("Would you like to run backup (1) or List Update (2) or Both(3)");
                int itest =  int.Parse(Console.ReadLine());

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Welcome to the C&I capture utility");
                Console.WriteLine("Sit back and grab some coffee");

                // UpdatePipelineList updatelist = new UpdatePipelineList(clientContextWeb, backupListTarget, backupListSource, pipelineBackupDocLib);
                if (itest == 1 || itest == 3)
                {
                    BackupHelper helper = new BackupHelper(clientContextWeb, backupListTarget, backupListSource, pipelineBackupDocLib);
                }
                 if (itest==2 || itest == 3)
                {
                    UpdatePipelineList updatelist = new UpdatePipelineList(clientContextWeb, updateList);
                }





            
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                log.Error("Exception in main", ex);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("We had an issue:{0}", ex.ToString()));
            }
        }

      
    }
}
