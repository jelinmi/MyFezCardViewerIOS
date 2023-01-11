using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Foundation;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;

using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace CardFez
{
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        protected static RestClient _restClient = new RestClient();

        

        int ShrineNumber = 0;


        public MainPage(int hqid = 0)
        {

            ShrineNumber = hqid;
            if (ShrineNumber == 0)
            {
                gethqid();

            };

            Xamarin.Forms.Application.Current.On<iOS>().SetEnableAccessibilityScalingForNamedFontSizes(false);
            InitializeComponent();

            var current = Connectivity.NetworkAccess;
            switch (current)
            {
                case NetworkAccess.Internet:
                // Connected to internet

                case NetworkAccess.Local:
                // Only local network access

                case NetworkAccess.ConstrainedInternet:
                // Connected, but limited internet access such as behind a network login page

                case NetworkAccess.Unknown:

                    // Internet access is unknown
                    MessagingCenter.Send(this, "SetLandscapeModeOn");
                    
                    OnAppearing();
                    break;

                default:
                    DisplayAlert("No connectivity ", "Ooops, it looks like you do not have an internet connection. Please check that you have mobile or Wi-Fi service and that airplane mode is not active.", "OK");
                    break;
            }
            var profiles = Connectivity.ConnectionProfiles;
            if (profiles.Contains(ConnectionProfile.WiFi))
            {
                // Active Wi-Fi connection.
            }
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            



        }
        async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var access = e.NetworkAccess;
            var profiles = e.ConnectionProfiles;
            switch (access)
            {
                case NetworkAccess.Internet:
                // Connected to internet
                case NetworkAccess.Local:
                // Only local network access
                case NetworkAccess.ConstrainedInternet:
                // Connected, but limited internet access such as behind a network login page

                case NetworkAccess.Unknown:
                    // Internet access is unknown
                    MessagingCenter.Send(this, "SetLandscapeModeOn");
                    await GetInformationNoble();
                    break;
                default:

                    await DisplayAlert("No connectivity ", "Ooops, it looks like you do not have an internet connection to the internet. Please double check that you have mobile or Wi-Fi service and that airplane mode is not active.", "OK");

                    break;

            }

        }


    

        public async void gethqid()
        {

            var x = await Xamarin.Essentials.SecureStorage.GetAsync("HQID");
            ShrineNumber = Int32.Parse(x);


        }

        
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send(this, "SetLandscapeModeOn");
            await GetInformationNoble();
      
            UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.LandscapeLeft), new NSString("orientation"));
        
    }

        
       public async Task GetInformationNoble()
        {
            var nameValue = ShrineNumber;

            var response = await _restClient.GetAsync<Root>("https://webfez.shrinenet.org/PublicAPI/v1/Nobles/" +
                   nameValue +
                    "/MyFezCard");
           

            var TplNumber = response.content.tplNum;


            if (response.statusCode == 0)
            {
                var d = "";
                if (response.content.assocTemples.Contains("Shriners"))
                {
                    d = "yes";
                }
                else
                {
                    d = "no";
                    MainListView.IsVisible = false;
                 
                    NobleInformationNoble.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));

                }
                var card2 = response.content.shrineCard;
                
                webview3.Source = new HtmlWebViewSource
                {
                    Html = card2
                    
                };

                var myDisplay = DeviceDisplay.MainDisplayInfo;
                webview3.HeightRequest = myDisplay.Height;
                webview3.WidthRequest = myDisplay.Width;
                var DuesYear = response.content.duesYear;
               
                var fullname = (response.content.nameLine1 + " " + response.content.nameLine2).ToString();
                var Templename = response.content.templeName;
                var assoc = response.content.assocTemples;
                string[] namesArray = assoc.Split(',').ToArray();
                List<string> list = new List<string>(namesArray);
                MainListView.ItemsSource = list;

                string fixedText2 = (fullname + Environment.NewLine + "Shrine ID: " + ShrineNumber + Environment.NewLine +"Home Temple: "+ Templename + Environment.NewLine + "Current Dues Year: "+ DuesYear);

                NobleInformationNoble.Text = fixedText2;

                this.barcodeImage2.BarcodeOptions = new ZXing.Common.EncodingOptions
                {
                    Height = 200,
                    Width = 200
                };
                this.barcodeImage2.BarcodeValue = response.content.qrcode;


            }
            else
            {
                var tpl = await Xamarin.Essentials.SecureStorage.GetAsync("TplNum");
                ShrineNumber = Int32.Parse(tpl);
                App.Current.MainPage = new ErrorPage(tpl);

            }

        }




    }

    

    
        
        
        public class Root
        {
            public Content content { get; set; }
            public int statusCode { get; set; }
            public string statusDetail { get; set; }
        }
        public class Content
        {
            public string cardType { get; set; }
            public string duesYear { get; set; }
            public string tplNum { get; set; }
            public string statusID { get; set; }
            public string statusName { get; set; }
            public string templeName { get; set; }
            public string mbrTplRecID { get; set; }
            public string hqid { get; set; }
            public string isLegacy { get; set; }
            public string nobleName { get; set; }
            public string nameLine1 { get; set; }
            public string nameLine2 { get; set; }
            public string memNum { get; set; }
            public string qrcode { get; set; }
            public string assocTemples { get; set; }
            public string shrineCard { get; set; }
        }








}


