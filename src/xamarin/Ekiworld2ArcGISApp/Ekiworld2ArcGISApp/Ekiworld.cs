using System;
using System.Net.Http;

namespace Ekiworld2ArcGISApp
{
    public class Ekiworld
    {
        public string stationName { get; set; }
        public string stationYomi { get; set; }
        public string stationCode { get; set; }
        public string stationType { get; set; }

        public double longi_d { get; set; }
        public double lati_d { get; set; }
        public string prefName { get; set; }

        public Ekiworld()
        {
            this.stationName = "";
            this.stationYomi = "";
            this.stationCode = "";
            this.stationType = "";
            this.lati_d = 0;
            this.longi_d = 0;
            this.prefName = "";
        }
    }
}
