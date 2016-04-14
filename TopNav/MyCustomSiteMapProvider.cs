using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Navigation;
using Microsoft.SharePoint.Publishing;
using Microsoft.SharePoint.Publishing.Navigation;
using System;
using System.Web;

namespace TopNav
{
    public class MyCustomSiteMapProvider : SPNavigationProvider
    {

       
        public override SiteMapNodeCollection GetChildNodes(SiteMapNode node)
        {
            ULSLog2013.LogMessage("Child Nodes was called");
   



                    SiteMapNodeCollection test = new SiteMapNodeCollection();
                   
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            try
                            {
                                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                                {
                                    using (SPWeb web = site.RootWeb)
                                    {
                                        SPList nav = web.GetList("/Lists/Navigation");
                                        
                                        ULSLog2013.LogMessage("Found our list:" + nav.Title);
                                        SPQuery sPQuery = new SPQuery();
                                        sPQuery.Query = "<OrderBy><FieldRef Name='LinkOrder' Ascending='True' /><FieldRef Name='Title' Ascending='True' /></OrderBy><Where><IsNull><FieldRef Name='ParentKey' /></IsNull></Where>";
                                        SPListItemCollection items = nav.GetItems(sPQuery);

                                        foreach (SPListItem item in items)
                                        {
                                            SiteMapNode smNode = new SiteMapNode(this, item["Key"].ToString(), item["LinkURL"].ToString(), item["Title"].ToString());
                                            test.Add(smNode);

                                            string str = item["Key"].ToString();
                                            SetChildNodes(str, smNode, nav);
                                           

                                        }
                                        ULSLog2013.LogMessage("We got this many items:" + items.Count.ToString());
                                       

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ULSLog2013.LogError(ex, "finding list");
                            }
                        });
                   
                    return test;

                   
                
        }

        private void SetChildNodes(string str, SiteMapNode smNode, SPList nav)
        {

            SPQuery sPQuery = new SPQuery();
            sPQuery.Query = "<OrderBy><FieldRef Name='LinkOrder' Ascending='True' /><FieldRef Name='Title' Ascending='True' /></OrderBy><Where><Eq><FieldRef Name=\"ParentKey\" /><Value Type=\"Text\">" + str + "</Value></Eq></Where>";
            SPListItemCollection itemschild = nav.GetItems(sPQuery);

            if (itemschild.Count > 0)
            {
                SiteMapNodeCollection test12 = new SiteMapNodeCollection();
                foreach (SPListItem itemchild in itemschild)
                {
                    SiteMapNode smNode2 = new SiteMapNode(this, itemchild["Key"].ToString(), itemchild["LinkURL"].ToString(), itemchild["Title"].ToString());

                    sPQuery = new SPQuery();
                    string childitem = itemchild["Key"].ToString();
                    sPQuery.Query = "<OrderBy><FieldRef Name='Title' Ascending='True' /></OrderBy><Where><Eq><FieldRef Name=\"ParentKey\" /><Value Type=\"Text\">" + childitem + "</Value></Eq></Where>";
                    SPListItemCollection itemschildtest = nav.GetItems(sPQuery);
                    if (itemschildtest.Count > 0)
                        SetChildNodes(childitem, smNode2, nav);
                    else
                        smNode2.ChildNodes = null;

                    test12.Add(smNode2);

                }
                smNode.ChildNodes = test12;

               
            }
            else
            {
                ULSLog2013.LogMessage("smNode to null");
                smNode.ChildNodes = null;



            }
        }

        

        private string GetSiteName()
        {
            string listName = "http://win-3qnlgcnit6m:95";
            ULSLog2013.LogMessage("GetSiteName was called");

            SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    try {
                        SPFarm farm = SPFarm.Local;

                        if (farm.Properties.ContainsKey("SiteName"))
                        {
                            listName = farm.Properties["SiteName"].ToString();
                            ULSLog2013.LogMessage("Farm key is present lets update");
                        }
                        else
                            ULSLog2013.LogMessage("Site name key is not present use default");
                    }
                    catch (Exception ex)
                        {
                            ULSLog2013.LogError(ex, "getSiteName");
                    }
                }
                
                );
            ULSLog2013.LogMessage("Site Name has been set to :" + listName);
            return listName;
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection attributes)
        {
            ULSLog2013.LogMessage("Initialize was called");
            ULSLog2013.LogMessage(name);
           
            foreach (string s in attributes.Keys)
            {
                ULSLog2013.LogMessage(s);
                ULSLog2013.LogMessage(attributes[s].ToString());
            }

          
            base.Initialize(name, attributes);

            //this.GetChildNodes(null);

            

        }
    }
}