using Excel;
using log4net;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SharePointListCopy
{
    public class UpdatePipelineList
    {
        private static readonly string[] yesnoarray = new string[] { "won", "closed" };
        private static readonly ILog log = LogManager.GetLogger(typeof(UpdatePipelineList));

        private int countupdate = 0;
        private int countnew = 0;
        public UpdatePipelineList(string clientContextWeb, string backupListTarget)
        {

           
            System.IO.Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Export\");

            string folder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Export\";
            string filter = "*.xlsx";
            string[] files = Directory.GetFiles(folder, filter);

            string pipelinefile = "Pipeline.xlsx";
            string dhcfile = "DHCUpdate.xlsx";

            Regex regexPipeline = FindFilesPatternToRegex.Convert("*pipeline*.xlsx");
            Regex regexDHC = FindFilesPatternToRegex.Convert("*dhc*.xlsx");

            foreach (string file in files)
            {
                if (regexPipeline.IsMatch(file.ToLower()))
                    pipelinefile = file;
                else if (regexDHC.IsMatch(file.ToLower()))
                    dhcfile = file;
            }
            FileStream stream, streamDHC;

            try
            {
                //update for reading files
                 stream = System.IO.File.Open(pipelinefile, FileMode.Open, FileAccess.Read);

                //update for reading files
                 streamDHC = System.IO.File.Open(dhcfile, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("Please close the excel file and press enter");
                Console.ReadLine();
                //update for reading files
                 stream = System.IO.File.Open(pipelinefile, FileMode.Open, FileAccess.Read);

                //update for reading files
                 streamDHC = System.IO.File.Open(dhcfile, FileMode.Open, FileAccess.Read);

            }



            IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            reader.IsFirstRowAsColumnNames = true;

            DataSet ds = reader.AsDataSet();

            IExcelDataReader readerDHC = ExcelReaderFactory.CreateOpenXmlReader(streamDHC);
            readerDHC.IsFirstRowAsColumnNames = true;

            DataSet dsDHC = readerDHC.AsDataSet();

            DataRowSharepointMappingCollection mapping = MyRetriever.GetTheCollection();
            DataRowSharepointMappingCollection mappingDHC = MyRetriever.GetTheCollection("DataRowDHCMappingsSection");



            DataTable dt = ds.Tables[0];
            DataColumn dcParent = dt.Columns["Opportunity Name"];

            using (var clientContext = new ClientContext(clientContextWeb))
            {
                Web web = clientContext.Web;


                List oldList = web.Lists.GetByTitle(backupListTarget);
                CamlQuery query = CamlQuery.CreateAllItemsQuery(2000);
                ListItemCollection oldItems = oldList.GetItems(query);

                clientContext.Load(oldItems);

                var listFields = oldList.Fields;
                clientContext.Load(listFields, fields => fields.Include(field => field.Title, field => field.InternalName, field => field.ReadOnlyField));


                clientContext.ExecuteQuery();



                /*  foreach (Field f in listFields)
                  {
                      log.Debug(f.InternalName);
                  }*/


                foreach (DataRow dr in ds.Tables[0].Rows)
                {



                    var page = from ListItem itemlist in oldItems.ToList()
                               where itemlist["HPOppID"].ToString() == dr["HPE Opportunity Id"].ToString()
                               select itemlist;

                    //this is an update
                    if (page.Count() == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine(string.Format("Name:{0}  ID:{1}", dr["Opportunity Name"].ToString(), dr["HPE Opportunity Id"].ToString()));

                        ListItem item = page.FirstOrDefault();

                        //iterate the mapping between sharepoint list items and excel spreadsheet items
                        foreach (DataRowSharepointMapping map in mapping)
                        {
                            UpdateField(item, map.SharePointColumn, map.DataRowColumn, dr);


                        }
                        CompareSalesStage(item, dsDHC, mappingDHC);

                        // just update the item
                        item.Update();
                        //update the list
                        oldList.Update();

                        countupdate++;


                    }
                    // This is a new record
                    else if (page.Count() == 0 && !string.IsNullOrEmpty(dr["HPE Opportunity Id"].ToString()))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(string.Format("Name:{0}  ID:{1}", dr["Opportunity Name"].ToString(), dr["HPE Opportunity Id"].ToString()));
                        
                        ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                        ListItem oListItem = oldList.AddItem(itemCreateInfo);
                        //iterate the mapping between sharepoint list items and excel spreadsheet items
                        foreach (DataRowSharepointMapping map in mapping)
                        {
                            UpdateField(oListItem, map.SharePointColumn, map.DataRowColumn, dr);


                        }
                        CompareSalesStage(oListItem, dsDHC, mappingDHC);

                        // just update the item
                        oListItem.Update();
                        //update the list
                        oldList.Update();

                        countnew++;

                    }

                    else
                    {

                        //Console.ForegroundColor = ConsoleColor.Red;
                        //Console.WriteLine("ERROR");

                    }

                    clientContext.ExecuteQuery();





                }
                Console.ForegroundColor = ConsoleColor.Green;
               Console.WriteLine(string.Format("We updated: {0} records and we added {1} records",countupdate.ToString(),countnew.ToString()));
            }
        }

        public static void UpdateField(ListItem item, string SharePointColumn, string DataRowColumn, DataRow dr)
        {
            if (dr.Table.Columns.Contains(DataRowColumn) && !string.IsNullOrEmpty(dr[DataRowColumn].ToString()))
            {
                if (yesnoarray.Contains(SharePointColumn.ToLower()))
                {
                    if (dr[DataRowColumn].ToString() == "0")
                        item[SharePointColumn] = "No";
                    else
                        item[SharePointColumn] = "Yes";
                }
                else
                    item[SharePointColumn] = dr[DataRowColumn];

            }
        }

        public static void CompareSalesStage( ListItem dr, DataSet dsDHC, DataRowSharepointMappingCollection mappingDHC)
        {
            //query sharepoint for the record we are updateing by hpoppid
            var result = (from myRow in dsDHC.Tables[0].AsEnumerable()
                          where myRow["HPE Opportunity Id"].ToString() == dr["HPOppID"].ToString()
                          select myRow).FirstOrDefault(); 
            foreach (DataRowSharepointMapping map in mappingDHC)
            {
                if (result != null && result[map.DataRowColumn]!= null)
                { 
                    dr[map.SharePointColumn] = result[map.DataRowColumn];
                }
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(string.Format("Updating DHC Name:{0}  ID:{1}", dr["Title"].ToString(), dr["HPOppID"].ToString())); ;
        }

    }

    internal static class FindFilesPatternToRegex
    {
        private static Regex HasQuestionMarkRegEx = new Regex(@"\?", RegexOptions.Compiled);
        private static Regex IllegalCharactersRegex = new Regex("[" + @"\/:<>|" + "\"]", RegexOptions.Compiled);
        private static Regex CatchExtentionRegex = new Regex(@"^\s*.+\.([^\.]+)\s*$", RegexOptions.Compiled);
        private static string NonDotCharacters = @"[^.]*";
        public static Regex Convert(string pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException();
            }
            pattern = pattern.Trim();
            if (pattern.Length == 0)
            {
                throw new ArgumentException("Pattern is empty.");
            }
            if (IllegalCharactersRegex.IsMatch(pattern))
            {
                throw new ArgumentException("Pattern contains illegal characters.");
            }
            bool hasExtension = CatchExtentionRegex.IsMatch(pattern);
            bool matchExact = false;
            if (HasQuestionMarkRegEx.IsMatch(pattern))
            {
                matchExact = true;
            }
            else if (hasExtension)
            {
                matchExact = CatchExtentionRegex.Match(pattern).Groups[1].Length != 3;
            }
            string regexString = Regex.Escape(pattern);
            regexString = "^" + Regex.Replace(regexString, @"\\\*", ".*");
            regexString = Regex.Replace(regexString, @"\\\?", ".");
            if (!matchExact && hasExtension)
            {
                regexString += NonDotCharacters;
            }
            regexString += "$";
            Regex regex = new Regex(regexString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return regex;
        }
    }

}
