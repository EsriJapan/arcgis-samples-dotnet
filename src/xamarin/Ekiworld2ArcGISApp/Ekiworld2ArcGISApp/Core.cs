using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ekiworld2ArcGISApp
{
    public class Core
    {
        static string api_Endpoint = "https://api.ekispert.jp";
        static string api_key = "LE_FZDRqSBUYcQXG";

        public static async Task<List<Ekiworld>> GetEkiworldResult(string Ekiworld)
        {   
            // 駅すぱあと Web サービスの駅情報 API の URL
            string queryString = api_Endpoint + "/v1/json/station?key=" + api_key + "&" + "gcs=wgs84&" + "name=" + Ekiworld;
            // HttpService クラスからリクエストを投げて、リクエスト結果を JSON オブジェクトで取得
            JObject results = await HttpService.getDataFromService(queryString).ConfigureAwait(false);

            var resultset = results["ResultSet"];

            if (resultset != null && resultset["Point"] != null) 
            {
                List<Ekiworld> ekiArr = new List<Ekiworld>();
                var type = resultset["Point"].GetType();
                if (type.Name == "JArray") {

                    var points = (JArray)resultset["Point"];

                    foreach (JObject point in points)
                    {
                        Ekiworld eki = new Ekiworld();

                        // Station
                        eki.stationName = (string)point["Station"]["Name"];
                        eki.stationCode = (string)point["Station"]["code"];
                        eki.stationType = (string)point["Station"]["Type"];
                        eki.stationYomi = (string)point["Station"]["Yomi"];

                        // GeoPoint
                        eki.longi_d = (double)point["GeoPoint"]["longi_d"];
                        eki.lati_d = (double)point["GeoPoint"]["lati_d"];

                        // Prefecture
                        eki.prefName = (string)point["Prefecture"]["Name"];

                        ekiArr.Add(eki);

                    }
                    return ekiArr;
                } 
                else if (type.Name == "JObject") 
                {
                    var point = (JObject)resultset["Point"];

                    Ekiworld eki = new Ekiworld();

                    // Station
                    eki.stationName = (string)point["Station"]["Name"];
                    eki.stationCode = (string)point["Station"]["code"];
                    eki.stationType = (string)point["Station"]["Type"];
                    eki.stationYomi = (string)point["Station"]["Yomi"];

                    // GeoPoint
                    eki.longi_d = (double)point["GeoPoint"]["longi_d"];
                    eki.lati_d = (double)point["GeoPoint"]["lati_d"];

                    // Prefecture
                    eki.prefName = (string)point["Prefecture"]["Name"];

                    ekiArr.Add(eki);

                    return ekiArr;
                }
                else
                {
                    return null;    
                }
            } 
            else 
            {
                return null;    
            }

        }
    }
}
