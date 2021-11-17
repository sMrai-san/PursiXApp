using Newtonsoft.Json;
using PursiX.Models.Admin;
using PursiX.Models.User;
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
    public partial class AdminAddUser : ContentPage
    {
        public AdminAddUser()
        {
            InitializeComponent();

            input_firstName.Focus();

            BindingContext = this;
            emailcheck_loading.IsVisible = false;
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


        //********************************************************************
        //Check if email already registered...
        //********************************************************************
        async void Entry_EmailCheck(object sender, FocusEventArgs e)
        {
            emailcheck_loading.IsVisible = true;
            AddRegistrationModel checkEmail = new AddRegistrationModel()
            {
                Email = input_emailAddress.Text
            };

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("yourapiipaddress");
            string input = JsonConvert.SerializeObject(checkEmail);
            StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
            HttpResponseMessage message = await client.PostAsync("/api/login/checkemail", content);
            string reply = await message.Content.ReadAsStringAsync();
            bool success = JsonConvert.DeserializeObject<bool>(reply);

            if (success)
            {
                emailcheck_loading.IsVisible = false;
                input_emailAddress.TextColor = Color.Green;
                //await DisplayAlert("Rekisteröinti", "Tätä sähköpostiosoitetta ei ole vielä rekisteröity", "OK");
            }
            else
            {
                emailcheck_loading.IsVisible = false;
                await DisplayAlert("Sähköposti", "Sähköpostiosoite on jo käytössä, syötä toinen sähköpostiosoite tai ota yhteys tukeen yourapplicationgmailaccount@gmail.com", "OK"); //dev purposes
                input_emailAddress.TextColor = Color.Red;
                input_emailAddress.Focus();
            }


        }






        //************************************************************************************
        //REGISTER BUTTON CLICK
        //************************************************************************************
        async void AddUser(object sender, EventArgs e)
        {
            //await DisplayAlert("Rekisteröitymistiedot", "Käyttäjän tiedot: " + " " + input_firstName.Text + " " + input_lastName.Text + " " + input_address.Text + " " + input_city.Text + " " + input_postalCode.Text + " " + input_phoneNumber.Text + " " + input_emailAddress.Text, "OK"); //dev purposes

            //if validation errors
            //ERROR HANDLING
            if (input_firstName.Text == "")
            {
                await DisplayAlert("Virhe", "Ole hyvä ja syötä etunimi", "OK");
                input_firstName.Focus();
            }
            else if (input_lastName.Text == "")
            {
                await DisplayAlert("Virhe", "Ole hyvä ja syötä sukunimi", "OK");
                input_lastName.Focus();
            }
            else if (input_address.Text == "")
            {
                await DisplayAlert("Virhe", "Ole hyvä ja syötä lähiosoite", "OK");
                input_address.Focus();
            }
            else if (input_postalCode.Text == "")
            {
                await DisplayAlert("Virhe", "Ole hyvä ja syötä postinumero", "OK");
                input_postalCode.Focus();
            }
            else if (input_city.Text == "")
            {
                await DisplayAlert("Virhe", "Ole hyvä ja syötä postitoimipaikka", "OK");
                input_city.Focus();
            }

            else if (input_phoneNumber.Text == "")
            {
                await DisplayAlert("Virhe", "Ole hyvä ja syötä puhelinnumero", "OK");
                input_phoneNumber.Focus();
            }
            else if (input_phoneNumber.TextColor == Color.Red)
            {
                await DisplayAlert("Virhe", "Ole hyvä ja syötä kelvollinen puhelinnumero", "OK");
                input_phoneNumber.Focus();
            }
            else if (input_emailAddress.Text == "")
            {
                await DisplayAlert("Virhe", "Ole hyvä ja syötä sähköpostiosoite", "OK");
                input_emailAddress.Focus();
            }
            else if (input_emailAddressAgain.Text == "")
            {
                await DisplayAlert("Virhe", "Ole hyvä ja syötä sähköpostiosoite uudelleen.", "OK");
                input_emailAddress.Focus();
            }
            else if (input_emailAddressAgain.Text != input_emailAddress.Text)
            {
                await DisplayAlert("Virhe", "Sähköpostiosoitteet eivät täsmää", "OK");
                input_emailAddressAgain.Focus();
            }
            else if (input_password.Text == "")
            {
                await DisplayAlert("Virhe", "Syötä haluttu salasana", "OK");
                input_password.Focus();
            }
            else if (input_passwordAgain.Text == "")
            {
                await DisplayAlert("Virhe", "Ole hyvä ja syötä salasana uudelleen.", "OK");
                input_passwordAgain.Focus();
            }
            else if (input_passwordAgain.Text != input_password.Text)
            {
                await DisplayAlert("Virhe", "Syötetyt salasanat eivät täsmää", "OK");
                input_passwordAgain.Focus();
            }

            //if all the input fields are valid we can continue
            else
            {
                AddUserModel newUser = new AddUserModel()
                {
                    FirstName = input_firstName.Text,
                    LastName = input_lastName.Text,
                    Address = input_address.Text,
                    PostalCode = input_postalCode.Text,
                    City = input_city.Text.ToUpper(),
                    Phone = input_phoneNumber.Text.ToString(),
                    Email = input_emailAddress.Text,
                    PassWord = input_password.Text,
                    Admin = sw_isAdmin.IsToggled,
                    Confirmed = true,
                    AdminLogged = App._AdminLogged
                    
                };

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");
                string input = JsonConvert.SerializeObject(newUser);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PostAsync("/api/login/adminadduser", content);
                string reply = await message.Content.ReadAsStringAsync();
                bool success = JsonConvert.DeserializeObject<bool>(reply);

                if (success)
                {
                    await DisplayAlert("Käyttäjän lisäys", "Käyttäjä " + input_emailAddress.Text + " lisätty onnistuneesti.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Virhe", "Käyttäjän lisäys epäonnistui, yritä uudelleen tai ota yhteys tukeen:\nyourapplicationgmailaccount@gmail.com", "OK");
                }
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