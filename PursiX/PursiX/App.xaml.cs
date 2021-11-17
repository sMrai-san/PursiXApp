using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PursiX
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new StartPage());
        }

        //Login information variables
        public static bool _IsLogged { get; set; }
        public static int _userId { get; set; }
        public static bool _AdminLogged { get; set; }

        //StartPage carousel
        public static bool _slideShow { get; set; }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
