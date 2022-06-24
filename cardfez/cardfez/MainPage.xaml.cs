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

        public ContactDetails contact { get; set; }

        int ShrineNumber = 0;


        public MainPage(int hqid = 0)
        {

            ShrineNumber = hqid;
            if (ShrineNumber == 0)
            {
                gethqid();

            };


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


    

    async void gethqid()
        {

            var x = await Xamarin.Essentials.SecureStorage.GetAsync("HQID");
            ShrineNumber = Int32.Parse(x);


        }

        
        protected override async void OnAppearing()
        {
            MessagingCenter.Send(this, "SetLandscapeModeOn");
            await GetInformationNoble();
        }

        
       public async Task GetInformationNoble()
        {
            
            
            var nameValue = ShrineNumber;
            var response = await _restClient.GetAsync<Root>("https://webfez.shrinenet.org/PublicAPI/v1/Nobles/" +
                   nameValue +
                    "/MyFezCard");
            var response1 = await _restClient.GetAsync<Root2>("https://webfez.shrinenet.org/PublicAPI/v1/Nobles/" +
                 nameValue);
            var response2 = await _restClient.GetAsync<Root3>("https://webfez.shrinenet.org/PublicAPI/v1/Nobles/" + nameValue + "/NobleTempleRecords");
            var nobletemplrecinfo = response2.content;

            var TplNumber = nobletemplrecinfo[0].TplNum;


            if (response.statusCode == 0)
            {
                
                var card2 = response.content.shrineCard;
                
                webview3.Source = new HtmlWebViewSource
                {
                    Html = card2
                    
                };

                var myDisplay = DeviceDisplay.MainDisplayInfo;
                webview3.HeightRequest = myDisplay.Height;
                webview3.WidthRequest = myDisplay.Width;
               
                var maininfo = response1.content;
                var fullname = (maininfo.firstName + " " + maininfo.middleName + " " + maininfo.lastName).ToString();
                var Templename = nobletemplrecinfo[0].TempleName;

                string fixedText2 = (fullname + Environment.NewLine + "Shrine ID: " + ShrineNumber + Environment.NewLine + Templename + Environment.NewLine + "Current Dues Year:  2022");

                NobleInformationNoble.Text = fixedText2;

                this.barcodeImage2.BarcodeOptions = new ZXing.Common.EncodingOptions
                {
                    Height = 200,
                    Width = 200
                };
                this.barcodeImage2.BarcodeValue = response.content.qrcode + nameValue;


            }
            else
            {

                App.Current.MainPage = new ErrorPage(TplNumber);

            }

        }




    }

   

        public class Root2
        {
            public Content2 content { get; set; }
            public int statusCode { get; set; }
            public string statusDetail { get; set; }
        }

        public class Content2
        {
            public string hqid { get; set; }
            public string lastName { get; set; }
            public string lastName2 { get; set; }
            public string firstName { get; set; }
            public string middleName { get; set; }
            public string prefix { get; set; }
            public string suffix { get; set; }
            public string nickName { get; set; }
            public string dob { get; set; }
            public string birthplace { get; set; }
            public string spouse { get; set; }
            public string spousePrefix { get; set; }
            public string occupationTypeID { get; set; }
            public string isNoble { get; set; }
            public string isPCM { get; set; }
            public string isPCLM { get; set; }
            public string isDirectory { get; set; }
            public string isLegalSpouse { get; set; }
            public string hqnonSolicitation { get; set; }
            public string notes { get; set; }
            public string changeDate { get; set; }
            public string userID { get; set; }
            public string hasWebFezLogin { get; set; }
            public string canAccessLibrary { get; set; }
        }

        public class Content3
        {
            public string MbrTplRecID { get; set; }
            public string HQID { get; set; }
            public string TplNum { get; set; }
            public string TempleName { get; set; }
            public string MemNum { get; set; }
            public string StatusID { get; set; }
            public string StatusTitle { get; set; }
            public string StatusTypeT { get; set; }
            public string StatusDate { get; set; }
            public string DuesClassID { get; set; }
            public string DuesTitle { get; set; }
            public string DuesTotal { get; set; }
            public string DuesArrears { get; set; }
            public string BillPayWebsite { get; set; }
            public string Website { get; set; }
            public string IsNPD { get; set; }
        }

        public class Root3
        {
            public List<Content3> content { get; set; }
            public int statusCode { get; set; }
            public string statusDetail { get; set; }
        }
        public class Content
        {
        public string cardType { get; set; }
        public string duesYear { get; set; }
        public string tplNum { get; set; }
        public string templeName { get; set; }
        public string mbrTplRecID { get; set; }
        public string hqid { get; set; }
        public string isLegacy { get; set; }
        public string nobleName { get; set; }
        public string nameLine1 { get; set; }
        public string nameLine2 { get; set; }
        public string memNum { get; set; }
        public string qrcode { get; set; }
        public string shrineCard { get; set; }
    }

        public class Root
        {
            public Content content { get; set; }
            public int statusCode { get; set; }
            public string statusDetail { get; set; }
        }




        public class ContactDetails
        {
            public string HQID { get; set; }

        }

    
}


