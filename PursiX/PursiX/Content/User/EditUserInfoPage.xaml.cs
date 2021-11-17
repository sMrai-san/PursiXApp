using Newtonsoft.Json;
using PursiX.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PursiX.Content.User
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditUserInfoPage : ContentPage
    {
        public EditUserInfoPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
        }

        //User can not use phone's back button (iOS might be different case)
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        //input handling (allow only letters,  )
        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var isValid = Regex.IsMatch(e.NewTextValue, "^[a-öA-Ö- ]+$");

            if (e.NewTextValue.Length > 0)
            {
                ((Xamarin.Forms.Entry)sender).Text = isValid ? e.NewTextValue : e.NewTextValue.Remove(e.NewTextValue.Length - 1);
            }
        }


        //************************************************************************************
        //UPDATE USERINFO BUTTON
        //************************************************************************************
        async void UpdateUserInfo(object sender, EventArgs e)
        {
            //if validation errors occurred in email or (Finnish) phonenumber field
            if (input_firstName.TextColor != Color.Red
                && input_lastName.TextColor != Color.Red
                && input_address.TextColor != Color.Red
                && input_postalCode.TextColor != Color.Red
                && input_city.TextColor != Color.Red
                && input_phoneNumber.TextColor != Color.Red
                )
            {
                if (App._IsLogged == true)
                {
                    //input fields must have something in them
                    if (input_firstName.Text == "")
                    {
                        await DisplayAlert("Virhe", "Syötä etunimi jatkaaksesi", "OK");
                        input_firstName.Focus();
                    }
                    else if (input_lastName.Text == "")
                    {
                        await DisplayAlert("Virhe", "Syötä sukunimi jatkaaksesi", "OK");
                        input_lastName.Focus();
                    }
                    else if (input_address.Text == "")
                    {
                        await DisplayAlert("Virhe", "Syötä lähiosoite jatkaaksesi", "OK");
                        input_address.Focus();
                    }
                    else if (input_postalCode.Text == "")
                    {
                        await DisplayAlert("Virhe", "Syötä postinumero jatkaaksesi", "OK");
                        input_postalCode.Focus();
                    }
                    else if (input_city.Text == "")
                    {
                        await DisplayAlert("Virhe", "Syötä paikkakunta jatkaaksesi", "OK");
                        input_city.Focus();
                    }
                    else if (input_phoneNumber.Text == "")
                    {
                        await DisplayAlert("Virhe", "Syötä puhelinnumero jatkaaksesi", "OK");
                        input_phoneNumber.Focus();
                    }
                    //if all the input fields are valid we can continue
                    else
                    {
                        EditUserInfoModel updateUser = new EditUserInfoModel()
                        {
                            LoginId = App._userId,
                            Email = input_emailAddress.Text,
                            FirstName = input_firstName.Text,
                            LastName = input_lastName.Text,
                            Address = input_address.Text,
                            PostalCode = input_postalCode.Text,
                            City = input_city.Text.ToUpper(),
                            Phone = input_phoneNumber.Text.ToString(),
                        };

                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("yourapiipaddress");
                        string input = JsonConvert.SerializeObject(updateUser);
                        StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                        HttpResponseMessage message = await client.PutAsync("/api/userinfo/updateuserinfo", content);
                        string reply = await message.Content.ReadAsStringAsync();
                        bool success = JsonConvert.DeserializeObject<bool>(reply);

                        if (success)
                        {
                            await DisplayAlert("OK", "Yhteystiedot tallennettu onnistuneesti", "OK");
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            await DisplayAlert("Virhe", "Yhteystietojen tallennuksessa tapahtui virhe, ole hyvä ja yritä uudelleen", "OK");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Virhe", "Et ole kirjautunut sisään, siirrytään aloitussivulle!", "OK");
                    await Navigation.PopToRootAsync();
                }
            }
            else
            {
                await DisplayAlert("Virhe", "Ole hyvä ja tarkasta punaisella merkityt kentät ja yritä tallentaa yhteystiedot uudelleen", "OK");
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