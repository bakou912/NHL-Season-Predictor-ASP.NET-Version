#region Header

// Author: Tommy Andrews
// File: RouteConfig.cs
// Project: NHLPredictorASP
// Created: 06/07/2019

#endregion

using System.Web.Routing;
using Microsoft.AspNet.FriendlyUrls;

namespace NHLPredictorASP
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var settings = new FriendlyUrlSettings
            {
                AutoRedirectMode = RedirectMode.Permanent
            };
            routes.EnableFriendlyUrls(settings);
        }
    }
}