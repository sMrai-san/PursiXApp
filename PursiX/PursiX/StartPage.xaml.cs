using PursiX.Content;
using PursiX.Content.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.Xaml;

namespace PursiX
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartPage : ContentPage
    {
        //introducing variables
        List<string> sources;
        //private double width;
        //private double height;


        public StartPage()
        {
            InitializeComponent();
        }

        //whenever the page is called
        protected override void OnAppearing()
        {
            if (App._slideShow == true)
            {
                //do nothing, because the timer is already running
            }
            else
            {
            GetImages();
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public async void QuitPursi(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Sulje sovellus?", "Haluatko varmasti sulkea sovelluksen?", "Kyllä", "Ei");

            if (confirm == true)
            {
                //forcequits the application
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
            else
            {
                //do not quit
            }
        }
        //when the screen is tilted horizontal and the screen width > height
        //protected override void OnSizeAllocated(double width, double height)
        //{
        //    base.OnSizeAllocated(width, height);
        //    if (width != this.width || height != this.height)
        //    {
        //        this.width = width;
        //        this.height = height;
        //        if (width > height)
        //        {
        //            slideshow.HorizontalOptions = LayoutOptions.FillAndExpand;
        //            slideshow.VerticalOptions = LayoutOptions.FillAndExpand;
        //            slideshow.HeightRequest = 1000;
        //            btn_login.HorizontalOptions = LayoutOptions.FillAndExpand;
        //            btn_register.HorizontalOptions = LayoutOptions.FillAndExpand;
        //            container_buttons.Orientation = StackOrientation.Horizontal;
        //            container_buttons.HorizontalOptions = LayoutOptions.FillAndExpand;
        //            container_buttons.HeightRequest = 50;
        //        }
        //        //after you turn it again to vertical position
        //        else
        //        {
        //            slideshow.HorizontalOptions = LayoutOptions.CenterAndExpand;
        //            slideshow.HeightRequest = 400;
        //            container_buttons.Orientation = StackOrientation.Vertical;
        //            container_buttons.VerticalOptions = LayoutOptions.FillAndExpand;
        //            container_buttons.HeightRequest = 120;

        //        }
        //    }
        //}



        //Automatic carousel for images, easy to setup without any NuGet -packages! (BUG!!!!!!!! When coming back to this page, the timer will start again)
        public void GetImages()
        {

            sources = new List<string>();
            sources.Add("https://www.bss.fi/@Bin/478228/IMGP7671.jpeg");
            sources.Add("https://www.bss.fi/@Bin/512453/10082246913_8658b05353_k.jpeg");
            sources.Add("https://www.bss.fi/@Bin/448025/IMGP7594.jpeg");
            sources.Add("https://www.bss.fi/@Bin/497629/IMGP1828.jpeg");
            sources.Add("https://www.bss.fi/@Bin/497631/IMGP1834.jpeg");

            slideshow.IsVisible = true;
            slideshow.ItemsSource = sources;

               Device.StartTimer(TimeSpan.FromSeconds(4), () =>
                    {
                        var animation = new Animation(v => slideshow.Scale = v, 0.8, 1);
                        animation.Commit(this, "CarouselAnimation", 16, 5000, Easing.Linear, (v, c) => slideshow.Scale = 1, () => true);
                        slideshow.Position = (slideshow.Position + 1) % sources.Count;
                        return true;

                    });

            App._slideShow = true;
          
        }

        //go to Admin -page
        async void ContinueToAdmin(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AdminLoginPage());
        }
        //go to info -page
        async void ContinueToInfo(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoPage());
        }

        //go to Login -page
        async void ContinueToLogin(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }

        //go to Registration -page
        async void ContinueToRegister(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistrationPage());
        }

    }
}