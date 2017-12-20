using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Net.Http;
using System.Threading.Tasks;

namespace Ekiworld2ArcGISApp
{
    public class HttpService
    {
        private static HttpClient client = new HttpClient();

        public static async Task<JObject> getDataFromService(string queryString)
        {
            JObject data = null;
            // HttpClient クラスを利用して、駅すぱあとWebサービスの URL を指定
            var response = await client.GetAsync(queryString);
            if ((response != null) && !((int)response.StatusCode >= 400))
            {
                string json = response.Content.ReadAsStringAsync().Result;
                // JSON をオブジェクトにデシリアライズ
                data = (JObject)JsonConvert.DeserializeObject(json);
            }

            return data;
        }
    }
}

