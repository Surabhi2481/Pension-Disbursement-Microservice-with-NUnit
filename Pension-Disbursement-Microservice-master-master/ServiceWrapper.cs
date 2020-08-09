using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PensionDisbursement
{
    public class ServiceWrapper
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(ServiceWrapper));
        private IConfiguration configuration;
        /// <summary>
        /// Dependency Injection
        /// </summary>
        /// <param name="iConfig"></param>
        public ServiceWrapper(IConfiguration iConfig)
        {
            configuration = iConfig;
        }
        /// <summary>
        /// Getting Value from the Pension Detail Microservice to validate the pension amount
        /// </summary>
        /// <param name="aadhar"></param>
        /// <returns>Status Code</returns>
        public PensionerDetail GetDetailResponse(string aadhar)
        {      
            HttpResponseMessage response = new HttpResponseMessage();
            string uriLink = configuration.GetValue<string>("Disbursementkey:UriLinkValue");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(uriLink);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    response = client.GetAsync("api/PensionerDetail/" + aadhar).Result;
                    _log4net.Info("respnse:" + response.ToString());
                }
                catch (Exception error) 
                {
                    _log4net.Error("Exception Occured" + error);

                    response = null; 
                    //return null;
                }
            }

            if (response == null)
            {

                return null;

            }
            string detailsResponse = response.Content.ReadAsStringAsync().Result;
            Result pen = JsonConvert.DeserializeObject<Result>(detailsResponse);       
            if(pen==null)
            {
                return null;
            }
            return pen.result;
        }   
    }
}
