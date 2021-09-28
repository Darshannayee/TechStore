using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechStore
{
    public class PaypalConfiguration
    {
        public readonly static string clientId;
        public readonly static string clientSecret;

        static PaypalConfiguration()
        {
            var config = getconfig();
            clientId = "Ad3wtvNP_Amzo-52CMXWX_s4HXeCEoAIv6eyNsN6Tdk5X8z-LMGHs7U-Ec58VBptuEa2FUu20EqVGqCZ";
            clientSecret = "EHFNz6ziJukd4bz0iKfR3c6KP7blArVNsUC7DG1-FtjPRHHmERebKejoQRkWgjaR6QclSCx4EsMXTSgK";

        }

        private static Dictionary<string, string> getconfig()
        {
            return PayPal.Api.ConfigManager.Instance.GetProperties();
        }

        private static string GetAccessToken()
        {
            string act = new OAuthTokenCredential(clientId, clientSecret, getconfig()).GetAccessToken();
            return act;
        }

        public static APIContext GetAPIContext()
        {
            APIContext apicntx = new APIContext(GetAccessToken());
            apicntx.Config = getconfig();
            return apicntx;
        }
    }
}