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

namespace PursiX.Content.Admin.Users
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminListUserViewPage : ContentPage
    {
        public AdminListUserViewPage()
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasBackButton(this, false);
        }
        protected override void OnAppearing()
        {
            NavigationPage.SetHasBackButton(this, false);
            base.OnAppearing();
        }

        //User can not use phone's back button (iOS might be different case)
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        //Passing data to another page https://forums.xamarin.com/discussion/136704/how-to-get-the-id-of-selected-item-from-listview
        //*******************************************************************************************************************************
        private async void LoadUserEdit(object s, EventArgs e)
        {
            try
            {
                //*****************************************************************************************
                //getting data from login-table, is the user admin or not
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");

                AddUserModel getAdmin = new AddUserModel
                {
                    Email = input_emailAddress.Text,
                    AdminLogged = App._AdminLogged
                };

                string input = JsonConvert.SerializeObject(getAdmin);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                HttpResponseMessage json = await client.PostAsync("/api/login/adminisadmin", content);
                string reply = await json.Content.ReadAsStringAsync();
                var adminData = JsonConvert.DeserializeObject<bool>(reply);

                //*****************************************************************************************

                var selectedEvent = new AddUserModel
                {
                    FirstName = input_firstName.Text,
                    LastName = input_lastName.Text,
                    Address = input_address.Text,
                    PostalCode = input_postalCode.Text,
                    City = input_city.Text,
                    Phone = input_phoneNumber.Text,
                    Email = input_emailAddress.Text,
                    LoginId = Convert.ToInt32(lbl_loginId.Text),
                    Admin = adminData
                };

                var EditUserPage = new AdminModifyUserPage();
                EditUserPage.BindingContext = selectedEvent;
                await Navigation.PushAsync(EditUserPage);


            }
            catch
            {
                await DisplayAlert("Virhe", "Virhe tapahtui siirryttäessä käyttäjän muokkaukseen, ole hyvä ja yritä uudelleen", "OK");
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