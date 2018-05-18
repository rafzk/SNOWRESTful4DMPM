using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SNOWRESTful4DMPM.Entities
{
    class API
    {

        string _endpoint;
        string _contentType;
        string _userName;
        string _passWord;
        string _timeout;


        public string Endpoint { get => _endpoint; set => _endpoint = value; }
        public string ContentType { get => _contentType; set => _contentType = value; }
        public string UserName { get => _userName; set => _userName = value; }
        public string Password { get => _passWord; set => _passWord = value; }
        public string Timeout { get => _timeout; set => _timeout = value; }

        public API (string endpoint, string contentType, string userName, string passWord, string timeout)
        {
            _contentType = contentType;
            _endpoint = endpoint;
            _userName = userName;
            _passWord = passWord;
            _timeout = timeout;
        }

        public string MakeWebRequest()
        {

            try
            {
                // Create a request for the URL. 
                WebRequest request = WebRequest.Create(this.Endpoint);
                // Set the Method property of the request to GET.
                request.Method = "GET";
                // Set the ContentType property of the request to GET.
                request.ContentType = this.ContentType;
                // Set the Timeout property of the request to GET.
                //request.ContentType = this.Timeout;
                request.Timeout = 60000000;
                // Set credentials
                request.Credentials = new NetworkCredential(UserName, this.Password);
                // Get the response.  
                WebResponse response = request.GetResponse();
                // Display the status.  
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.  
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string responseFromServer = reader.ReadToEnd();
                // Display the content.  
                Console.WriteLine(responseFromServer);
                // Clean up the streams and the response.  
                reader.Close();
                response.Close();

                return responseFromServer;
            }
            catch (Exception ex)
            {
                //https://docs.microsoft.com/en-us/dotnet/framework/network-programming/understanding-webrequest-problems-and-exceptions

                Console.WriteLine(ex.InnerException.ToString());


                return "";
                //if (ex.Status == WebExceptionStatus.ProtocolError)
                //{
                //    HttpWebResponse res = (HttpWebResponse)ex.Response;
                //    log.Error("Exception found on REST API call - ERROR - ProtocolError");
                //    log.Error("Exception found on REST API call - Response error code: " + (int)res.StatusCode);
                //    log.Error("Exception found on REST API call - Response message: " + ex.Message.ToString());
                //    log.Error("Exception found on REST API call - Endpoint: " + this.Endpoint);
                //    log.Error("Exception found on REST API call - Username: " + this.UserName);
                //    return ex.Message.ToString();
                //}
                //else
                //{
                //    HttpWebResponse res = (HttpWebResponse)ex.Response;
                //    log.Error("Exception found on REST API call - ERROR - " + ex.Status.ToString());
                //    log.Error("Exception found on REST API call - Response error code: " + (int)res.StatusCode);
                //    log.Error("Exception found on REST API call - Response message: " + ex.Message.ToString());
                //    log.Error("Exception found on REST API call - Endpoint: " + this.Endpoint);
                //    log.Error("Exception found on REST API call - Username: " + this.UserName);
                //    return ex.Message.ToString();
                //}

            }


        }

    }
}
