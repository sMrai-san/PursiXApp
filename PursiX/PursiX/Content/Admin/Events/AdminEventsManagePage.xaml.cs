using PursiX.Content.Admin.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PursiX.Content.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminEventsManagePage : ContentPage
    {
        public AdminEventsManagePage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
        }

        protected override void OnAppearing()
        {
            //styling issues
            grid_cancel.Opacity = 1;
            grid_addEvent.Opacity = 1;
            grid_deleteEvent.Opacity = 1;
            grid_listEvents.Opacity = 1;
            grid_modifyEvents.Opacity = 1;

            NavigationPage.SetHasBackButton(this, false);
            base.OnAppearing();
        }

        //User can not use phone's back button (iOS might be different case)
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        //****************************************************
        //ADD EVENT CLICK
        //****************************************************
        async void continueToAddEvent(object sender, EventArgs e)
        {
            grid_addEvent.Opacity = 1;
            await grid_addEvent.FadeTo(0, 100);
            await Navigation.PushAsync(new AdminAddEventMapPage());
        }

        //****************************************************
        //MODIFY EVENT CLICK
        //****************************************************
        async void continueToModifyEvent(object sender, EventArgs e)
        {
            grid_modifyEvents.Opacity = 1;
            await grid_modifyEvents.FadeTo(0, 100);
            await Navigation.PushAsync(new AdminModifyEventListPage());
        }

        //****************************************************
        //DELETE EVENT CLICK
        //****************************************************
        async void continueToDeleteEvent(object sender, EventArgs e)
        {
            grid_deleteEvent.Opacity = 1;
            await grid_deleteEvent.FadeTo(0, 100);
            await Navigation.PushAsync(new AdminDeleteEventPage());
        }

        //****************************************************
        //LIST ALL EVENTS CLICK
        //****************************************************
        async void continueToAllEvents(object sender, EventArgs e)
        {
            grid_listEvents.Opacity = 1;
            await grid_listEvents.FadeTo(0, 100);
            await Navigation.PushAsync(new AdminListAllEventsPage());
        }



        //****************************************************
        //CANCEL CLICK
        //****************************************************
        async void Cancel(object sender, EventArgs e)
        {
            grid_cancel.Opacity = 1;
            await grid_cancel.FadeTo(0, 100);
            await Navigation.PushAsync(new AdminPage());
        }
    }
}