using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static System.Collections.Specialized.BitVector32;
using static System.Net.WebRequestMethods;

namespace LiveWhiskyAuctions.Views
{
    public partial class AboutPage : ContentPage
    {      

        private const string AuctionsUrl = "https://whiskyhunter.net/events/";

        public List<Auction> AuctionsList { get; private set; }    

        public AboutPage()
        {
            InitializeComponent();
            LoadAuctionsAsync();

            // Start the countdown timer
            //Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            //{
            //    UpdateTimeRemaining();
            //    return true;
            //});
        }

        private async void LoadAuctionsAsync()
        {
            var auctions = await ScrapeAuctionsAsync();
            auctionsListView.ItemsSource = auctions;
            AuctionsList = auctions;
        }

        private async Task<List<Auction>> ScrapeAuctionsAsync()
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(AuctionsUrl);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var tableNode = htmlDocument.DocumentNode.Descendants("div")
               .FirstOrDefault(node => node.GetAttributeValue("class", "")
               .Contains("table-responsive"));

            if (tableNode != null)
            {
                var auctionNodes = tableNode.Descendants("tr");

                var auctions = new List<Auction>();

                foreach (var node in auctionNodes)
                {
                    var auctionColumns = node.Descendants("td").ToList();                   

                    if (auctionColumns.Count > 0)
                    {               
                        var auction = new Auction
                        {
                            Name = auctionColumns[0].Descendants("a").FirstOrDefault()?.InnerText.Trim(),                           
                            EndTime = auctionColumns[2]?.InnerText.Trim()                            
                        };                       

                        auction.GetLogoAsync();

                        auctions.Add(auction);
                    }
                    
                }

                var whiskyshopAuction = new Auction
                {
                    Name = "The Whisky Shop Auction",
                    StartTime = "Varies Per Lot",
                    EndTime = "Varies Per Lot",
                    Link = "https://www.whiskyshop.com/auctions/lots"
                };

                auctions.Add(whiskyshopAuction);

                return auctions;
            }
            return null;
        }

        private void UpdateTimeRemaining()
        {
            foreach (var auction in AuctionsList)
            {
                if(auction.Name != "The Whisky Shop Auction")
                {
                    var timeRemaining = DateTime.Parse(auction.EndTime) - DateTime.Now;

                    if (timeRemaining.TotalSeconds <= 0)
                    {
                        auction.TimeRemaining = "Auction Ended";
                    }
                    else
                    {
                        auction.TimeRemaining = $"{timeRemaining.Days}d {timeRemaining.Hours}h {timeRemaining.Minutes}m {timeRemaining.Seconds}s";
                    }
                }                
            }
        }
        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var selectedItem = (Auction)e.SelectedItem;
            Device.OpenUri(new Uri(selectedItem.Link));

            ((ListView)sender).SelectedItem = null;
        }
    }

    public class Auction : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Logo { get; set; }
        public string Link { get; set; }
        public string TimeRemaining { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void GetLogoAsync()
        {
            if (!string.IsNullOrEmpty(Logo))
                return;
            Name = Name.Replace("-", "").Replace(" ", "");
            if (Name.ToLower().Contains("unicorn"))
            {
                Link = "https://bid.unicornauctions.com/";
                return;
            }
            if (Name.ToLower().Contains("irish"))
            {
                Link = "https://irishwhiskeyauctions.ie/views/all-auctions.php?page=current-auction";
                return;
            }
            if (Name.ToLower().Contains("prestige"))
            {
                Link = "https://www.prestigewhiskyauction.com/live-auction";
                return;
            }
            if (Name.ToLower().Contains("wva"))
            {
                Link = "https://www.wvawhiskyauctions.co.uk/live-auction";
                return;
            }
            if (Name.ToLower().Contains("whisky.auction"))
            {
                Link = "https://whisky.auction/auctions/live";
                return;
            }
            if (Name.ToLower().Contains("whiskyonline"))
            {
                Link = "https://live.whisky-onlineauctions.com/";
                return;
            }
            if (Name.ToLower().Contains("scotchwhisky"))
            {
                Link = "https://www.scotchwhiskyauctions.com/auctions/";
                return;
            }
            if (Name.ToLower().Contains("whiskyauctioneer"))
            {
                Link = "https://whiskyauctioneer.com/current-auction";
                return;
            }
            if (Name.ToLower().Contains("whiskyauction"))
            {
                Link = "https://whiskyauction.com/wac/auctionBrowser";
                return;
            }           
            if (Name.ToLower().Contains("thewhiskyshop"))
            {
                Link = "https://www.whiskyshop.com/auctions/lots";
                return;
            }
            if (Name.ToLower().Contains("bull"))
            {
                Link = "https://www.whiskybull.com/live-auction";
                return;
            }

        }
    }
}