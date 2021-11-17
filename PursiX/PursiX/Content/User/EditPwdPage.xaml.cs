using Newtonsoft.Json;
using PursiX.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PursiX.Content.User
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditPwdPage : ContentPage
    {
        public EditPwdPage()
        {
            InitializeComponent();
            input_oldpassword.Focus();
            NavigationPage.SetHasBackButton(this, false);
        }

        //User can not use phone's back button (iOS might be different case)
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        //************************************************************************************
        //UPDATE PASSWORD BUTTON CLICK
        //************************************************************************************
        async void ChangePwd(object sender, EventArgs e)
        {
            if (input_oldpassword.Text != "")
            {

                if (input_password.Text == "")
                {
                    await DisplayAlert("Virhe", "Ole hyvä ja syötä uusi salasana", "OK");
                    input_password.Focus();
                }
                else if (input_passwordAgain.Text == "")
                {
                    await DisplayAlert("Virhe", "Ole hyvä ja syötä uusi salasana uudelleen.", "OK");
                    input_passwordAgain.Focus();
                }
                else if (input_passwordAgain.Text != input_password.Text)
                {
                    await DisplayAlert("Virhe", "Syötetyt salasanat eivät täsmää", "OK");
                    input_passwordAgain.Focus();
                }
                else
                {
                    try
                    {
                        EditLoginModel changeUsrPwd = new EditLoginModel()
                        {
                            LoginId = App._userId,
                            PassWord = input_oldpassword.Text,
                            NewPassWord = input_password.Text
                        };

                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("yourapiipaddress");
                        string input = JsonConvert.SerializeObject(changeUsrPwd);
                        StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                        HttpResponseMessage message = await client.PutAsync("/api/login/pwdchange", content);
                        string reply = await message.Content.ReadAsStringAsync();
                        bool success = JsonConvert.DeserializeObject<bool>(reply);

                        if (success)
                        {
                            await DisplayAlert("Salasanan vaihto", "Salasana vaihdettu onnistuneesti!", "OK");
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            await DisplayAlert("Virhe", "Salasanan vaihto epäonnistui, ole hyvä ja yritä uudelleen", "OK");
                        }
                    }
                    catch
                    {
                        await DisplayAlert("Virhe", "Salasanan vaihto epäonnistui, ole hyvä ja yritä uudelleen", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Virhe", "Ole hyvä ja syötä vanha salasana", "OK");
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