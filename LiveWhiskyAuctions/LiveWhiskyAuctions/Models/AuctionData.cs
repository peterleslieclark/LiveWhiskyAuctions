using System;
using System.Collections.Generic;
using System.Text;

namespace LiveWhiskyAuctions.Models
{
    public class AuctionData
    {
        public string SiteName { get; set; }
        public string LotName { get; set; }
        public string ImageUrl { get; set; }
        public string TimeLeft { get; set; }
        public string Url { get; set; }

        public AuctionData(string siteName, string lotName, string imageUrl, string timeLeft, string url)
        {
            SiteName = siteName;
            LotName = lotName;
            ImageUrl = imageUrl;
            TimeLeft = timeLeft;
            Url = url;
        }
    }

}
