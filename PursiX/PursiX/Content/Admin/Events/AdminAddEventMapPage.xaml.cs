using PursiX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace PursiX.Content.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminAddEventMapPage : ContentPage
    {
        public AdminAddEventMapPage()
        {
            InitializeComponent();

            NavigationPage.SetHasBackButton(this, false);

            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(60.16952 , 24.93545), Distance.FromMiles(1.0)));
        }

        protected override void OnAppearing()
        {
            NavigationPage.SetHasBackButton(this, false);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(60.16952, 24.93545), Distance.FromMiles(1.0)));
        }

        //User can not use phone's back button (iOS might be different case)
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async void OnMapClicked(object sender, MapClickedEventArgs e)
            {

            try
            {
                await DisplayAlert("Paikkatieto", "Paikkatieto jonka valitsit:\nLeveysaste: " + e.Position.Latitude + "\nPituusaste: " + e.Position.Longitude, "Jatka");

                pro_loading.IsRunning = true;
                pro_loading.IsVisible = true;

                var selectedLocation = new Event
                {
                    Latitude = e.Position.Latitude,
                    Longitude = e.Position.Longitude

                };

                var AddEventPage = new AdminAddEventPage();
                AddEventPage.BindingContext = selectedLocation;
                await Navigation.PushAsync(AddEventPage);


            }
            catch
            {
                await DisplayAlert("Virhe", "Virhe tapahtui valittaessa paikkatietoa, ole hyvä ja yritä uudelleen", "OK");
            }


            }

        //****************************************************
        //CANCEL CLICK
        //****************************************************
        async void Cancel(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}