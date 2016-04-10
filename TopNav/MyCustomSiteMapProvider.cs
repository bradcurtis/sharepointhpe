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
            ULSLog2013.LogMessage("I was called");
            PortalSiteMapNode pNode = node as PortalSiteMapNode;
            if (pNode != null)
            {
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
            SiteMapNode childNodeNew = new SiteMapNode(this,
                                                            "&lt;http://www.helpmeonsharepoint.com&gt;", "http://www.helpmeonsharepoint.com",
                                                            "Help Me On SharePoint");

            return new SiteMapNodeCollection(childNodeNew);
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
            ULSLog2013.LogMessage("I was Called");
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