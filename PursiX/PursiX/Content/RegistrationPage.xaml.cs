using Newtonsoft.Json;
using PursiX.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PursiX.Content
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage()
        {
            InitializeComponent();

            input_firstName.Focus();

            BindingContext = this;
            emailcheck_loading.IsVisible = false;
            NavigationPage.SetHasBackButton(this, false);
        }

        //User can not use phone's back button (iOS might be different case)
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        //Check if email already registered...
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
        async void Register(object sender, EventArgs e)
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
                //https://stackoverflow.com/questions/33749543/unique-4-digit-random-number-in-c-sharp
                //Generates random number for 4-digit activation code
                int _min = 1000;
                int _max = 9999;
                Random _rdm = new Random();
                int veriCode = _rdm.Next(_min, _max);

                AddRegistrationModel newUser = new AddRegistrationModel()
                {
                    FirstName = input_firstName.Text,
                    LastName = input_lastName.Text,
                    Address = input_address.Text,
                    PostalCode = input_postalCode.Text,
                    City = input_city.Text.ToUpper(),
                    Phone = input_phoneNumber.Text.ToString(),
                    Email = input_emailAddress.Text,
                    PassWord = input_password.Text,
                    VerificationCode = veriCode,
                    Confirmed = false
                };

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");
                string input = JsonConvert.SerializeObject(newUser);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PostAsync("/api/login/register", content);
                string reply = await message.Content.ReadAsStringAsync();
                bool success = JsonConvert.DeserializeObject<bool>(reply);

                if (success)
                {
                    await DisplayAlert("Rekisteröinti", "Rekisteröintitiedot tallennettu onnistuneesti! Sähköpostivarmistus lähetetty osoitteeseen: " + input_emailAddress.Text + " seuraa sähköpostissa olevia ohjeita aktivoidaksesi tunnuksesi.", "OK");

                    //*********************
                    //VERIFICATION EMAIL
                    //*********************

                    try
                    {
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                        mail.From = new MailAddress("yourapplicationgmailaccount@gmail.com");
                        mail.To.Add(input_emailAddress.Text/*"yourapplicationgmailaccount@gmail.com"*/);
                        mail.Subject = "PursiX tilin aktivointi";
                        mail.Body =
                            "Ohjelmiston ylläpito aktivoi tilisi mahdollisimman nopeasti, jonka jälkeen ohjelmisto on käytettävissänne. \n\n\n\n" +
                            "Aktivointikoodisi on: " + veriCode.ToString() + ".\n\n" + //when user has paid the application fee, user could get the activation code from that way too, but until implemented into use we will use this.
                            "Aktivointikoodi tulee syöttää ensimmäisellä kirjautumiskerralla PursiX -ohjelmistoon.\n\n" +
                            "PursiX -ohjelmistoon tulee kirjautua sähköpostiosoitteella " + input_emailAddress.Text + " ja syöttämällä tallentamasi salasana.\n\n\n\n" +
                            "Vikatilanteissa ota yhteys ylläpitoon sähköpostiosoitteeseen yourapplicationgmailaccount@gmail.com";

                        SmtpServer.Port = 587;
                        SmtpServer.Host = "smtp.gmail.com";
                        SmtpServer.EnableSsl = true;
                        SmtpServer.UseDefaultCredentials = false;
                        SmtpServer.Credentials = new System.Net.NetworkCredential("yourgmailaccount", "yourgmailpassword");

                        SmtpServer.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Virhe", ex.Message + ". Virhe lähetettäessä aktivointisähköpostia", "OK");
                    }




                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Virhe", "Rekisteröintitietojen vienti epäonnistui, yritä uudelleen tai ota yhteys tukeen: yourapplicationgmailaccount@gmail.com", "OK");
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