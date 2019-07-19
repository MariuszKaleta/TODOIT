using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using ASPSnippets.LinkedInAPI;
using Com.CloudRail.SI.ServiceCode.Commands.CodeRedirect;
using Com.CloudRail.SI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Sparkle.LinkedInNET;

namespace Test
{
    [TestClass]
    public class LinkedinTest
    {

        [TestMethod]
        public void Skills()
        {
            try
            {
                LinkedInConnect.APIKey = "864js2wi87i7qh";
                LinkedInConnect.APISecret = "gWCqzeSCRJ8ypreN";

                LinkedInConnect.RedirectUrl = "";

                LinkedInConnect.Authorize();

                if (LinkedInConnect.IsAuthorized)
                {
                    DataSet ds = LinkedInConnect.Fetch();
                    var ImageUrl = ds.Tables["person"].Rows[0]["picture-url"].ToString();
                    var firstName = ds.Tables["person"].Rows[0]["first-name"].ToString();
                    firstName += " " + ds.Tables["person"].Rows[0]["last-name"].ToString();
                    var email = ds.Tables["person"].Rows[0]["email-address"].ToString();
                    var headLine = ds.Tables["person"].Rows[0]["headline"].ToString();
                    var industry = ds.Tables["person"].Rows[0]["industry"].ToString();
                    var id = ds.Tables["person"].Rows[0]["id"].ToString();
                    var name = ds.Tables["location"].Rows[0]["name"].ToString();
                    var picture = ds.Tables["person"].Rows[0]["picture-url"].ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

            
        }

    }
}
