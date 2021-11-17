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
    public partial class AdminModifyUserPage : ContentPage
    {
        public AdminModifyUserPage()
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

        //************************************************************************************
        //EDIT USER
        //************************************************************************************
        async void ModifyUser(object sender, EventArgs e)
        {

            if (App._AdminLogged == true)
            {
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
                    AddUserModel editUser = new AddUserModel();
                    editUser.FirstName = input_firstName.Text;
                    editUser.LastName = input_lastName.Text;
                    editUser.Address = input_address.Text;
                    editUser.PostalCode = input_postalCode.Text;
                    editUser.City = input_city.Text;
                    editUser.Phone = input_phoneNumber.Text;
                    editUser.Email = input_emailAddress.Text;
                    editUser.Admin = sw_isAdmin.IsToggled;
                    editUser.AdminLogged = App._AdminLogged;
                    if (String.IsNullOrEmpty(input_password.Text))
                    {
                    }
                    else
                    {
                        editUser.PassWord = input_password.Text;
                    }
                    editUser.LoginId = Convert.ToInt32(lbl_loginId.Text);
                    

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("yourapiipaddress");
                    string input = JsonConvert.SerializeObject(editUser);
                    StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                    HttpResponseMessage message = await client.PutAsync("/api/login/adminedituser", content);
                    string reply = await message.Content.ReadAsStringAsync();
                    bool success = JsonConvert.DeserializeObject<bool>(reply);

                    if (success)
                    {
                        await DisplayAlert("OK", "Käyttäjää " + input_firstName.Text + " " + input_lastName.Text + " muokattu onnistuneesti", "OK");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Virhe", "Käyttäjää ei voitu muokata, yritä uudelleen", "OK");
                    }

                }
            }
            else
            {
                await DisplayAlert("Virhe", "Et ole kirjautuneena sisään!", "OK");
                await Navigation.PopToRootAsync();
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