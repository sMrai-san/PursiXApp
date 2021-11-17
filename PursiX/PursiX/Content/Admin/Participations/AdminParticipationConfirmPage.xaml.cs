using Newtonsoft.Json;
using PursiX.Models;
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
    public partial class AdminParticipationConfirmPage : ContentPage
    {
        private List<EventsAndParticipationsCombinedModel> itemsToShow { get; set; }
        static int takeHowMany = 10;
        static int skipHowMany = 0;

        public AdminParticipationConfirmPage()
        {
            InitializeComponent();
            Task task = LoadEvents();
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
            Task task = LoadEvents();
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
                int count = unPartList.ItemsSource.OfType<object>().Count();

                if (count > 1 && count <= 10)
                {
                    lbl_noMoreResults.Text = "";
                    skipHowMany = skipHowMany + 10;
                    btn_next.IsEnabled = true;
                    await LoadEvents();
                }
                if (count < 10)
                {
                    lbl_noMoreResults.Text = "Ei lisää näytettäviä ilmoittautumisia!";
                    btn_next.IsEnabled = false;
                    await LoadEvents();
                }
                if (count == 0)
                {
                    lbl_noMoreResults.Text = "Ei lisää näytettäviä ilmoittautumisia!";
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
        //ADMIN LOAD PARTICIPANT CONFIRMATIONS
        //***********************************************************************************************
        private async Task LoadEvents()
        {
            unPartList.ItemsSource = new string[] { "Ladataan hyväksymättömiä osallistumispyyntöjä..." };
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
                HttpResponseMessage json = await client.PostAsync("/api/participant/admingetpart", content);
                string reply = await json.Content.ReadAsStringAsync();
                var partData = JsonConvert.DeserializeObject<List<EventsAndParticipationsCombinedModel>>(reply);


                var allUnconfirmed = new List<EventsAndParticipationsCombinedModel>();
                foreach (var item in partData)
                {
                    allUnconfirmed.Add(new EventsAndParticipationsCombinedModel
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

                var sortOldestFirst = allUnconfirmed.OrderBy(x => x.EventDateTime)
                                                            .ToList();

                unPartList.ItemsSource = sortOldestFirst.Skip(skipHowMany).Take(takeHowMany);

                //paging count here:
                var eventCount = Math.Ceiling((decimal)allUnconfirmed.Count / 10); //decimal values rounds up to the next whole number
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
                    lbl_countDivider.Text = " / ";
                    lbl_pageCount.Text = eventCount.ToString();

                }
                if (pageCount == eventCount)
                {
                    lbl_countDivider.Text = " / ";
                    btn_next.IsEnabled = false;
                    lbl_pageCount.Text = lbl_eventCount.Text;
                }
                if (allUnconfirmed.Count < 10)
                {
                    btn_next.IsEnabled = false;
                    btn_previous.IsEnabled = false;
                    lbl_countDivider.Text = "";
                    lbl_eventCount.Text = "";
                    lbl_pageCount.Text = "";

                }
                else
                {
                    lbl_countDivider.Text = " / ";
                    lbl_pageCount.Text = pageCount.ToString();
                }
                itemsToShow = sortOldestFirst;

                pro_loading.IsRunning = false;
                pro_loading.IsVisible = false;
            }

            catch (Exception ex)
            {
                string error = ex.GetType().Name + ": " + ex.Message;
                unPartList.ItemsSource = new string[] { error };
            }
        }




        //************************************************************************************
        //CONFIRM OR DELETE PARTICIPATION
        //************************************************************************************
        private async void ConfirmParticipant(object s, SelectedItemChangedEventArgs e)
        {
            var obj = (EventsAndParticipationsCombinedModel)e.SelectedItem;

            bool confirm = await DisplayAlert("Osallistujan lisäys", "Haluatko hyväksyä osallistujan:\n" + obj.FirstName + " " + obj.LastName + "Tapahtumaan:\n" + obj.Name.ToUpper(), "Hyväksy", "Hylkää");

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("yourapiipaddress");

            if (confirm == true)
            {
                if (App._AdminLogged == true)
                {
                    if (obj.JoinedParticipants < obj.MaxParticipants)
                    {

                        EventParticipationsModel confirmPart = new EventParticipationsModel
                        {
                            ParticipationId = obj.ParticipationId,
                            AdminLogged = App._AdminLogged

                        };


                        string input = JsonConvert.SerializeObject(confirmPart);
                        StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                        HttpResponseMessage message = await client.PutAsync("/api/participant/confirmpart", content);
                        string reply = await message.Content.ReadAsStringAsync();
                        bool success = JsonConvert.DeserializeObject<bool>(reply);

                        if (success)
                        {
                            await DisplayAlert("OK", "Osallistuja lisätty onnistuneesti!", "OK");
                            Task task = LoadEvents();
                        }
                        else
                        {
                            await DisplayAlert("Virhe", "Osallistujan hyväksynnässä tapahtui virhe, yritä uudelleen", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Virhe", "Tapahtumaan ei mahdu lisää osallistujia!", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Virhe", "Et ole kirjautuneena sisään!", "OK");
                    await Navigation.PopToRootAsync();
                }

            }
            if (confirm == false)
            {
                if (App._AdminLogged == true)
                {
                    bool confirmDelete = await DisplayAlert("Ilmoittautumisen poisto", "Haluatko poistaa ilmoittautumisen henkilöltä:\n" + obj.FirstName + " " + obj.LastName, "Kyllä", "Ei");

                    if (confirmDelete == true)
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
                            await DisplayAlert("OK", "Osallistuja hylätty onnistuneesti!", "OK");
                            Task task = LoadEvents();
                        }
                        else
                        {
                            await DisplayAlert("Virhe", "Osallistujan hylkäyksessä tapahtui virhe, yritä uudelleen", "OK");
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
            else
            {

            }
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