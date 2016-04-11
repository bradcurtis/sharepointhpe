using Microsoft.SharePoint;
using Microsoft.SharePoint.Navigation;
using Microsoft.SharePoint.Publishing;
using Microsoft.SharePoint.Publishing.Navigation;
using System.Web;

namespace TopNav
{
    public class MyCustomSiteMapProvider : SPNavigationProvider
    {
        public override SiteMapNodeCollection GetChildNodes(SiteMapNode node)
        {
            ULSLog2013.LogMessage("Child Nodes was called");
           /* PortalSiteMapNode pNode = node as PortalSiteMapNode;
            if (pNode != null)
            {
                ULSLog2013.LogMessage("We currently have portal site node map");

                if (pNode.Type == NodeTypes.Area && pNode.WebId == SPContext.Current.Site.RootWeb.ID)
                {
                    SiteMapNodeCollection nodeColl = base.GetChildNodes(pNode);
                    SiteMapNode childNode = new SiteMapNode(this,
                                                            "&lt;http://www.google.com&gt;", "http://www.google.com",
                                                            "google link");

                    SiteMapNode childNode1 = new SiteMapNode(this,
                                                             "&lt;http://www.msn.com&gt;",
                                                             "http://www.msn.com", "MSN link");

                    nodeColl.Add(childNode);

                    SiteMapNodeCollection test = new SiteMapNodeCollection();
                    test.Add(childNode1);
                    childNode.ChildNodes = test;

                    return nodeColl;
                }
                return base.GetChildNodes(pNode);
            }

            ULSLog2013.LogMessage("We DO NOT have a node map");
            SiteMapNode childNodeNew = new SiteMapNode(this,
                                                            "github", "http://www.github.com",
                                                            "git hub link");

            SiteMapNode childNodeNew1 = new SiteMapNode(this,
                                                             "msn",
                                                             "http://www.msn.com", "MSN link");

           // childNodeNew.ChildNodes.Add(childNodeNew1);
            SiteMapNodeCollection test1 = new SiteMapNodeCollection();
           // childNodeNew.ChildNodes.Add(childNodeNew1);
           // test1.Add(childNodeNew);
           //test1.Add(childNodeNew1);
           // childNodeNew.ChildNodes.Add(childNodeNew1);
            return base.GetChildNodes(childNodeNew);*/


         
                   
                    SiteMapNode childNode = new SiteMapNode(this,
                    "Node1", "http://www.microsoft.com", "Microsoft");

                    SiteMapNode childNode1 = new SiteMapNode(this,
                    "Node11", "http://support.microsoft.com", "Support");

                  

                    SiteMapNodeCollection test = new SiteMapNodeCollection();
                    SiteMapNodeCollection test2 = new SiteMapNodeCollection();
                    SiteMapNodeCollection test3 = new SiteMapNodeCollection();
                    test.Add(childNode);
                    test2.Add(childNode1);
                    childNode.ChildNodes = test2;
                    childNode1.ChildNodes = test3;
                   // base.AddNode(childNode);
                   // base.AddNode(childNode1);
                   // return base.GetChildNodes(childNode);
                    return test;

                   
                
        }

        

        protected override EditableAspMenuState GetMenuState(string startingNodeKey, int maximumDepth)
        {
            ULSLog2013.LogMessage("MS");
            return base.GetMenuState(startingNodeKey, maximumDepth);
        }
        protected override void SaveUpdatedMenuState(EditableAspMenuState newState)
        {
            ULSLog2013.LogMessage("US");
            base.SaveUpdatedMenuState(newState);
        }
        protected override void AddNode(SiteMapNode node, SiteMapNode parentNode)
        {
            ULSLog2013.LogMessage("AN");
            base.AddNode(node, parentNode);
        }

        public override SiteMapNode GetParentNode(SiteMapNode node)
        {
            ULSLog2013.LogMessage("GPN");
            return base.GetParentNode(node);
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