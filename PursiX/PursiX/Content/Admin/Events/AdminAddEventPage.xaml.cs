using Newtonsoft.Json;
using PursiX.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace PursiX.Content.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminAddEventPage : ContentPage
    {
        public AdminAddEventPage()
        {
            InitializeComponent();
            var today = System.DateTime.Today;
            input_eventDate.Date = today;
            NavigationPage.SetHasBackButton(this, false);
        }

        protected override void OnAppearing()
        {
            var today = System.DateTime.Today;
            input_eventDate.Date = today;

            LocationPin pin = new LocationPin
            {
                Type = PinType.Place,
                Position = new Position(Convert.ToDouble(input_eventLatitude.Text), Convert.ToDouble(input_eventLongitude.Text)),
                Label = "",
                Address = "",
                Name = "",
                Url = ""
            };
            map.CustomPins = new List<LocationPin> { pin };
            map.Pins.Add(pin);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Convert.ToDouble(input_eventLatitude.Text), Convert.ToDouble(input_eventLongitude.Text)), Distance.FromMiles(1.0))); //you can use this when user clicks on event
        }

        //User can not use phone's back button (iOS might be different case)
        protected override bool OnBackButtonPressed()
        {
            return true;
        }


        async void AddEvent(object sender, EventArgs e)
        {
            if (input_eventUrl.TextColor == Color.Red)
            {
                await DisplayAlert("Virhe", "Tapahtuman sivusto on väärässä muodossa, ole hyvä ja korjaa sivusto oikeaan muotoon (esim. http://www.google.com)", "OK");
            }
            else 
            {
               
            AddEvent addEvent = new AddEvent()
            {
                EventDateTime = input_eventDate.Date,
                Name = input_eventName.Text,
                Description = input_eventDescription.Text,
                MaxParticipants = Convert.ToInt32(input_eventMaxParticipations.Text),
                Url = input_eventUrl.Text,
                AdditionalDetails = input_eventAddDetails.Text,
                Latitude = Convert.ToDouble(input_eventLatitude.Text),
                Longitude = Convert.ToDouble(input_eventLongitude.Text),
                AdminLogged = App._AdminLogged
            };

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("yourapiipaddress");
            string input = JsonConvert.SerializeObject(addEvent);
            StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
            HttpResponseMessage message = await client.PostAsync("/api/events/addnewevent", content);
            string reply = await message.Content.ReadAsStringAsync();
            bool success = JsonConvert.DeserializeObject<bool>(reply);

            if (success)
            {
                await DisplayAlert("OK", "Tapahtuma lisätty onnistuneesti!", "OK");
                await Navigation.PushAsync(new AdminEventsManagePage());
            }
            else
            {
                await DisplayAlert("Virhe", "Tapahtumaa ei voitu lisätä, yritä uudestaan", "OK");
            }

            }
        }




        //****************************************************
        //CANCEL CLICK
        //****************************************************
        async void Cancel(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AdminPage());
        }
    }
}