using Newtonsoft.Json;
using PursiX.Content.Admin.Participations;
using PursiX.Content.Admin.UserRegistration;
using PursiX.Content.Admin.Users;
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
    public partial class AdminPage : ContentPage
    {
        public AdminPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
        }

        protected override void OnAppearing()
        {
            //styling issues
            grid_cancel.Opacity = 1;
            grid_events.Opacity = 1;
            grid_partEdit.Opacity = 1;
            grid_participants.Opacity = 1;
            grid_users.Opacity = 1;
            grid_confirmRegistration.Opacity = 1;

            NavigationPage.SetHasBackButton(this, false);
            Task task = GetUnconfirmedCount();
            Task task2 = GetUnconfirmedUsersCount();
            base.OnAppearing();
        }

        //User can not use phone's back button (iOS might be different case)
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        async Task<string> GetUnconfirmedCount()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");
                Task<string> json = client.GetStringAsync("/api/participant/unconfirmed");
                var eventsData = JsonConvert.DeserializeObject<int>(await json);

                grid_pCount.Text = eventsData.ToString();
                return eventsData.ToString();
            }
            catch (Exception ex)
            {
                string error = ex.GetType().Name + ": " + ex.Message;
                await DisplayAlert("Virhe", error, "OK");
                return null;
            }

        }
        async Task<string> GetUnconfirmedUsersCount()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");
                Task<string> json = client.GetStringAsync("/api/login/unconfirmedusers");
                var userData = JsonConvert.DeserializeObject<int>(await json);

                grid_rCount.Text = userData.ToString();
                return userData.ToString();
            }
            catch (Exception ex)
            {
                string error = ex.GetType().Name + ": " + ex.Message;
                await DisplayAlert("Virhe", error, "OK");
                return null;
            }

        }

        //****************************************************
        //EVENTS CLICK
        //****************************************************
        async void continueToEventManagement(object sender, EventArgs e)
        {
            grid_events.Opacity = 1;
            await grid_events.FadeTo(0, 100);
            await Navigation.PushAsync(new AdminEventsManagePage());
        }

        //****************************************************
        //USER MANAGEMENT CLICK
        //****************************************************
        async void continueToUserManagement(object sender, EventArgs e)
        {
            grid_users.Opacity = 1;
            await grid_users.FadeTo(0, 100);
            await Navigation.PushAsync(new AdminManageUsers());
        }

        //****************************************************
        //PARTICIPATION CONFIRM CLICK
        //****************************************************
        async void continueToParticipationConfirm(object sender, EventArgs e)
        {
            grid_participants.Opacity = 1;
            await grid_participants.FadeTo(0, 100);
            await Navigation.PushAsync(new AdminParticipationConfirmPage());
        }

        //****************************************************
        //PARTICIPATION CONFIRM CLICK
        //****************************************************
        async void continueToParticipationEdit(object sender, EventArgs e)
        {
            grid_partEdit.Opacity = 1;
            await grid_partEdit.FadeTo(0, 100);
            await Navigation.PushAsync(new AdminParticipationManagePage());
        }

        //****************************************************
        //REGISTRATION CONFIRM CLICK
        //****************************************************
        async void continueToUserConfirmation(object sender, EventArgs e)
        {
            grid_confirmRegistration.Opacity = 1;
            await grid_confirmRegistration.FadeTo(0, 100);
            await Navigation.PushAsync(new AdminConfirmUserRegistrationPage());
        }


        //****************************************************
        //CANCEL CLICK
        //****************************************************
        async void Cancel(object sender, EventArgs e)
        {
            grid_cancel.Opacity = 1;
            await grid_cancel.FadeTo(0, 100);
            await Navigation.PopToRootAsync();
        }
    }
}