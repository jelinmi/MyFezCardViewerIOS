using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CardFez
{
    public partial class ErrorPage : ContentPage
    {
        public string _TplNumber;
        protected static RestClient _restClient = new RestClient();

        public ErrorPage(string TplNum)
        {
            _TplNumber = TplNum;

            InitializeComponent();
        }

        protected override async void OnAppearing()

        {


            await GetInformationNobleMotherTemple(_TplNumber);

        }
        public async Task GetInformationNobleMotherTemple(string tpl)
        {

            var response = await _restClient.GetAsync<Root>("https://webfez.shrinenet.org/PublicAPI/TemplesInformation");


            var Responsefilter = response.content;


            //query
            var result = Responsefilter.Where(c => c.TplNum == _TplNumber).ToList();

            var templeNameResult = result[0].TempleName;
            var templeEmailResult = result[0].Email;
            var templePhoneNumResult = result[0].PhoneNumber;
            Infortemple.Text = "Your membership card cannot be displayed. Please contact your temple for more information." + Environment.NewLine + templeNameResult + Environment.NewLine + "Phone number: " + templePhoneNumResult + Environment.NewLine + "Email: " + templeEmailResult;



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

    }

}

