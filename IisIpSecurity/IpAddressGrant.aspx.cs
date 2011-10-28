using System;
using System.DirectoryServices;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Security.Principal;

namespace IisIpSecurity
{
    public partial class IpAddressGrant : System.Web.UI.Page
    {
        IISGrantedIpList IpList = null;
        string MetaBasePath = null;

        protected void Page_Init(object sender, EventArgs e)
        {
            // *** Force Authentication - 
            // *** NOTE: REQUIRES WINDOWS AUTH TO WORK 
            //           since we'll need to impersonate an admin account
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                Response.StatusCode = 401;
                Response.End();
            }

            // *** Force Impersonation
            Impersonate();

            this.MetaBasePath = Request.Form[this.txtMetaBasePath.UniqueID];
            if (string.IsNullOrEmpty(this.MetaBasePath))
            {
                this.MetaBasePath = Request.ServerVariables["INSTANCE_META_PATH"];
                this.MetaBasePath = ("IIS:/" + this.MetaBasePath + "/ROOT").Replace("LM", "localhost");
                this.txtMetaBasePath.Text = this.MetaBasePath;
            }
            this.IpList = new IISGrantedIpList(this.MetaBasePath);

            ShowGrantedIps();

            IIsWebSite[] Sites = this.IpList.GetIIsWebSites();
            this.lstSites.DataSource = Sites;
            this.lstSites.DataTextField = "SiteName";
            this.lstSites.DataValueField = "IISPath";
            this.lstSites.DataBind();
        }


        private void ShowGrantedIps()
        {
            string[] ipGrantList = IpList.GetIpList();
            StringBuilder sb = new StringBuilder();

            int count = 0;
            foreach (string IP in ipGrantList)
            {
                sb.AppendLine(IP);
            }
            this.lblIpCount.Text = (ipGrantList.Length).ToString() + " granted addresses";

            this.txtAddresses.Text = sb.ToString();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string enteredIps = this.txtAddresses.Text;
            string[] ipStrings = enteredIps.Replace("\n", "").Split(new char[1] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
            this.IpList.SetIpList(ipStrings);

            this.ShowGrantedIps();
        }


        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            // *** Explicitly Force update of the IP Path
            this.IpList.MetaBasePath = this.txtMetaBasePath.Text;

            // *** Update the granted IP list
            this.ShowGrantedIps();
        }



        void Impersonate()
        {
            if (!string.IsNullOrEmpty(this.User.Identity.Name) && this.User.IsInRole("Administrators"))
            {
                WindowsIdentity id = this.User.Identity as WindowsIdentity;
                WindowsImpersonationContext context = id.Impersonate();
            }

            return;
        }

        /// <summary>
        /// Allows reading and setting the IIS Granted IP List for a given Web site
        /// 
        /// Note:
        /// These routines only handle simple IP addressing rules. It handles
        /// the IPGrant list only and works only with full IP Addresses or
        /// Subnet granted wildcard IP Addresses. If you use domain name
        /// lookups don't use these routines to update!
        /// 
        /// These routines require ADMIN or System level permissions in
        /// order to set IIS configuration settings.
        /// 
        /// Disclaimer:
        /// Use at your own risk. These routines modify the IIS metabase
        /// and therefore have the potential to muck up your configuration.
        /// YOU ARE FULLY RESPONSIBLE FOR ANY PROBLEMS THAT THESE ROUTINES
        /// MIGHT CAUSE TO YOUR CONFIGURATION!
        /// </summary>
        public class IISGrantedIpList : IDisposable
        {

            public string MetaBasePath
            {
                get { return _MetaBasePath; }
                set { _MetaBasePath = value; }
            }
            private string _MetaBasePath = "IIS://localhost/W3SVC/1/ROOT";

            private DirectoryEntry IIS;

            public IISGrantedIpList(string metaBasePath)
            {
                if (!string.IsNullOrEmpty(metaBasePath))
                    this.MetaBasePath = metaBasePath;


            }


            private void Open()
            {
                this.Open(this.MetaBasePath);
            }
            private void Open(string IISMetaPath)
            {
                if (this.IIS == null)
                    this.IIS = new DirectoryEntry(IISMetaPath);
            }
            private void Close()
            {
                if (IIS != null)
                {
                    this.IIS.Close();
                    this.IIS = null;

                }
            }

            /// <summary>
            /// Returns a list of Ips as a plain string returning just the
            /// IP Addresses, leaving out the subnet mask values.
            /// 
            /// Any wildcarded IP Addresses will return .0 for the
            /// wild card characters.
            /// </summary>
            /// <returns></returns>
            public string[] GetIpList()
            {
                this.Open();

                // *** Grab the IP List
                object IPSecurity = IIS.Properties["IPSecurity"].Value;


                // retrieve the IPGrant list from the IPSecurity object. Note: Strings as objects
                //Array origIPGrantList = (Array)wwUtils.GetPropertyCom(IPSecurity, "IPGrant");

                Array origIPGrantList = (Array)
                 IPSecurity.GetType().InvokeMember("IPGrant",
                        BindingFlags.Public |
                        BindingFlags.Instance | BindingFlags.GetProperty,
                        null, IPSecurity, null);

                this.Close();

                // *** Format and Extract into a string list
                List<string> Ips = new List<string>();
                foreach (string IP in origIPGrantList)
                {
                    // *** Strip off the subnet-mask - we'll use .0 or .* to represent
                    string TIP = IP.Substring(0, IP.IndexOf(",")); //wwUtils.ExtractString(IP, "", ",");
                    Ips.Add(TIP);
                }


                return Ips.ToArray();
            }

