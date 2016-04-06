using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace TopNav.TopNav
{
    [ToolboxItemAttribute(false)]
    public class TopNav : WebPart
    {
        public string _MenuList = "Navigation";

        public string _ServerURL = "";

        private char[] fwdSlash = new char[]
		{
			'/'
		};

        public string _MenuOrientation = "Horizontal";

        public string _MenuName = "spNavMenu";

        public string _TopNavigationMenu = "yes";

        public string _MenuPreRenderClientScript = "";

        private bool _NewWindowFieldExists = false;

        [Personalizable, WebBrowsable, WebDisplayName("Menu List")]
        public string Set_MenuList
        {
            get
            {
                return this._MenuList;
            }
            set
            {
                this._MenuList = value;
            }
        }

        [Personalizable, WebBrowsable, WebDisplayName("Server URL")]
        public string Set_ServerURL
        {
            get
            {
                string result;
                if (this._ServerURL.Trim().EndsWith("/"))
                {
                    result = this._ServerURL.Trim().TrimEnd(this.fwdSlash);
                }
                else
                {
                    result = this._ServerURL;
                }
                return result;
            }
            set
            {
                this._ServerURL = value;
            }
        }

        [Personalizable, WebBrowsable, WebDisplayName("Menu Orientation (Horizontal or Vertical")]
        public string Set_MenuOrientation
        {
            get
            {
                return this._MenuOrientation;
            }
            set
            {
                this._MenuOrientation = value;
            }
        }

        [Personalizable, WebBrowsable, WebDisplayName("Menu Name")]
        public string Set_MenuName
        {
            get
            {
                return this._MenuName;
            }
            set
            {
                this._MenuName = value;
            }
        }

        [Personalizable, WebBrowsable, WebDisplayName("Top Navigation Menu? (yes or no)")]
        public string Set_TopNavigationMenu
        {
            get
            {
                return this._TopNavigationMenu;
            }
            set
            {
                this._TopNavigationMenu = value;
            }
        }

        [Personalizable, WebBrowsable, WebDisplayName("Menu Client Script")]
        public string Set_MenuPreRenderClientScript
        {
            get
            {
                return this._MenuPreRenderClientScript;
            }
            set
            {
                this._MenuPreRenderClientScript = value;
            }
        }

        public string SetServerURL(string inURL)
        {
            string result;
            if (inURL.ToLower().StartsWith("http"))
            {
                result = inURL;
            }
            else if (inURL.StartsWith("/"))
            {
                result = this.Set_ServerURL + inURL;
            }
            else if (inURL.Length > 0)
            {
                result = this.Set_ServerURL + "/" + inURL;
            }
            else
            {
                result = "";
            }
            return result;
        }

        protected override void CreateChildControls()
        {
            this.ChromeType = PartChromeType.None;
            string text = this._MenuList + "spCustomMenu";
            AspMenu aspMenu = new AspMenu();
            SPWeb sPWeb = null;
            try
            {
                aspMenu.UseSimpleRendering = true;
                aspMenu.CssClass = "spNavigation";
                if (this.Set_TopNavigationMenu.ToLower() != "yes" && this.Set_TopNavigationMenu.ToLower() != "no")
                {
                    this.Set_TopNavigationMenu = "yes";
                }
                if (this.Set_TopNavigationMenu.ToLower() == "yes")
                {
                    this.Set_MenuName = "TopNavigationMenu";
                    this.Set_MenuOrientation = "horizontal";
                }
                aspMenu.ID = this.Set_MenuName;
                aspMenu.EnableViewState = false;
                if (this.Set_MenuOrientation.ToLower() == "horizontal")
                {
                    aspMenu.Orientation = Orientation.Horizontal;
                }
                else
                {
                    aspMenu.Orientation = Orientation.Vertical;
                }
                SPSite sPSite;
                if (this.Set_ServerURL == "" || this.Set_ServerURL == null)
                {
                    sPSite = SPControl.GetContextSite(this.Context);
                    this.Set_ServerURL = sPSite.Url;
                }
                else
                {
                    sPSite = new SPSite(this.Set_ServerURL);
                }
                sPWeb = sPSite.OpenWeb("/");
                SPList sPList = sPWeb.Lists[this.Set_MenuList];
                this._NewWindowFieldExists = sPList.Fields.ContainsField("OpenNewWindow");
                SPQuery sPQuery = new SPQuery();
                MenuItem menuItem = new MenuItem();
                this.Controls.Add(new LiteralControl("made it this far"));
                sPQuery.Query = "<OrderBy><FieldRef Name='LinkOrder' Ascending='True' /><FieldRef Name='Title' Ascending='True' /></OrderBy><Where><And><IsNull><FieldRef Name='ParentMenu' /></IsNull><Eq><FieldRef Name='ShowMenuItem' /><Value Type='Choice'>Yes</Value></Eq></And></Where>";
                SPListItemCollection items = sPList.GetItems(sPQuery);
                //SPListItemCollection items = sPList.GetItems();
                foreach (SPListItem sPListItem in items)
                {
                    if (sPListItem["Link URL"] == null)
                   {
                        menuItem = new MenuItem(sPListItem["Title"].ToString());
                    }
                    /*  else
                     {
                         menuItem = new MenuItem(sPListItem["Title"].ToString(), "", "", this.SetServerURL(sPListItem["Link URL"].ToString()));
                     }*/
                   // this.GetListItems(sPListItem["ID"].ToString(), menuItem, sPList);
                    aspMenu.Items.Add(menuItem);
                }
                this.Controls.Add(aspMenu);
            }
            catch (Exception ex)
            {
                this.Controls.Add(new LiteralControl("An error has occured with this web part.  Please contact your system administrator and relay this error message: " + ex.InnerException.ToString() + " sub:CreateChildControls "));
            }
            finally
            {
                if (sPWeb != null)
                {
                    sPWeb.Dispose();
                }
            }
        }

        private void GetListItems(string str, MenuItem _spMenu, SPList _spListMenu)
        {
            try
            {
                SPQuery sPQuery = new SPQuery();
                sPQuery.Query = "<OrderBy><FieldRef Name='LinkOrder' Ascending='True' /><FieldRef Name='Title' Ascending='True' /></OrderBy><Where><And><Eq><FieldRef Name='ParentMenu' LookupId= 'TRUE'  /><Value Type='Lookup'>" + str + "</Value></Eq><Eq><FieldRef Name='ShowMenuItem' /><Value Type='Choice'>Yes</Value></Eq></And></Where>";
                SPListItemCollection items = _spListMenu.GetItems(sPQuery);
                MenuItem menuItem = new MenuItem();
                foreach (SPListItem sPListItem in items)
                {
                    string target = "";
                    if (sPListItem["Link URL"] == null)
                    {
                        menuItem = new MenuItem(sPListItem["Title"].ToString());
                    }
                    else
                    {
                        if (this._NewWindowFieldExists && sPListItem["OpenNewWindow"] != null && (bool)sPListItem["OpenNewWindow"])
                        {
                            target = "_blank";
                        }
                        menuItem = new MenuItem(sPListItem["Title"].ToString(), "", "", this.SetServerURL(sPListItem["Link URL"].ToString()), target);
                    }
                    this.GetListItems(sPListItem["ID"].ToString(), menuItem, _spListMenu);
                    _spMenu.ChildItems.Add(menuItem);
                }
            }
            catch (Exception ex)
            {
                this.Controls.Add(new LiteralControl("An error has occured with this web part.  Please contact your system administrator and relay this error message: " + ex.Message + " sub:GetListItems"));
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (this.Set_MenuPreRenderClientScript.Length > 0)
            {
                this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), this.Set_MenuPreRenderClientScript, this.Set_MenuPreRenderClientScript + "(document.getElementById('" + this.ClientID + "'));", true);
            }
            base.OnPreRender(e);
        }
    }
}
