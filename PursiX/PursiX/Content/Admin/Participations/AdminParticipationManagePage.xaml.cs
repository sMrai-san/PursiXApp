using Newtonsoft.Json;
using PursiX.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PursiX.Content.Admin.Participations
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminParticipationManagePage : ContentPage
    {
        private List<EventsAndParticipationsCombinedModel> itemsToShow { get; set; }
        static int takeHowMany = 10;
        static int skipHowMany = 0;
        public static string addEventName;
        public static string addInfo;
        public static string userFullName;
        public static bool? isConfirmed;
        public static bool? stillConfirmed;

        public AdminParticipationManagePage()
        {
            InitializeComponent();
            Task task = LoadConfirmed();
            NavigationPage.SetHasBackButton(this, false);
        }

        protected override void OnAppearing()
        {
            pro_loading.IsRunning = true;
            pro_loading.IsVisible = true;
            if (skipHowMany == 0)
            {
                btn_previous.IsEnabled = false;
            }
            Task task = LoadConfirmed();
            NavigationPage.SetHasBackButton(this, false);
            base.OnAppearing();
        }

        //User can not use phone's back button (iOS might be different case)
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        //********************************************************************************************
        //NEXT AND PREVIOUS PAGINATION
        //********************************************************************************************
        private async void nextPage(object sender, EventArgs e)
        {


            if (skipHowMany >= 0)
            {
                btn_previous.IsEnabled = true;
                int count = partList.ItemsSource.OfType<object>().Count();

                if (count > 1 && count <= 10)
                {
                    lbl_noMoreResults.Text = "";
                    skipHowMany = skipHowMany + 10;
                    btn_next.IsEnabled = true;
                    await LoadConfirmed();
                }
                if (count < 10)
                {
                    lbl_noMoreResults.Text = "Ei lisää ilmoittautumisia!";
                    btn_next.IsEnabled = false;
                    await LoadConfirmed();
                }
                if (count == 0)
                {
                    lbl_noMoreResults.Text = "Ei lisää ilmoittautumisia!";
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
                await LoadConfirmed();
            }
            if (skipHowMany < 0)
            {
                lbl_noMoreResults.Text = "";
                skipHowMany = 10;
                btn_next.IsEnabled = true;
                await LoadConfirmed();
            }
            if (skipHowMany == 0)
            {
                btn_previous.IsEnabled = false;
                btn_next.IsEnabled = true;
            }

        }





        //***********************************************************************************************
        //ADMIN LOAD PARTICIPANT CONFIRMATIONS
        //***********************************************************************************************
        private async Task LoadConfirmed()
        {
            partList.ItemsSource = new string[] { "Ladataan hyväksyttyjä osallistujia..." };
            try
            {
                EventsAndParticipationsCombinedModel getAllParticipations = new EventsAndParticipationsCombinedModel
                {
                    AdminLogged = App._AdminLogged
                };

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");
                string input = JsonConvert.SerializeObject(getAllParticipations);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                HttpResponseMessage json = await client.PostAsync("/api/participant/admingetpartlist", content);
                string reply = await json.Content.ReadAsStringAsync();
                var partData = JsonConvert.DeserializeObject<List<EventsAndParticipationsCombinedModel>>(reply);


                var allConfirmed = new List<EventsAndParticipationsCombinedModel>();
                foreach (var item in partData)
                {
                    allConfirmed.Add(new EventsAndParticipationsCombinedModel
                    {
                        ParticipationId = item.ParticipationId,
                        EventId = item.EventId,
                        LoginId = item.LoginId,
                        AddInfo = item.AddInfo,
                        Confirmed = item.Confirmed,
                        Name = item.Name,
                        Description = item.Description,
                        EventDateTime = item.EventDateTime,
                        JoinedParticipants = item.JoinedParticipants,
                        MaxParticipants = item.MaxParticipants,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Email = item.Email
                    });
                }

                var sortOldestFirst = allConfirmed.OrderBy(x => x.EventDateTime)
                                                            .ToList();

                partList.ItemsSource = sortOldestFirst.Skip(skipHowMany).Take(takeHowMany);

                //paging count here:
                var eventCount = Math.Ceiling((decimal)allConfirmed.Count / 10); //decimal values rounds up to the next whole number
                lbl_eventCount.Text = eventCount.ToString();
                var pageCount = (skipHowMany / 10) + 1;
                if (btn_next.IsEnabled == false && btn_previous.IsEnabled == false)
                {
                    lbl_countDivider.Text = "";
                    lbl_eventCount.Text = "";
                    lbl_pageCount.Text = "";
                }
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

                itemsToShow = sortOldestFirst;

                pro_loading.IsRunning = false;
                pro_loading.IsVisible = false;
            }

            catch (Exception ex)
            {
                string error = ex.GetType().Name + ": " + ex.Message;
                partList.ItemsSource = new string[] { error };
            }
        }




        //************************************************************************************
        //CONFIRM OR DELETE PARTICIPATION
        //************************************************************************************
        private async void ManageParticipant(object s, SelectedItemChangedEventArgs e)
        {
            var obj = (EventsAndParticipationsCombinedModel)e.SelectedItem;

            bool confirm = await DisplayAlert("Osallistujan muokkaus", "Haluatko poistaa osallistujan:\n" + obj.FirstName + " " + obj.LastName + "\nTapahtumasta: " + obj.Name.ToUpper(), "Kyllä", "Ei");

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("yourapiipaddress");

            if (confirm == false)
            {
                if (App._AdminLogged == true)
                {
                    bool confirmManage = await DisplayAlert("Ilmoittautumisen muokkaus", "Haluatko muokata henkilön " + obj.FirstName + " " + obj.LastName + " ilmoittautumistietoja.", "Kyllä", "Ei");

                    if (confirmManage == true)
                    {
                        //event name for popup
                        addEventName = obj.Name;
                        addInfo = obj.AddInfo;
                        userFullName = obj.FirstName + " " + obj.LastName;
                        isConfirmed = obj.Confirmed;
                        


                        string commentInput = await AdditionalInfo(this.Navigation);

                        EventParticipationsModel editPart = new EventParticipationsModel
                        {
                            ParticipationId = obj.ParticipationId,
                            Confirmed = stillConfirmed,
                            AddInfo = commentInput,
                            AdminLogged = App._AdminLogged

                        };


                        string input = JsonConvert.SerializeObject(editPart);
                        StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                        HttpResponseMessage message = await client.PutAsync("/api/participant/editpart", content);
                        string reply = await message.Content.ReadAsStringAsync();
                        bool success = JsonConvert.DeserializeObject<bool>(reply);

                        if (success)
                        {
                            await DisplayAlert("OK", "Osallistujaa muokattu onnistuneesti!", "OK");
                            Task task = LoadConfirmed();
                        }
                        else
                        {
                            await DisplayAlert("Virhe", "Osallistujan muokkauksessa tapahtui virhe, yritä uudelleen", "OK");
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    await DisplayAlert("Virhe", "Et ole kirjautuneena sisään!", "OK");
                    await Navigation.PopToRootAsync();
                }

            }
            if (confirm == true)
            {
                if (App._AdminLogged == true)
                {

                        EventParticipationsModel deletePart = new EventParticipationsModel
                        {
                            ParticipationId = obj.ParticipationId,
                            AdminLogged = App._AdminLogged

                        };

                        string input = JsonConvert.SerializeObject(deletePart);
                        StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                        HttpResponseMessage message = await client.PostAsync("/api/participant/deletepart", content);
                        string reply = await message.Content.ReadAsStringAsync();
                        bool success = JsonConvert.DeserializeObject<bool>(reply);

                        if (success)
                        {
                            await DisplayAlert("OK", "Osallistuja poistettu onnistuneesti!", "OK");
                            Task task = LoadConfirmed();
                        }
                        else
                        {
                            await DisplayAlert("Virhe", "Osallistujan poistossa tapahtui virhe, yritä uudelleen", "OK");
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



        //***********************************************************************************************
        //PARTICIPANT MANAGE POPUP
        //***********************************************************************************************
        public static Task<string> AdditionalInfo(INavigation navigation)
        {
            // user comment input
            var tcs = new TaskCompletionSource<string>();

            var lblEventName = new Label { Text = addEventName, FontAttributes = FontAttributes.Bold, FontSize = 14, BackgroundColor = Color.FromRgb(56, 166, 84), Padding = 15, TextColor = Color.White };
            var lblMessage = new Label { Text = "Muuta käyttäjän " + userFullName + " lisäinfo sisältöä: " + addInfo, FontAttributes = FontAttributes.Italic };
            var lblLine = new BoxView { HeightRequest = 1, BackgroundColor = Color.FromRgb(0, 0, 0), HorizontalOptions = LayoutOptions.FillAndExpand };
            var txtInput = new Editor { Text = "", WidthRequest = 200, HeightRequest = 400, BackgroundColor = Color.FromRgb(220, 230, 224) };
            var lblActivation = new Label { Text = "Käyttäjän hyväksyntä tapahtumaan?" };
            var sw_Confirmation = new Switch { IsToggled = (bool)isConfirmed, HorizontalOptions = LayoutOptions.Start };
            sw_Confirmation.Toggled += (sender, e) =>
            {
                if (sw_Confirmation.IsToggled == true)
                {
                    stillConfirmed = true;
                }
                if (sw_Confirmation.IsToggled == false)
                {
                    stillConfirmed = false;
                }

            };

            var btnOk = new Button
            {
                Text = "Muokkaa",
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
                Children = { lblEventName, lblLine, lblMessage, txtInput, lblActivation, sw_Confirmation, slButtons },
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


        //****************************************************
        //CANCEL BUTTON CLICK
        //****************************************************
        async void Cancel(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

    }
}