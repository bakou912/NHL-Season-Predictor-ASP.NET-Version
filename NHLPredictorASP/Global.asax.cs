#region Header

// Author: Tommy Andrews
// File: Global.asax.cs
// Project: NHLPredictorASP
// Created: 06/07/2019

#endregion

using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace NHLPredictorASP
{
    public class Global : HttpApplication
    {
        private void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}