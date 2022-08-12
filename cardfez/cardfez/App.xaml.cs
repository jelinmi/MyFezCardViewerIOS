using System;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CardFez
{
    public partial class App : Application
    {
        
        public App()
        {
            InitializeComponent();
            var isLoggedIn = Preferences.Get("IsLoggedIn", "");

            if (isLoggedIn != "true")
            {
                //Load if Not Logged In
                MainPage = new NavigationPage(new ValidateMember());

            }
            else
            {
                //Load if Logged In
                MainPage = new NavigationPage(new MainPage());

            }


        }

        protected override void OnStart()
        {
            

        }
        protected override void OnSleep()
        {
            
            
        }

        protected override void OnResume()
        {
           
            var isLoggedIn = Preferences.Get("IsLoggedIn", "");

            if (isLoggedIn != "true")
            {
                //Load if Not Logged In
                MainPage = new NavigationPage(new ValidateMember());

            }
            else
            {
                //Load if Logged In
                MainPage = new NavigationPage(new MainPage());

            }

        }

    }
}
