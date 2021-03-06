﻿using System;
using System.Web;
using System.Web.Mvc;

namespace Purchasing.Mvc.Services
{
    public interface IServerLink
    {
        string Address { get; }
    }
    
    public class ServerLink : IServerLink
    {
        public string Address
        {
            get
            {
                return FullyQualifiedUri("~/Order/Lookup/").ToString();
            }
        }

        public static Uri FullyQualifiedUri(string relativeOrAbsolutePath)
        {
            Uri baseUri = HttpContext.Current.Request.Url;
            string path = UrlHelper.GenerateContentUrl(relativeOrAbsolutePath, new HttpContextWrapper(HttpContext.Current));
            Uri instance = null;
            bool ok = Uri.TryCreate(baseUri, path, out instance);
            return instance; // instance will be null if the uri could not be created
        }
    }
}