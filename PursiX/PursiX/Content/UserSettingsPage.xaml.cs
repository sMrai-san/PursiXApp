using Newtonsoft.Json;
using PursiX.Content.User;
using PursiX.Models;
using PursiX.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PursiX.Content
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserSettingsPage : ContentPage
    {
        public UserSettingsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
        }

        protected override void OnAppearing()
        {
            grid_edituserinfo.Opacity = 1;
            grid_changepwd.Opacity = 1;
            grid_contact.Opacity = 1;
            grid_info.Opacity = 1;
            grid_cancel.Opacity = 1;

            NavigationPage.SetHasBackButton(this, false);
            base.OnAppearing();
        }

        //User can not use phone's back button (iOS might be different case)
        protected override bool OnBackButtonPressed()
        {
            return true;
        }


        //***********************************************************************************************
        //MODAL FOR CONTACT INFO
        //***********************************************************************************************
        public static Task<string> contactInfoTask(INavigation navigation)
        {
            //contact info
            var tcs = new TaskCompletionSource<string>();

            var lblLine0 = new BoxView { HeightRequest = 1, BackgroundColor = Color.FromRgb(0, 0, 0), HorizontalOptions = LayoutOptions.FillAndExpand };
            var lblEventName = new Label { Text = "Yhteystiedot", FontAttributes = FontAttributes.Bold, FontSize = 18, Padding = 15 };
            var lblLine1 = new BoxView { HeightRequest = 1, BackgroundColor = Color.FromRgb(0, 0, 0), HorizontalOptions = LayoutOptions.FillAndExpand };
            var lblContact = new Label { Text = "PursiX sähköpostiosoite:\nyourapplicationgmailaccount@gmail.com\n\nGooglePlay:\nwww.google.com", HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, Margin = 15 };

            var btnCancel = new Button
            {
                Text = "OK",
                WidthRequest = 85,
                TextColor = Color.White,
                BackgroundColor = Color.FromRgb(56, 166, 84)
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
                Children = { btnCancel },
            };

            var layout = new StackLayout
            {
                Padding = new Thickness(0, 40, 0, 0),
                Margin = new Thickness(2, 0, 2, 0),
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Vertical,
                Children = { lblLine0, lblEventName, lblLine1, lblContact, slButtons },
            };

            // create and show page
            var page = new ContentPage();
            page.Content = layout;
            navigation.PushModalAsync(page);

            return tcs.Task;
        }

        //***********************************************************************************************
        //EDIT USERINFO 
        //***********************************************************************************************
        async void LoadUserEdit(object s, EventArgs e)
        {
            grid_edituserinfo.Opacity = 1;
            await grid_edituserinfo.FadeTo(0, 100);



            try
            {
                EditUserInfoModel getUserInfo = new EditUserInfoModel
                {
                    LoginId = App._userId,
                    isLogged = App._IsLogged
                };

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");
                string input = JsonConvert.SerializeObject(getUserInfo);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                HttpResponseMessage json = await client.PostAsync("/api/userinfo/getuserinfo/", content);
                string reply = await json.Content.ReadAsStringAsync();
                var userData = JsonConvert.DeserializeObject<List<EditUserInfoModel>>(reply);

                var userInfoData = new EditUserInfoModel();
                foreach (var item in userData)
                {
                    userInfoData.LoginId = item.LoginId;
                    userInfoData.FirstName = item.FirstName;
                    userInfoData.LastName = item.LastName;
                    userInfoData.Address = item.Address;
                    userInfoData.PostalCode = item.PostalCode;
                    userInfoData.City = item.City;
                    userInfoData.Phone = item.Phone.ToString();
                    userInfoData.Email = item.Email;
                };

                //await DisplayAlert("OK", "Tiedot haettiin onnistuneesti.", "OK");

                var EditUserInfo = new EditUserInfoPage();
                    EditUserInfo.BindingContext = userInfoData;
                    await Navigation.PushAsync(EditUserInfo);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", "Tapahtui virhe siirryttäessä yhteystietojen muokkaukseen. Ole hyvä ja yritä uudelleen." + ex.Message, "OK");
            }
        }

        //***********************************************************************************************
        //EDIT PWD
        //***********************************************************************************************
        async void LoadPasswordEdit(object s, EventArgs e)
        {
            grid_changepwd.Opacity = 1;
            await grid_changepwd.FadeTo(0, 100);
            await Navigation.PushAsync(new EditPwdPage());
        }

        //****************************************************
        //CONTACT CLICK
        //****************************************************
        async void openContact(object sender, EventArgs e)
        {
            grid_contact.Opacity = 1;
            await grid_contact.FadeTo(0, 100);
            await contactInfoTask(this.Navigation);
        }

        //****************************************************
        //INFO CLICK
        //****************************************************
        async void continueToInfo(object sender, EventArgs e)
        {
            grid_info.Opacity = 1;
            await grid_info.FadeTo(0, 100);
            await Navigation.PushAsync(new InfoPage());
        }

        //****************************************************
        //CANCEL CLICK
        //****************************************************
        async void Cancel(object sender, EventArgs e)
        {
            grid_cancel.Opacity = 1;
            await grid_cancel.FadeTo(0, 100);
            await Navigation.PopAsync();
        }

        //****************************************************
        //LOGOUT CLICK
        //****************************************************
        async void Logout(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Kirjaudu ulos?", "Haluatko varmasti kirjautua ulos?", "Kyllä", "Ei");

            if (confirm == true)
            {
                App._IsLogged = false;
                await Navigation.PopToRootAsync();
            }
            else
            {

            }
        }
    }
}