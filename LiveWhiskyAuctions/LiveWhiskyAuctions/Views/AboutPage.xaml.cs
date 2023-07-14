using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


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
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                UpdateTimeRemaining();
                return true;
            });
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
                        var endTime = auctionColumns[2]?.InnerText.Trim();
                        var timeRemaining = DateTime.Parse(endTime) - DateTime.Now;
                        
                        var auction = new Auction
                        {
                            Name = auctionColumns[0].Descendants("a").FirstOrDefault()?.InnerText.Trim(),                           
                            EndTime = endTime,
                            TimeRemaining = $"{timeRemaining.Days}d {timeRemaining.Hours}h {timeRemaining.Minutes}m"
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
                    TimeRemaining = "Varies Per Lot",                  
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
                    if (auction.Name.ToLower().Contains("whisky.auction"))
                    {
                        
                    }
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
        private string _timeRemaining;
        public string TimeRemaining { get => _timeRemaining; set { _timeRemaining = Uri.UnescapeDataString(value ?? string.Empty); OnPropertyChanged(); } }

       
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
            if (Name.ToLower().Contains("just"))
            {
                Link = "https://www.just-whisky.co.uk/";
                return;
            }
            if (Name.ToLower().Contains("grand"))
            {
                Link = "https://www.thegrandwhiskyauction.com/live-auction";
                return;
            }                     
            if (Name.ToLower().Contains("thewhiskyshop"))
            {
                Link = "https://www.whiskyshop.com/auctions/lots";
                return;
            }
            if (Name.ToLower().Contains("hammer"))
            {
                Link = "https://www.whiskyhammer.com/";
                return;
            }
            if (Name.ToLower().Contains("auktion"))
            {
                Link = "https://www.whiskyauktionberlin.de/";
                return;
            }
            if (Name.ToLower().Contains("bull"))
            {
                Link = "https://www.whiskybull.com/live-auction";
                return;
            }
            if (Name.ToLower().Contains("rum"))
            {
                Link = "https://rumauctioneer.com/";
                return;
            }
            if (Name.ToLower().Contains("speyside"))
            {
                Link = "https://www.speysidewhiskyauctions.co.uk/";
                return;
            }
            if (Name.ToLower().Contains("austra"))
            {
                Link = "https://www.australianwhiskyauctions.com.au/";
                return;
            }
            if (Name.ToLower().Contains("european"))
            {
                Link = "https://europeanwhiskyauctions.com/";
                return;
            }
            if (Name.ToLower().Contains("celtic"))
            {
                Link = "https://www.celticwhiskeyauction.com/en/auctions";
                return;
            }          
            if (Name.ToLower().Contains("whiskyauction"))
            {
                Link = "https://whiskyauction.com/wac/auctionBrowser";
                return;
            }

            //If we get here then there is an issue
            Link = "https://www.google.com/search?q=" + Name;
            return;
        }
    }
}