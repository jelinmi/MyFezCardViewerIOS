using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CardFez
{	
	public partial class ValidateMember : ContentPage
	{
		protected static RestClient _restClient = new RestClient();
		public int Tpl;
		public class SIValidate
		{
			public int HQID { get; set; }
			public int TplNum { get; set; }
			public string LastName { get; set; }


		}
		public class UserInformation
		{
			public int hqid { get; set; }

			public int tplNum { get; set; }
			public string lastName { get; set; }
		}
		public async void POpUpConection()
		{


            await DisplayAlert("No connectivity ", "Ooops, it looks like you do not have an internet connection. Please check that you have mobile or Wi-Fi service and that airplane mode is not active.", "OK");
        }
        public async void btnLogin_Clicked(object sender, EventArgs e)
        {
            if (entryShrineId.Text.Length > 0 && entryLastName.Text.Length > 0 && Tpl > 0)
            {
                UserInformation userinformation = new UserInformation
                {
                    lastName = entryLastName.Text.ToString(),

                    hqid = Convert.ToInt32(entryShrineId.Text),
                    tplNum = Tpl


                };

                var response = await _restClient.PostAsync<Root2>("https://webfez.shrinenet.org/PublicAPI/v1/auth/ValidateMember", userinformation);


                if (response.statusCode != 2)

                {
                    
                    Preferences.Set("IsLoggedIn","true");
                    
                    await Xamarin.Essentials.SecureStorage.SetAsync("HQID", entryShrineId.Text);
                    App.Current.MainPage = new MainPage(userinformation.hqid);

                }
                else
                {
                    await DisplayAlert("", "Registration Failed!", "OK");

                }
            }




            else
            {
                await DisplayAlert("", "Registration Failed!", "OK");
            }
        }
        ViewCell lastCell;
        void OnPickerSelectedIndexChanged(object sender, ItemTappedEventArgs e)
        {
           
            var id = (Temple)searchResults.SelectedItem;
            var tp = id.TplNum;
            Tpl = Convert.ToInt32(tp);

            // searchResults.IsVisible = false;

        }
        List<Temple> ListOfTemples = new List<Temple>();
        async void GetTemple()
        {

            var response = await _restClient.GetAsync<Root>("https://webfez.shrinenet.org/PublicAPI/TemplesInformation");

            for (int i = 0; i < response.content.Count; i++)
            {
                var TempleNumResponse = response.content[i].TplNum;
                var TempleNameresponse = response.content[i].TempleName;

                ListOfTemples.Add(new Temple { TempleName = TempleNameresponse, TplNum = TempleNumResponse });



            };
        }

        public ValidateMember ()
		{
			InitializeComponent ();
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
                    GetTemple();
                    break;

                default:
                    POpUpConection();
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
                    
                    GetTemple();
                    break;
                default:

                    await DisplayAlert("No connectivity ", "Ooops, it looks like you do not have an internet connection to the internet. Please double check that you have mobile or Wi-Fi service and that airplane mode is not active.", "OK");

                    break;

            }

        }

        void OnTextChanged(object sender, EventArgs e)
        {

            var keyword = searchBar.Text;

            searchResults.ItemsSource = ListOfTemples.Where(n => n.TempleName.ToLower().Contains(keyword.ToLower()));


        }



        public class Temple
        {

            public string TplNum { get; set; }
            public string TempleName { get; set; }

        }
        public class Content
        {
            public string TplNum { get; set; }
            public string TempleName { get; set; }
            public string DirHours { get; set; }
            public string Email { get; set; }
            public string WebSite { get; set; }
            public string SuspendAfter { get; set; }
            public string BillPayWebsite { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }
            public string CountryCodeID { get; set; }
            public string PhoneNumber { get; set; }
        }

        public class Root
        {
            public List<Content> content { get; set; }
            public int statusCode { get; set; }
            public string statusDetail { get; set; }
        }
        public class Root2
        {
            public Content content { get; set; }
            public int statusCode { get; set; }
            public string statusDetail { get; set; }
        }

    }
}

