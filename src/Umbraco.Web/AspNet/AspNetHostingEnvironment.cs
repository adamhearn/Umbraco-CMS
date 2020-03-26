using System;
using System.Web;
using System.Web.Hosting;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Hosting;

namespace Umbraco.Web.Hosting
{
    public class AspNetHostingEnvironment : IHostingEnvironment
    {
        
        private readonly IHostingSettings _hostingSettings;
        private string _localTempPath;

        public AspNetHostingEnvironment(IHostingSettings hostingSettings)
        {
            _hostingSettings = hostingSettings ?? throw new ArgumentNullException(nameof(hostingSettings));
            SiteName = HostingEnvironment.SiteName;
            ApplicationId = HostingEnvironment.ApplicationID;
            ApplicationPhysicalPath = HostingEnvironment.ApplicationPhysicalPath;
            ApplicationVirtualPath = HostingEnvironment.ApplicationVirtualPath;
            IISVersion = HttpRuntime.IISVersion;
        }

        public string SiteName { get; }
        public string ApplicationId { get; }
        public string ApplicationPhysicalPath { get; }

        public string ApplicationVirtualPath { get; }
        public bool IsDebugMode => HttpContext.Current?.IsDebuggingEnabled ?? _hostingSettings.DebugMode;
        /// <inheritdoc/>
        public bool IsHosted => (HttpContext.Current != null || HostingEnvironment.IsHosted);

        public Version IISVersion { get; }

        public string MapPath(string path)
        {
            return HostingEnvironment.MapPath(path);
        }

        public string ToAbsolute(string virtualPath, string root) => VirtualPathUtility.ToAbsolute(virtualPath, root);
        

        public string LocalTempPath
        {
            get
            {
                if (_localTempPath != null)
                    return _localTempPath;

                switch (_hostingSettings.LocalTempStorageLocation)
                {
                    case LocalTempStorage.AspNetTemp:
                        return _localTempPath = System.IO.Path.Combine(HttpRuntime.CodegenDir, "UmbracoData");

                    case LocalTempStorage.EnvironmentTemp:

                        // environment temp is unique, we need a folder per site

                        // use a hash
                        // combine site name and application id
                        //  site name is a Guid on Cloud
                        //  application id is eg /LM/W3SVC/123456/ROOT
                        // the combination is unique on one server
                        // and, if a site moves from worker A to B and then back to A...
                        //  hopefully it gets a new Guid or new application id?

                        var hashString = SiteName + "::" + ApplicationId;
                        var hash = hashString.GenerateHash();
                        var siteTemp = System.IO.Path.Combine(Environment.ExpandEnvironmentVariables("%temp%"), "UmbracoData", hash);

                        return _localTempPath = siteTemp;

                    //case LocalTempStorage.Default:
                    //case LocalTempStorage.Unknown:
                    default:
                        return _localTempPath = MapPath("~/App_Data/TEMP");
                }
            }
        }
       
    }


}