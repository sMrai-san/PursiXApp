using Newtonsoft.Json;
using PursiX.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace PursiX.Content.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminModifyEventPage : ContentPage
    {
        public AdminModifyEventPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
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

        private async void OnMapClicked(object sender, MapClickedEventArgs e)
        {

            try
            {
                await DisplayAlert("Paikkatieto", "Paikkatieto jonka valitsit:\nLeveysaste: " + e.Position.Latitude + "\nPituusaste: " + e.Position.Longitude, "Jatka");

                input_eventLatitude.Text = e.Position.Latitude.ToString();
                input_eventLongitude.Text = e.Position.Longitude.ToString();
                map.Pins.Clear();

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
            catch
            {
                await DisplayAlert("Virhe", "Virhe tapahtui valittaessa paikkatietoa, ole hyvä ja yritä uudelleen", "OK");
            }


        }


        //************************************************************************************
        //EDIT EVENT
        //************************************************************************************
        async void EditEvent(object sender, EventArgs e)
        {

            if (App._AdminLogged == true)
            {
                if (input_eventUrl.TextColor == Color.Red)
                {
                    await DisplayAlert("Virhe", "Tapahtuman sivusto on väärässä muodossa, ole hyvä ja korjaa sivusto oikeaan muotoon (esim. http://www.google.com)", "OK");
                }
                else
                {
                    ModifyEvent editEvent = new ModifyEvent
                    {
                        EventId = Convert.ToInt32(input_eventId.Text),
                        EventDateTime = input_eventDate.Date,
                        Name = input_eventName.Text,
                        Description = input_eventDescription.Text,
                        MaxParticipants = Convert.ToInt32(input_eventMaxParticipations.Text),
                        Url = input_eventUrl.Text,
                        Latitude = Convert.ToDouble(input_eventLatitude.Text),
                        Longitude = Convert.ToDouble(input_eventLongitude.Text),
                        AdminLogged = App._AdminLogged
                    };

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("yourapiipaddress");
                    string input = JsonConvert.SerializeObject(editEvent);
                    StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                    HttpResponseMessage message = await client.PutAsync("/api/events/editevent", content);
                    string reply = await message.Content.ReadAsStringAsync();
                    bool success = JsonConvert.DeserializeObject<bool>(reply);

                    if (success)
                    {
                        await DisplayAlert("OK", "Tapahtumaa muokattu onnistuneesti", "OK");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Virhe", "Tapahtumaa ei voitu muuttaa, yritä uudelleen", "OK");
                    }

                }
            }
            else
            {
                await DisplayAlert("Virhe", "Et ole kirjautuneena sisään!", "OK");
                await Navigation.PopToRootAsync();
            }
        }


        //************************************************************************************
        //DELETE EVENT
        //************************************************************************************
        async void DeleteEvent(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Poista tapahtuma?", "Haluatko varmasti poistaa tapahtuman " + input_eventName.Text, "Kyllä", "Ei");

            if (confirm == true)
            {
                if (App._AdminLogged == true)
            {

                ModifyEvent deleteEvent = new ModifyEvent
                {
                    EventId = Convert.ToInt32(input_eventId.Text),
                    EventDateTime = input_eventDate.Date,
                    Name = input_eventName.Text,
                    Description = input_eventDescription.Text,
                    MaxParticipants = Convert.ToInt32(input_eventMaxParticipations.Text),
                    Url = input_eventUrl.Text,
                    Latitude = Convert.ToDouble(input_eventLatitude.Text),
                    Longitude = Convert.ToDouble(input_eventLongitude.Text),
                    AdminLogged = App._AdminLogged
                };


                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");
                string input = JsonConvert.SerializeObject(deleteEvent);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PostAsync("/api/events/deleteevent", content);
                string reply = await message.Content.ReadAsStringAsync();
                bool success = JsonConvert.DeserializeObject<bool>(reply);

                if (success)
                {
                    await DisplayAlert("OK", "Tapahtuma poistettu onnistuneesti!", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Virhe", "Tapahtumaa ei voida poistaa, ole hyvä ja yritä uudelleen", "OK");
                }

            }
            else
            {
                await DisplayAlert("Virhe", "Et ole kirjautuneena sisään!", "OK");
                await Navigation.PopToRootAsync();
            }

            }
            else
            {

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