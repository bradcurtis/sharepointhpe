using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Collections.ObjectModel;

namespace TopNav.Features.Feature1
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("3bc40425-2872-4abf-b57e-b64094d56a6a")]
    public class Feature1EventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                ULSLog2013.LogWarning("Starting feature activation");
                SPWebService service = SPWebService.ContentService;

                //There is another SPWebConfigModification constructor accepting 2 parameters:
                //name and xpath, but I think it is cleaner from the reader's perspective to do it in separate lines.
                SPWebConfigModification myModification = new SPWebConfigModification();

                //xPath to where we want to add our node. In order to avoid weird errors, consider this as key sensitive.
                myModification.Path = "configuration/system.web/siteMap/providers";
                myModification.Name = "add[@name='MyCustomNavigationProvider']";

                myModification.Sequence = 0;
                //The owner property will help us to categorize and clean just our own customizations when the feature will be deactivated.
                //You can choose a more professional naming convention to identify your changes to web.config.
                myModification.Owner = "DH";
                myModification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
                myModification.Value = "<add name=\"MyCustomNavigationProvider\" type=\"TopNav.MyCustomSiteMapProvider, TopNav, Version=1.0.0.0, Culture=neutral, PublicKeyToken=912f3c6491be12c2\" NavigationType=\"Global\" />";
                //You are adding a SPWebConfigModification to the collection of modifications.
                //Every time you run this code a new item will be added to it.
                service.WebConfigModifications.Add(myModification);

                /*Call Update and ApplyWebConfigModifications to save changes*/
                //If there is an error in any of the elements to add/modify in the web.config, it will produce an error and your line will not be added.
                //This collection works as a FIFO queue and therefore, you could be adding more and more elements to it.
                service.Update();
                service.ApplyWebConfigModifications();
            }
            catch (Exception exception)
            {
                //This is a logging class I use to write exceptions to the ULS Logs. There are plenty of options in this area as well.
                //Logging.WriteExceptionToTraceLog(exception.Source, exception.Message, exception, null);
                throw;
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPWebService service = SPWebService.ContentService;

                Collection<SPWebConfigModification> modsCollection = service.WebConfigModifications;

                // Find the most recent modification of a specified owner
                int modsCount1 = modsCollection.Count;
                for (int i = modsCount1 - 1; i > -1; i--)
                {
                    //I will remove from the web.config just the lines related to this solution and not the ones deployed from other solutions.
                    if (modsCollection[i].Owner.Equals("DH"))
                    {
                        modsCollection.Remove(modsCollection[i]);
                    }
                }

                // Save web.config changes. 
                service.Update();
                // Applies the list of web.config modifications to all Web applications in this Web service across the farm.
                service.ApplyWebConfigModifications();
            }
            catch (Exception exception)
            {
                //This is a logging class I use to write exceptions to the ULS Logs. There are plenty of options in this area as well.
                ULSLog2013.LogError(exception);
                throw;
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
