using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ModelState = System.Web.Mvc.ModelState;

namespace Cpanel.Context
{

    public class Connecttocore
    {  //Base Url
       public static string BASE_URL = "http://192.168.66.9:8080/IBMiddleware/webresources/IBWebservices";
     //  public static string BASE_URL = "http://192.168.0.126:8080/IBMiddleware/webresources/IBWebservices";
        public static string GetCustinfo(string accountNo)
        {

            Uri requestUri = new Uri(BASE_URL + "/GetCustinfo");

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.account = accountNo;//"990042010593883".ToString();
           
            dynamicJson.lang = "1";
            dynamicJson.uuid = Guid.NewGuid();

            string json = "";
            json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";
            JObject JResp = new JObject();

            using (var objClient = new HttpClient())
            {
                try
                {

                    HttpResponseMessage respon = objClient
                        .PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;

                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error";
                }

                return responJsonText;

            }

        }

        public static string GetCustaccounts(string accountNo)
        {

            Uri requestUri = new Uri(BASE_URL + "/GetCustinfoByID");

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.account = accountNo;//"990042010593883".ToString();

            dynamicJson.lang = "1";
            dynamicJson.uuid = Guid.NewGuid();

            string json = "";
            json = JsonConvert.SerializeObject(dynamicJson);
            var responJsonText = "";
            JObject JResp = new JObject();

            using (var objClient = new HttpClient())
            {
                try
                {

                    HttpResponseMessage respon = objClient
                        .PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;

                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error";
                }

                return responJsonText;

            }

        }
    }
}