            /// <summary>
            /// Allows you to pass an array of strings that contain the IP Addresses
            /// to grant. 
            /// 
            /// Wildcard IPs should use .* or .0 to indicate grants.
            /// 
            /// Note this string list should contain ALL IP addresses to grant
            /// not just new and added ones (ie. use GetList first and then
            /// add to the list.
            /// </summary>
            /// <param name="IPStrings"></param>
            public void SetIpList(string[] IPStrings)
            {
                this.Open();

                object IPSecurity = IIS.Properties["IPSecurity"].Value;

                // *** IMPORTANT: This list MUST be object or COM call will fail!
                List<object> newIpList = new List<object>();

                foreach (string Ip in IPStrings)
                {
                    string newIp;

                    if (Ip.EndsWith(".*.*.*") || Ip.EndsWith(".0.0.0"))
                        newIp = Ip.Replace(".*", ".0") + ",255.0.0.0";
                    else if (Ip.EndsWith(".*.*") || Ip.EndsWith(".0.0"))
                        newIp = Ip.Replace(".*", ".0") + ",255.255.0.0";
                    else if (Ip.EndsWith(".*") || Ip.EndsWith(".0"))
                    {
                        // *** Wildcard requires different IP Mask
                        newIp = Ip.Replace(".*", ".0") + ",255.255.255.0";
                    }
                    else
                        newIp = Ip + ", 255.255.255.255";


                    // *** Check for dupes - nasty but required because
                    // *** object -> string comparison can't do BinarySearch
                    bool found = false;
                    foreach (string tempIp in newIpList)
                    {
                        if (newIp == tempIp)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        newIpList.Add(newIp);
                }

                //wwUtils.SetPropertyCom(this.IPSecurity, "GrantByDefault", true);

                IPSecurity.GetType().InvokeMember("GrantByDefault",
                        BindingFlags.Public |
                        BindingFlags.Instance | BindingFlags.SetProperty,
                        null, IPSecurity, new object[] { true });


                object[] ipList = newIpList.ToArray();

                IPSecurity.GetType().InvokeMember("GrantByDefault",
                         BindingFlags.Public |
                         BindingFlags.Instance | BindingFlags.SetProperty,
                         null, IPSecurity, new object[] { false });

                // *** Apply the new list
                //wwUtils.SetPropertyCom(this.IPSecurity, "IPGrant",ipList);

                IPSecurity.GetType().InvokeMember("IPGrant",
                         BindingFlags.Public |
                         BindingFlags.Instance | BindingFlags.SetProperty,
                         null, IPSecurity, new object[] { ipList });


                IIS.Properties["IPSecurity"].Value = IPSecurity;
                IIS.CommitChanges();
                IIS.RefreshCache();

                this.Close();
            }


            /// <summary>
            /// Adds IP Addresses to the existing IP Address list
            /// </summary>
            /// <param name="IPString"></param>
            public void AddIpList(string[] newIps)
            {
                string[] origIps = this.GetIpList();

                List<string> Ips = new List<string>(origIps);
                foreach (string ip in newIps)
                {
                    Ips.Add(ip);
                }

                this.SetIpList(Ips.ToArray());
            }

            /// <summary>
            /// Returns a list of all IIS Sites on the server
            /// </summary>
            /// <returns></returns>
            public IIsWebSite[] GetIIsWebSites()
            {
                // *** IIS://Localhost/W3SVC/
                string iisPath = this.MetaBasePath.Substring(0, this.MetaBasePath.ToLower().IndexOf("/w3svc/")) + "/W3SVC";
                DirectoryEntry root = new DirectoryEntry(iisPath);

                List<IIsWebSite> Sites = new List<IIsWebSite>();
                foreach (DirectoryEntry Entry in root.Children)
                {
                    System.DirectoryServices.PropertyCollection Properties = Entry.Properties;

                    try
                    {
                        IIsWebSite Site = new IIsWebSite();
                        Site.SiteName = (string)Properties["ServerComment"].Value;

                        // *** Skip over non site entries
                        if (Site.SiteName == null || Site.SiteName == "")
                            continue;

                        Site.IISPath = Entry.Path;
                        Sites.Add(Site);
                    }
                    catch { ; }
                }

                root.Close();

                return Sites.ToArray();
            }


            #region IDisposable Members
            public void Dispose()
            {
                if (this.IIS != null)
                    IIS.Close();
            }
            #endregion
        }

        /// <summary>
        /// Container class that holds information about an IIS Web site
        /// </summary>
        public class IIsWebSite
        {

            /// <summary>
            /// The display name of the Web site
            /// </summary>
            public string SiteName
            {
                get { return _SiteName; }
                set { _SiteName = value; }
            }
            private string _SiteName = "";

            /// <summary>
            /// The IIS Metabase path
            /// </summary>
            public string IISPath
            {
                get { return _IISPath; }
                set { _IISPath = value; }
            }
            private string _IISPath = "";

        }

    }
}