using Newtonsoft.Json;
using PursiX.Content.User;
using PursiX.Models;
using PursiX.Models.User;
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

namespace PursiX.Content
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventsPage : ContentPage
    {
        //For highlighted item
        ViewCell lastCell;
        //for global userId
        static int currentUser;
        //pagination variables
        static int takeHowMany = 3;
        static int skipHowMany = 0;
        //additional info popup data variables
        public static string addEventName;
        public static string addInfo;

        public EventsPage()
        {
            InitializeComponent();

            eventList.ItemsSource = new string[] { "" };

            NavigationPage.SetHasBackButton(this, false);

            Task task = LoadEvents();
        }

        protected override void OnAppearing()
        {
            Task task = LoadEvents();
            if (skipHowMany == 0)
            {
                btn_previous.IsEnabled = false;
            }
            NavigationPage.SetHasBackButton(this, false);
            base.OnAppearing();
        }

        //User can not use phone's back button (iOS might be different case)
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        async void OpenLinkInBrowser(object sender, System.EventArgs e)
        {
            if (Uri.IsWellFormedUriString(lbl_eventUrl.Text.ToLower(), UriKind.Absolute))
            {
                var uri = new Uri(lbl_eventUrl.Text);
                OpenBrowser(uri);
            }
            else
            {
                await DisplayAlert("Virhe", "Tapahtuman sivustoa ei voida avata.", "OK");
            }
        }
        public async void OpenBrowser(Uri uri)
        {
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }




        //***********************************************************************************************
        //LOAD ALL EVENTS to <eventList ListView>
        //private async void LoadWorkAssignments(object sender, EventArgs e) //dev use this kind of call with button 
        //***********************************************************************************************
        private async Task LoadEvents()
        {
            eventList.ItemsSource = new string[] { "Ladataan tapahtumia..." };

            try
            {

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");
                Task<string> json = client.GetStringAsync("/api/events");
                var eventsData = JsonConvert.DeserializeObject<List<Event>>(await json);


                var alleventsList = new List<Event>();
                foreach (var item in eventsData)
                {
                    alleventsList.Add(new Event { EventId = item.EventId, EventDateTime = item.EventDateTime, Name = item.Name, Description = item.Description, MaxParticipants = item.MaxParticipants, Url = item.Url, AdditionalDetails = item.AdditionalDetails, Latitude = item.Latitude, Longitude = item.Longitude });
                }

                //we need to sort our records from first to last
                var sortOldestFirst = alleventsList.OrderBy(x => x.EventDateTime)
                                                            .ToList();
                eventList.ItemsSource = sortOldestFirst.Skip(skipHowMany).Take(takeHowMany);

                //paging count here:
                var eventCount = Math.Ceiling((decimal)alleventsList.Count / 3); //decimal values rounds up to the next whole number
                lbl_eventCount.Text = eventCount.ToString();
                var pageCount = (skipHowMany / 3) + 1;
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



                pro_loading.IsRunning = false;
                pro_loading.IsVisible = false;

            }

            catch (Exception ex)
            {
                string error = ex.GetType().Name + ": " + ex.Message;
                eventList.ItemsSource = new string[] { error };
            }
        }


        //***********************************************************************************************
        //WHEN CLICKING AN EVENT FROM THE LIST
        //***********************************************************************************************

        private async void LoadDescription(object s, SelectedItemChangedEventArgs e)
        {
            try
            {
                //declaring model variable
                var obj = (Event)e.SelectedItem;

                //page element populating and showing description etc..
                if (String.IsNullOrEmpty(obj.Url))
                {
                    lbl_eventUrl.IsVisible = false;
                }
                else
                {
                    lbl_eventUrl.IsVisible = true;
                    lbl_eventUrl.Text = obj.Url;
                }
                lbl_eventNameDescription.IsVisible = true;
                lbl_eventDescription.Text = obj.Description;
                lbl_eventDescription.IsVisible = true;
                layout_participants.IsVisible = true;
                lbl_maxPartMax.Text = obj.MaxParticipants.ToString();
                addEventName = obj.Name;
                addInfo = obj.AdditionalDetails;


                //fetching participant data from database
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");
                Task<string> json = client.GetStringAsync("/api/participant");
                var eventsData = JsonConvert.DeserializeObject<List<EventParticipations>>(await json);

                //getting participant count for the current event
                var participantCount = (from ap in eventsData
                                        where ap.EventId == obj.EventId
                                        select ap.LoginId).Count();

                lbl_maxPartNow.Text = participantCount.ToString();


                //check if user is a participating in the selected Event
                try
                {
                   
                    currentUser = App._userId;
                    lbl_userId.Text = App._userId.ToString();

                    var alreadyParticipant = (from al in eventsData
                                              where al.LoginId == currentUser && al.EventId == obj.EventId
                                              select al).ToList();

                    //if user has not pressed participate button
                    if (alreadyParticipant.Count == 0)
                    {
                        lbl_pending.IsVisible = false;
                        btn_participate.IsVisible = true;
                        lbl_alreadyPart.IsVisible = false;
                    }
                    //if user has already pressed participate button
                    else if (alreadyParticipant.Count == 1)
                    {
                        var confirmed = (from co in eventsData
                                         where co.LoginId == currentUser && co.EventId == obj.EventId && co.Confirmed == true
                                         select co.Confirmed).FirstOrDefault();

                        //if user has participated, but admin has yet to confirm participation
                        if (confirmed == null)
                        {
                            lbl_pending.IsVisible = true;
                            btn_participate.IsVisible = false;
                            lbl_alreadyPart.IsVisible = false;
                        }
                        //if user has been participated and confirmed
                        else
                        {
                            lbl_pending.IsVisible = false;
                            btn_participate.IsVisible = false;
                            lbl_alreadyPart.IsVisible = true;
                        }
                    }

                }
                catch
                {
                    await DisplayAlert("Virhe", "Virhe on tapahtunut, yritä uudelleen", "OK");
                }

                //getting map data from Event -model.
                layout_map.IsVisible = true;
                LocationPin pin = new LocationPin
                {
                    Type = PinType.Place,
                    Position = new Position(obj.Latitude, obj.Longitude),
                    Label = "",
                    Address = "",
                    Name = "",
                    Url = ""
                };
                map.Pins.Clear(); //if this is not called, the pins will be added to user's map forever!
                map.CustomPins = new List<LocationPin> { pin };
                map.Pins.Add(pin);
                //when user clicks event from eventlist:
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(obj.Latitude, obj.Longitude), Distance.FromMiles(1.0)));
            }
            
            catch
            {
                await DisplayAlert("Virhe", "Virhe on tapahtunut, yritä uudelleen", "OK");
            }

        }


        //***********************************************************************************************
        //Keeping selected item higlighted (will not work if toggling between programs on android)
        //***********************************************************************************************
        private void ViewCell_Tapped(object sender, System.EventArgs e)
        {
            if (lastCell != null)
                lastCell.View.BackgroundColor = Color.Transparent;
            var viewCell = (ViewCell)sender;
            if (viewCell.View != null)
            {
                viewCell.View.BackgroundColor = Color.FromHex("F2BC57");
                lastCell = viewCell;
            }
        }

        //***********************************************************************************************
        //PARTICIPATE BUTTON POPUP
        //***********************************************************************************************

        private async void btn_participation(object sender, EventArgs e)
        {
            string commentInput = await AdditionalInfo(this.Navigation);

            if (string.IsNullOrEmpty(addEventName))
            {
                await DisplayAlert("Virhe", "Tapahtumaa ei ole valittuna, koita valita haluttu tapahtuma uudelleen", "OK");
            }
            else if (commentInput == null)
            {
                Task task = LoadEvents();
                lbl_eventUrl.IsVisible = false;
                lbl_eventNameDescription.IsVisible = false;
                lbl_eventDescription.IsVisible = false;
                layout_participants.IsVisible = false;
                lbl_alreadyPart.IsVisible = false;
                lbl_pending.IsVisible = false;
                btn_participate.IsVisible = false;
                layout_map.IsVisible = false;
                addEventName = "";
            }
            else
            {
                //await DisplayAlert(addEventName + " lisäinfo", "Lisäinfo: " + commentInput, "OK"); //dev purposes


                try
                {
                    var obj = (Event)eventList.SelectedItem;

                    AddEventParticipationModel data = new AddEventParticipationModel()
                    {
                        LoginId = App._userId,
                        EventId = obj.EventId,
                        Confirmed = false,
                        AddInfo = commentInput.ToString()
                    };

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("yourapiipaddress");
                    string input = JsonConvert.SerializeObject(data);
                    StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                    HttpResponseMessage message = await client.PostAsync("/api/participant/addparticipation", content);
                    string reply = await message.Content.ReadAsStringAsync();
                    bool success = JsonConvert.DeserializeObject<bool>(reply);

                    try
                    {
                        if (success)
                        {
                            await DisplayAlert("", "Osallistumispyyntö tapahtumaan " + addEventName.ToUpper() + " suoritettu. ", "OK");
                            Task task = LoadEvents();
                            lbl_eventUrl.IsVisible = false;
                            lbl_eventNameDescription.IsVisible = false;
                            lbl_eventDescription.IsVisible = false;
                            layout_participants.IsVisible = false;
                            lbl_alreadyPart.IsVisible = false;
                            lbl_pending.IsVisible = false;
                            btn_participate.IsVisible = false;
                            layout_map.IsVisible = false;
                            addEventName = "";
                        }
                        else
                        {
                            await DisplayAlert("Virhe!", "Virhe käsiteltäessä osallistumispyyntöä, ole hyvä ja yritä uudelleen", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = ex.GetType().Name + ": " + ex.Message;
                        Task task = LoadEvents();
                    }

                }
                catch (Exception ex)
                {
                    string error = ex.GetType().Name + ": " + ex.Message;
                    Task task = LoadEvents();
                }
            }
        }

        //***********************************************************************************************
        //https://forums.xamarin.com/discussion/35838/how-to-do-a-simple-inputbox-dialog
        //***********************************************************************************************
        public static Task<string> AdditionalInfo(INavigation navigation)
        {
            // user comment input
            var tcs = new TaskCompletionSource<string>();

            var lblEventName = new Label { Text = addEventName, FontAttributes = FontAttributes.Bold, FontSize = 14, BackgroundColor = Color.FromRgb(56, 166, 84), Padding = 15, TextColor = Color.White };
            var lblMessage = new Label { Text = addInfo, FontAttributes = FontAttributes.Italic };
            var lblLine = new BoxView { HeightRequest = 1, BackgroundColor = Color.FromRgb(0, 0, 0), HorizontalOptions = LayoutOptions.FillAndExpand };
            var txtInput = new Editor { Text = "", WidthRequest = 200, HeightRequest = 400, BackgroundColor = Color.FromRgb(220, 230, 224) };

            var btnOk = new Button
            {
                Text = "Lähetä",
                WidthRequest = 150,
                TextColor = Color.White,
                BackgroundColor = Color.FromRgb(56, 166, 84),
            };
            btnOk.Clicked += async (s, e) =>
            {
                // close page
                var result = txtInput.Text;
                await navigation.PopModalAsync();
                // pass result
                tcs.SetResult(result);
            };

            var btnCancel = new Button
            {
                Text = "Peruuta",
                WidthRequest = 85,
                TextColor = Color.White,
                BackgroundColor = Color.FromRgb(242, 114, 125)
            };
            btnCancel.Clicked += async (s, e) =>
            {
                // close page
                await navigation.PopModalAsync();
                // pass empty result
                tcs.SetResult(null);
            };

            var slButtons = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Children = { btnOk, btnCancel },
            };

            var layout = new StackLayout
            {
                Padding = new Thickness(0, 40, 0, 0),
                Margin = new Thickness(35, 0, 35, 0),
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Vertical,
                Children = { lblEventName, lblLine, lblMessage, txtInput, slButtons },
            };

            // create and show page
            var page = new ContentPage();
            page.Content = layout;
            navigation.PushModalAsync(page);
            // open keyboard
            txtInput.Focus();

            // code is waiting her, until result is passed with tcs.SetResult() in btn-Clicked
            // then proc returns the result
            return tcs.Task;
        }





        //***********************************************************************************************
        //PAGING
        //***********************************************************************************************

        //paging with buttons
        private async void nextPage(object sender, EventArgs e)
        {


            if (skipHowMany >= 0)
            {
                btn_previous.IsEnabled = true;
                int count = eventList.ItemsSource.OfType<object>().Count();
                if (count > 1 && count <= 3)
                {
                    lbl_noMoreResults.Text = "";
                    skipHowMany = skipHowMany + 3;
                    btn_next.IsEnabled = true;
                    await LoadEvents();
                }
                if (count <= 3)
                {
                    await LoadEvents();
                    if (eventList.ItemsSource.OfType<object>().Count() < 3)
                    {
                        lbl_noMoreResults.Text = "Tapahtumalistassa ei lisää näytettäviä tapahtumia!";
                        btn_next.IsEnabled = false;
                    }
                    else
                    {

                    }

                }
                if (count == 0)
                {
                    lbl_noMoreResults.Text = "Tapahtumalistassa ei lisää näytettäviä tapahtumia!";
                    btn_next.IsEnabled = false;
                }
            }

        }
        private async void previousPage(object sender, EventArgs e)
        {
            if (skipHowMany > 0)
            {
                lbl_noMoreResults.Text = "";
                skipHowMany = skipHowMany - 3;
                btn_next.IsEnabled = true;
                await LoadEvents();
            }
            if (skipHowMany < 0)
            {
                lbl_noMoreResults.Text = "";
                skipHowMany = 3;
                btn_next.IsEnabled = true;
                await LoadEvents();
            }
            if (skipHowMany == 0)
            {
                btn_previous.IsEnabled = false;
                btn_next.IsEnabled = true;
            }

        }

        //paging by swiping
        private async void NextPage_Swiped(object sender, SwipedEventArgs e)
        {
            if (skipHowMany >= 0)
            {
                btn_previous.IsEnabled = true;
                int count = eventList.ItemsSource.OfType<object>().Count();
                if (count > 1 && count <= 3)
                {
                    lbl_noMoreResults.Text = "";
                    skipHowMany = skipHowMany + 3;
                    btn_next.IsEnabled = true;
                    await LoadEvents();
                }
                if (count < 3)
                {
                    lbl_noMoreResults.Text = "Tapahtumalistassa ei lisää näytettäviä tapahtumia!";
                    btn_next.IsEnabled = false;
                    await LoadEvents();
                }
                if (count == 0)
                {
                    lbl_noMoreResults.Text = "Tapahtumalistassa ei lisää näytettäviä tapahtumia!";
                    btn_next.IsEnabled = false;
                }
            }

        }

        private async void PreviousPage_Swiped(object sender, SwipedEventArgs e)
        {
            if (skipHowMany > 0)
            {
                lbl_noMoreResults.Text = "";
                skipHowMany = skipHowMany - 3;
                btn_next.IsEnabled = true;
                await LoadEvents();
            }
            if (skipHowMany < 0)
            {
                lbl_noMoreResults.Text = "";
                skipHowMany = 3;
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
        //USER SETTINGS PAGE (passing global variable App._userId to UserSettingsPage)
        //***********************************************************************************************
        async void userSettings(object sender, EventArgs e)
        {
            try
            {
                var userId = new Login
                {
                    LoginID = App._userId
                };

                var SettingsPage = new UserSettingsPage();
                SettingsPage.BindingContext = userId;
                await Navigation.PushAsync(SettingsPage);


            }
            catch
            {
                await DisplayAlert("Virhe", "Virhe tapahtui siirryttäessä asetukset -sivulle. Ole hyvä ja yritä uudestaan.", "OK");
            }

            //await Navigation.PushAsync(new UserSettingsPage());
        }


        //***********************************************************************************************
        //LOGOUT CLICK
        //***********************************************************************************************
        async void Logout(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Kirjaudu ulos?", "Haluatko varmasti kirjautua ulos?", "Kyllä", "Ei");

            if (confirm == true)
            {
                App._IsLogged = false;
                await Navigation.PopToRootAsync();
            }
            else
            {

            }
        }


    }
}