using Newtonsoft.Json;
using PursiX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PursiX.Content.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminLoginPage : ContentPage
    {
        public AdminLoginPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
        }

        protected override void OnAppearing()
        {
            pro_loading.IsVisible = false;
            inputUserName.Focus();
            NavigationPage.SetHasBackButton(this, false);
            base.OnAppearing();
        }

        //User can not use phone's back button (iOS might be different case)
        protected override bool OnBackButtonPressed()
        {
            return true;
        }


        //******************************************************************************
        //ON LOGIN CLICK
        //******************************************************************************
        async void Login(object sender, EventArgs e)
        {
            pro_loading.IsVisible = true;

            //connection to API
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("yourapiipaddress");

            try
            {
                Login logindata = new Login()
                {
                    Password = inputPassword.Text,
                    Email = inputUserName.Text

                };

                if (inputUserName.Text != null && inputPassword != null)
                {
                    //Api Post -> return bool
                    string input = JsonConvert.SerializeObject(logindata);
                    StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                    HttpResponseMessage message = await client.PostAsync("/api/login/adminlogin", content);
                    string reply = await message.Content.ReadAsStringAsync();
                    bool success = JsonConvert.DeserializeObject<bool>(reply);


                    try
                    {
                        //if user inputted admin username and password is right
                        if (success)
                        {
                            App._AdminLogged = true;
                            await Navigation.PushAsync(new AdminPage());
                            
                        }
                        //if user inputted admin username and password is wrong
                        else
                        {
                            await DisplayAlert("Kirjautumisvirhe", "Käyttäjänimi tai salasana on väärin, ole hyvä ja yritä uudelleen", "OK");
                            pro_loading.IsVisible = false;
                        }
                    }

                    //catch connection or input is faulty to API
                    catch (Exception ex)
                    {
                        string error = ex.GetType().Name + ": " + ex.Message;
                        await DisplayAlert("Virhe", error, "OK");
                        pro_loading.IsVisible = false;
                    }
                }

                else
                {
                    await DisplayAlert("Syöte ei kelpaa", "Yksi tai useampi kentistä on tyhjänä, täytä kentät ja yritä uudelleen", "OK");
                    pro_loading.IsVisible = false;
                }

            }
            catch (Exception ex)
            {
                string error = ex.GetType().Name + ": " + ex.Message;
                await DisplayAlert("Virhe", error, "OK");
                pro_loading.IsVisible = false;
            }

        }



        //******************************************************************************
        //ON CANCEL CLICK
        //******************************************************************************
        async void Cancel(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}