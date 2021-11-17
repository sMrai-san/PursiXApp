using Newtonsoft.Json;
using PursiX.Models;
using PursiX.Models.Admin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PursiX.Content.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminModifyEventListPage : ContentPage
    {
        //for search
        private List<Event> itemsToShow { get; set; }
        //for pagination
        static int takeHowMany = 10;
        static int skipHowMany = 0;

        public AdminModifyEventListPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
        }

        protected override void OnAppearing()
        {
            pro_loading.IsRunning = true;
            pro_loading.IsVisible = true;
            showActive.IsToggled = false;
            NavigationPage.SetHasBackButton(this, false);
            if (skipHowMany == 0)
            {
                btn_previous.IsEnabled = false;
            }
            Task task = LoadEvents();
            base.OnAppearing();
        }

        //User can not use phone's back button (iOS might be different case)
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void showPast_Toggled(object sender, ToggledEventArgs e)
        {
            pro_loading.IsRunning = true;
            pro_loading.IsVisible = true;
            input_searchEvent.Text = "";
            Task task = LoadEvents();
        }

        private void InputSearch(object sender, TextChangedEventArgs e)
        {
            //https://www.c-sharpcorner.com/article/search-data-from-xamarin-forms-list-view/
            //you have to make a public list<Event> for this one to work...
            //*******************************************************************************
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                eventList.ItemsSource = itemsToShow.Skip(skipHowMany).Take(takeHowMany);
            }

            else
            {
                eventList.ItemsSource = itemsToShow.Where(x => x.Name.ToLower().Contains(e.NewTextValue.ToLower()));
            }
        }

        //********************************************************************************************
        //NEXT AND PREVIOUS PAGINATION
        //********************************************************************************************
        private async void nextPage(object sender, EventArgs e)
        {


            if (skipHowMany >= 0)
            {
                btn_previous.IsEnabled = true;
                int count = eventList.ItemsSource.OfType<object>().Count();

                if (count > 1 && count <= 10)
                {
                    lbl_noMoreResults.Text = "";
                    skipHowMany = skipHowMany + 10;
                    btn_next.IsEnabled = true;
                    await LoadEvents();
                }
                if (count < 10)
                {
                    lbl_noMoreResults.Text = "Ei lisää näytettäviä tapahtumia!";
                    btn_next.IsEnabled = false;
                    await LoadEvents();
                }
                if (count == 0)
                {
                    lbl_noMoreResults.Text = "Ei lisää näytettäviä tapahtumia!";
                    btn_next.IsEnabled = false;
                }
            }

        }
        private async void previousPage(object sender, EventArgs e)
        {
            if (skipHowMany > 0)
            {
                lbl_noMoreResults.Text = "";
                skipHowMany = skipHowMany - 10;
                btn_next.IsEnabled = true;
                await LoadEvents();
            }
            if (skipHowMany < 0)
            {
                lbl_noMoreResults.Text = "";
                skipHowMany = 10;
                btn_next.IsEnabled = true;
                await LoadEvents();
            }
            if (skipHowMany == 0)
            {
                btn_previous.IsEnabled = false;
                btn_next.IsEnabled = true;
            }

        }

        //***********************************************************************************************
        //ADMIN LOAD EVENTS
        //***********************************************************************************************
        private async Task LoadEvents()
        {
            eventList.ItemsSource = new string[] { "Ladataan tapahtumia..." };

            try
            {
                //Define HttpClient for if/else
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");

                if (showActive.IsToggled == false)
                {
                    Task<string> json = client.GetStringAsync("/api/events");
                    var eventsData = JsonConvert.DeserializeObject<List<Event>>(await json);


                    var alleventsList = new List<Event>();
                    foreach (var item in eventsData)
                    {
                        alleventsList.Add(new Event { EventId = item.EventId, EventDateTime = item.EventDateTime, Name = item.Name, Description = item.Description, MaxParticipants = item.MaxParticipants, Url = item.Url, Latitude = item.Latitude, Longitude = item.Longitude });
                    }

                    //we need to sort our records from first to last
                    var sortOldestFirst = alleventsList.OrderBy(x => x.EventDateTime)
                                                                .ToList();
                    //data to eventlist
                    eventList.ItemsSource = sortOldestFirst.Skip(skipHowMany).Take(takeHowMany);

                    //paging count here:
                    var eventCount = Math.Ceiling((decimal)alleventsList.Count / 10); //decimal values rounds up to the next whole number
                    lbl_eventCount.Text = eventCount.ToString();
                    var pageCount = (skipHowMany / 10) + 1;
                    if (pageCount > eventCount)
                    {
                        lbl_pageCount.Text = eventCount.ToString();
                    }
                    if (pageCount == eventCount)
                    {
                        btn_next.IsEnabled = false;
                        lbl_pageCount.Text = lbl_eventCount.Text;
                    }
                    else
                    {
                        lbl_pageCount.Text = pageCount.ToString();
                    }

                    //same data to public list for search
                    itemsToShow = sortOldestFirst;


                    pro_loading.IsRunning = false;
                    pro_loading.IsVisible = false;
                }

                else if (showActive.IsToggled == true)
                {

                    AddEvent getAllEvents = new AddEvent
                    {
                        AdminLogged = App._AdminLogged
                    };

                    string input = JsonConvert.SerializeObject(getAllEvents);
                    StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                    HttpResponseMessage json = await client.PostAsync("/api/events/adminallevents", content);
                    string reply = await json.Content.ReadAsStringAsync();
                    var eventsData = JsonConvert.DeserializeObject<List<Event>>(reply);


                    var alleventsList = new List<Event>();
                    foreach (var item in eventsData)
                    {
                        alleventsList.Add(new Event { EventId = item.EventId, EventDateTime = item.EventDateTime, Name = item.Name, Description = item.Description, MaxParticipants = item.MaxParticipants, Url = item.Url, Latitude = item.Latitude, Longitude = item.Longitude });
                    }

                    var sortOldestFirst = alleventsList.OrderBy(x => x.EventDateTime)
                                                                .ToList();
                    
                    eventList.ItemsSource = sortOldestFirst.Skip(skipHowMany).Take(takeHowMany);

                    //paging count here:
                    var eventCount = Math.Ceiling((decimal)alleventsList.Count / 10); //decimal values rounds up to the next whole number
                    lbl_eventCount.Text = eventCount.ToString();
                    var pageCount = (skipHowMany / 10) + 1;
                    if (pageCount > eventCount)
                    {
                        lbl_pageCount.Text = eventCount.ToString();
                    }
                    else
                    {
                        lbl_pageCount.Text = pageCount.ToString();
                    }

                    itemsToShow = sortOldestFirst;

                    pro_loading.IsRunning = false;
                    pro_loading.IsVisible = false;
                }

            }

            catch (Exception ex)
            {
                string error = ex.GetType().Name + ": " + ex.Message;
                eventList.ItemsSource = new string[] { error };
            }
        }




        //*******************************************************************************************************************************
        //Passing data to another page https://forums.xamarin.com/discussion/136704/how-to-get-the-id-of-selected-item-from-listview
        //*******************************************************************************************************************************
        private async void LoadEventEdit(object s, SelectedItemChangedEventArgs e)
        {
            try
            {

                CultureInfo dateFormat = new CultureInfo("fi-FI", false);

                var obj = (Event)e.SelectedItem;

                var selectedEvent = new Event
                {
                    EventId = obj.EventId,
                    EventDateTime = obj.EventDateTime,
                    Name = obj.Name,
                    Description = obj.Description,
                    MaxParticipants = obj.MaxParticipants,
                    Url = obj.Url,
                    Latitude = obj.Latitude,
                    Longitude = obj.Longitude
                };

                var EditEventPage = new AdminModifyEventPage();
                EditEventPage.BindingContext = selectedEvent;
                await Navigation.PushAsync(EditEventPage);


            }
            catch
            {
                await DisplayAlert("Virhe", "Virhe tapahtui siirryttäessä tapahtuman muokkaukseen, ole hyvä ja yritä uudelleen", "OK");
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