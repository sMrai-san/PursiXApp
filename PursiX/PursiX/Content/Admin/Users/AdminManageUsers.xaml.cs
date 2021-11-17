using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PursiX.Content.Admin.Users
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminManageUsers : ContentPage
    {
        public AdminManageUsers()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
        }

        protected override void OnAppearing()
        {
            //styling issues
            grid_cancel.Opacity = 1;
            grid_addUser.Opacity = 1;
            grid_modifyUser.Opacity = 1;
            grid_deleteUser.Opacity = 1;
            grid_listUsers.Opacity = 1;

            NavigationPage.SetHasBackButton(this, false);
            base.OnAppearing();
        }

        //User can not use phone's back button (iOS might be different case)
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        //****************************************************
        //ADD USER CLICK
        //****************************************************
        async void AddUser(object sender, EventArgs e)
        {
            grid_addUser.Opacity = 1;
            await grid_addUser.FadeTo(0, 100);
            await Navigation.PushAsync(new AdminAddUser());
        }

        //****************************************************
        //EDIT USER CLICK
        //****************************************************
        async void ModifyUser(object sender, EventArgs e)
        {
            grid_modifyUser.Opacity = 1;
            await grid_modifyUser.FadeTo(0, 100);
            await Navigation.PushAsync(new AdminModifyUserListPage());
        }


        //****************************************************
        //DELETE USER CLICK
        //****************************************************
        async void DeleteUser(object sender, EventArgs e)
        {
            grid_deleteUser.Opacity = 1;
            await grid_deleteUser.FadeTo(0, 100);
            await Navigation.PushAsync(new AdminDeleteUserPage());
        }

        //****************************************************
        //LIST USERS CLICK
        //****************************************************
        async void ListUsers(object sender, EventArgs e)
        {
            grid_listUsers.Opacity = 1;
            await grid_listUsers.FadeTo(0, 100);
            await Navigation.PushAsync(new AdminListUsersPage());
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