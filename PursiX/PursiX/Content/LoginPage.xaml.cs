using Newtonsoft.Json;
using PursiX.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PursiX.Content
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            inputUserName.Focus();
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
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("yourapiipaddress");

            pro_loading.IsVisible = true;


            try
            {
                Login logindata = new Login()
                {
                    UserName = inputUserName.Text,
                    Password = inputPassword.Text,
                    Email = inputUserName.Text

                };

                if (inputUserName.Text != null && inputPassword != null)
                {

                    string input = JsonConvert.SerializeObject(logindata);
                    StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                    HttpResponseMessage message = await client.PostAsync("/api/login", content);
                    string reply = await message.Content.ReadAsStringAsync();
                    bool success = JsonConvert.DeserializeObject<bool>(reply);


                    try
                    {
                        //if user inputted username and password is right
                        if (success)
                        {
                            string checkConfirmationinput = JsonConvert.SerializeObject(logindata);
                            StringContent checkConfirmcontent = new StringContent(checkConfirmationinput, Encoding.UTF8, "application/json");
                            HttpResponseMessage checkConfirmmessage = await client.PostAsync("/api/login/checkconfirmation", checkConfirmcontent);
                            string confirmReply = await checkConfirmmessage.Content.ReadAsStringAsync();
                            bool confirmSucces = JsonConvert.DeserializeObject<bool>(confirmReply);

                            if (confirmSucces)
                            {

                                Login checkActivationData = new Login()
                                {
                                    UserName = inputUserName.Text,
                                    Email = inputUserName.Text

                                };
                                string checkActivationInput = JsonConvert.SerializeObject(checkActivationData);
                                StringContent checkActivationContent = new StringContent(checkActivationInput, Encoding.UTF8, "application/json");
                                HttpResponseMessage activationMessage = await client.PostAsync("/api/login/checkactivation", checkActivationContent);
                                string activationReply = await activationMessage.Content.ReadAsStringAsync();
                                bool activationSuccess = JsonConvert.DeserializeObject<bool>(activationReply);

                                //if account is activated user is being logged in
                                if (activationSuccess)
                                {
                                    try
                                    {
                                        /*await DisplayAlert("Kirjautuminen", "Kirjautuminen onnistui", "OK");*/ //dev purposes

                                        //get logged id to global variable
                                        Login getId = new Login()
                                        {
                                            UserName = inputUserName.Text,
                                        };

                                        string inputId = JsonConvert.SerializeObject(logindata);
                                        StringContent contentId = new StringContent(inputId, Encoding.UTF8, "application/json");
                                        HttpResponseMessage fetchId = await client.PostAsync("/api/login/getuserid", contentId);
                                        string replyId = await fetchId.Content.ReadAsStringAsync();




                                        //global variable for user being logged in
                                        App._IsLogged = true;
                                        App._userId = Convert.ToInt32(replyId);

                                        //binding user data to EventsPage
                                        var eventsPage = new EventsPage();
                                        eventsPage.BindingContext = checkActivationData;
                                        await Navigation.PushAsync(eventsPage);


                                    }
                                    catch (Exception ex)
                                    {
                                        string error = ex.GetType().Name + ": " + ex.Message;
                                        await DisplayAlert("Virhe", "Kirjautumisen aikana tapahtui virhe. Virhetiedot: " + error, "OK");
                                        pro_loading.IsVisible = false;
                                    }


                                }


                                //if account has not been activated
                                else
                                {
                                    //await DisplayAlert("Kirjautuminen", "Siirrytään tilin aktivointisivulle", "OK"); //dev purposes
                                    string activationCode = await ActivationCode(this.Navigation);

                                    Login activationData = new Login()
                                    {
                                        Email = inputUserName.Text,
                                        VerificationCode = Convert.ToInt32(activationCode)
                                    };

                                    string activationInput = JsonConvert.SerializeObject(activationData);
                                    StringContent ActivationContent = new StringContent(activationInput, Encoding.UTF8, "application/json");
                                    HttpResponseMessage verification = await client.PutAsync("/api/login/activateaccount", ActivationContent);
                                    string verificationReply = await verification.Content.ReadAsStringAsync();
                                    bool verificationSuccess = JsonConvert.DeserializeObject<bool>(verificationReply);

                                    //if user input for verification code is valid
                                    if (verificationSuccess)
                                    {
                                        await DisplayAlert("Aktivointi", "Tilin " + inputUserName.Text + " aktivointi onnistui! Siirry takaisin kirjautumissivulle kirjautuaksesi sisään", "OK");
                                        await Navigation.PopAsync();

                                    }
                                    else
                                    {
                                        await DisplayAlert("Aktivointi", "Tilin " + inputUserName.Text + " aktivointi epäonnistui! Ole hyvä ja yritä uudelleen", "OK");
                                        await Navigation.PopAsync();
                                    }
                                }
                            }
                            else
                            {
                                await DisplayAlert("Aktivointi", "Ylläpitäjä ei ole vielä hyväksynyt käyttäjää " + inputUserName.Text + ". Ole hyvä ja odota, tai ota yhteys ylläpitoon yourapplicationgmailaccount@gmail.com", "OK");
                                await Navigation.PopAsync();
                            }


                        }
                        //if user inputted username and password is wrong
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
                    await DisplayAlert("Syöte ei kelpaa", "Yksi tai useampi kentistä oli tyhjänä, täytä kentät ja yritä uudelleen", "OK");
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




        //***********************************************************************************************
        //https://forums.xamarin.com/discussion/35838/how-to-do-a-simple-inputbox-dialog
        //***********************************************************************************************
        public static Task<string> ActivationCode(INavigation navigation)
        {
            // user comment input
            var tcs = new TaskCompletionSource<string>();

            var lblCode = new Label { Text = "Aktivointikoodi", FontAttributes = FontAttributes.Bold, FontSize = 14, BackgroundColor = Color.FromRgb(56, 166, 84), Padding = 15, TextColor = Color.White };
            var lblMessage = new Label { Text = "Ole hyvä ja syötä aktivointikoodisi allaolevaan tekstikenttään: ", FontAttributes = FontAttributes.Italic };
            var lblLine = new BoxView { HeightRequest = 1, BackgroundColor = Color.FromRgb(0, 0, 0), HorizontalOptions = LayoutOptions.FillAndExpand };
            var txtInput = new Entry { Text = "", WidthRequest = 200, BackgroundColor = Color.FromRgb(220, 230, 224), Keyboard = Keyboard.Numeric, MaxLength = 4 };

            var btnOk = new Button
            {
                Text = "Aktivoi",
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
                Children = { lblCode, lblMessage, txtInput, slButtons },
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



        //******************************************************************************
        //ON CANCEL CLICK
        //******************************************************************************
        async void Cancel(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }


    }
}