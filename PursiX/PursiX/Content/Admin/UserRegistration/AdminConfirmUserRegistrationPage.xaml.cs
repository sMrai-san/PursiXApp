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

namespace PursiX.Content.Admin.UserRegistration
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminConfirmUserRegistrationPage : ContentPage
    {
        private List<AddUserModel> itemsToShow { get; set; }

        //paging variables
        static int takeHowMany = 10;
        static int skipHowMany = 0;

        public AdminConfirmUserRegistrationPage()
        {
            InitializeComponent();
            lbl_countDivider.Text = "";
            lbl_eventCount.Text = "";
            lbl_pageCount.Text = "";
            Task task = LoadUnconfirmedAccounts();
            NavigationPage.SetHasBackButton(this, false);
        }

        protected override void OnAppearing()
        {
            lbl_countDivider.Text = "";
            lbl_eventCount.Text = "";
            lbl_pageCount.Text = "";
            pro_loading.IsRunning = true;
            pro_loading.IsVisible = true;
            if (skipHowMany == 0)
            {
                btn_previous.IsEnabled = false;
            }
            Task task = LoadUnconfirmedAccounts();
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
                int count = unConfirmedAccountsList.ItemsSource.OfType<object>().Count();

                if (count > 1 && count <= 10)
                {
                    lbl_noMoreResults.Text = "";
                    skipHowMany = skipHowMany + 10;
                    btn_next.IsEnabled = true;
                    await LoadUnconfirmedAccounts();
                }
                if (count < 10)
                {
                    lbl_noMoreResults.Text = "Ei lisää näytettäviä ilmoittautumisia!";
                    btn_next.IsEnabled = false;
                    await LoadUnconfirmedAccounts();
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
                await LoadUnconfirmedAccounts();
            }
            if (skipHowMany < 0)
            {
                lbl_noMoreResults.Text = "";
                skipHowMany = 10;
                btn_next.IsEnabled = true;
                await LoadUnconfirmedAccounts();
            }
            if (skipHowMany == 0)
            {
                btn_previous.IsEnabled = false;
                btn_next.IsEnabled = true;
            }

        }

        //***********************************************************************************************
        //ADMIN LOAD UNCONFIRMED USER ACCOUNTS
        //***********************************************************************************************
        private async Task LoadUnconfirmedAccounts()
        {
            unConfirmedAccountsList.ItemsSource = new string[] { "Ladataan hyväksymättömiä osallistumispyyntöjä..." };

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");

                AddUserModel getAllUnconfirmedAccounts = new AddUserModel
                {
                    AdminLogged = App._AdminLogged
                };

                string input = JsonConvert.SerializeObject(getAllUnconfirmedAccounts);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                HttpResponseMessage json = await client.PostAsync("/api/userinfo/adminlistunconfirmedaccounts", content);
                string reply = await json.Content.ReadAsStringAsync();
                var eventsData = JsonConvert.DeserializeObject<List<AddUserModel>>(reply);


                var allUnconfirmedUsersList = new List<AddUserModel>();
                foreach (var item in eventsData)
                {
                    allUnconfirmedUsersList.Add(new AddUserModel
                    {
                        LoginId = item.LoginId,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        City = item.City,
                        Email = item.Email
                    });
                }

                var sortOldestFirst = allUnconfirmedUsersList.OrderBy(x => x.Email)
                                                            .ToList();

                unConfirmedAccountsList.ItemsSource = sortOldestFirst.Skip(skipHowMany).Take(takeHowMany);

                //paging count here:
                var eventCount = Math.Ceiling((decimal)allUnconfirmedUsersList.Count / 10); //decimal values rounds up to the next whole number
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
                    btn_next.IsEnabled = false;
                    lbl_countDivider.Text = " / ";
                    lbl_pageCount.Text = lbl_eventCount.Text;
                }
                if (allUnconfirmedUsersList.Count < 10)
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
                unConfirmedAccountsList.ItemsSource = new string[] { error };
            }
        }


        //************************************************************************************
        //CONFIRM OR DELETE PARTICIPATION
        //************************************************************************************
        private async void ConfirmAccount(object s, SelectedItemChangedEventArgs e)
        {
            var obj = (AddUserModel)e.SelectedItem;

            bool confirm = await DisplayAlert("Käyttäjätilin hyväksyminen", "Haluatko hyväksyä käyttäjän \n " + obj.FirstName + " " + obj.LastName + "\n" + obj.City + "\n" + obj.Email, "Hyväksy", "Hylkää");

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("yourapiipaddress");

            if (confirm == true)
            {
                if (App._AdminLogged == true)
                {
                       AddUserModel confirmPart = new AddUserModel
                        {
                            LoginId = obj.LoginId,
                            AdminLogged = App._AdminLogged

                        };


                        string input = JsonConvert.SerializeObject(confirmPart);
                        StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                        HttpResponseMessage message = await client.PutAsync("/api/login/activateuseraccount", content);
                        string reply = await message.Content.ReadAsStringAsync();
                        bool success = JsonConvert.DeserializeObject<bool>(reply);

                        if (success)
                        {
                            await DisplayAlert("OK", "Käyttäjätili lisätty onnistuneesti!", "OK");
                            Task task = LoadUnconfirmedAccounts();
                        }
                        else
                        {
                            await DisplayAlert("Virhe", "Käyttäjätilin hyväksynnässä tapahtui virhe, yritä uudelleen", "OK");
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
                    bool confirmDelete = await DisplayAlert("Käyttäjätilin poisto", "Haluatko poistaa käyttäjätilin \n" + obj.FirstName + " " + obj.LastName + "\n" + obj.City + "\n" + obj.Email, "Kyllä", "Ei");

                    if (confirmDelete == true)
                    {
                        AddUserModel deleteUserAccount = new AddUserModel
                        {
                            LoginId = obj.LoginId,
                            AdminLogged = App._AdminLogged

                        };

                        string input = JsonConvert.SerializeObject(deleteUserAccount);
                        StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                        HttpResponseMessage message = await client.PostAsync("/api/login/admindeleteuser", content);
                        string reply = await message.Content.ReadAsStringAsync();
                        bool success = JsonConvert.DeserializeObject<bool>(reply);

                        if (success)
                        {
                            await DisplayAlert("OK", "Käyttäjätili hylätty onnistuneesti!", "OK");
                            Task task = LoadUnconfirmedAccounts();
                        }
                        else
                        {
                            await DisplayAlert("Virhe", "Käyttäjätilin hylkäyksessä tapahtui virhe, yritä uudelleen", "OK");
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
            await Navigation.PushAsync(new AdminPage());
        }

    }
}