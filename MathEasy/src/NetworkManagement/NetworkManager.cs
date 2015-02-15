using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using MathEasy.src.ConfigurationManagement;

namespace MathEasy.src.NetworkManagement
{
    public static class NetworkManager
    {
        /// <summary>
        /// Uploads data to web service.
        /// </summary>
        /// <param name="ptext">Plaintext to be uploaded</param>
        /// <param name="htmlBody">HTMl body of the document to be uploaded</param>
        /// <returns>ACCESS URL</returns>
        public static String uploadData(String ptext, String htmlBody)
        {
            try
            {
                byte[] response = null;
                using (WebClient client = new WebClient())
                {
                    response = client.UploadValues(ConfigurationManager.URL+@"/upload.php", new NameValueCollection()
                {
                    { "ptext", ptext },
                    { "html", htmlBody }
                });
                }
                return Encoding.UTF8.GetString(response);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while uploading data to URL: " + ConfigurationManager.URL+"\nError text: "+ex.Message);
            }
        }

        /// <summary>
        /// Downloads data from web service.
        /// </summary>
        /// <param name="URL">URL from which to download</param>
        /// <returns>PLAINTEXT</returns>
        public static String downloadData(String aCode)
        {
            try
            {
                String sURL;
                sURL = ConfigurationManager.URL+"/appget/"+aCode;

                WebRequest wrGETURL;
                wrGETURL = WebRequest.Create(sURL);

                Stream objStream;
                objStream = wrGETURL.GetResponse().GetResponseStream();

                StreamReader objReader = new StreamReader(objStream);

                String response = objReader.ReadToEnd();

                return response ;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while downloading data from URL: " + ConfigurationManager.URL +"/appget/"+aCode+ "\nError text: " + ex.Message);
            }
        }
    }
}
