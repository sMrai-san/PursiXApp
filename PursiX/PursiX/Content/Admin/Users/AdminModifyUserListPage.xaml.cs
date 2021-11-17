using Newtonsoft.Json;
using PursiX.Models.Admin;
using PursiX.Models.User;
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
    public partial class AdminModifyUserListPage : ContentPage
    {
        //for pagination
        static int takeHowMany = 10;
        static int skipHowMany = 0;
        //for search
        private List<AddUserModel> usersToShow { get; set; }

        public AdminModifyUserListPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
        }

        protected override void OnAppearing()
        {
            pro_loading.IsRunning = true;
            pro_loading.IsVisible = true;
            NavigationPage.SetHasBackButton(this, false);
            if (skipHowMany == 0)
            {
                btn_previous.IsEnabled = false;
            }
            Task task = LoadUsers();
            base.OnAppearing();
        }

        //User can not use phone's back button (iOS might be different case)
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void InputSearch(object sender, TextChangedEventArgs e)
        {
            //https://www.c-sharpcorner.com/article/search-data-from-xamarin-forms-list-view/
            //you have to make a public list<Event> for this one to work...
            //*******************************************************************************
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                userList.ItemsSource = usersToShow.Skip(skipHowMany).Take(takeHowMany);
            }

            else
            {
                userList.ItemsSource = usersToShow.Where(x => x.LastName.ToLower().Contains(e.NewTextValue.ToLower()));
            }
        }

        //********************************************************************************************
        //NEXT AND PREVIOUS PAGINATION
        //********************************************************************************************
        private async void nextPage(object sender, EventArgs e)
        {


            if (skipHowMany >= 0)
            {
                btn_previous.IsEnabled = true;
                int count = userList.ItemsSource.OfType<object>().Count();

                if (count > 1 && count <= 10)
                {
                    lbl_noMoreResults.Text = "";
                    skipHowMany = skipHowMany + 10;
                    btn_next.IsEnabled = true;
                    await LoadUsers();
                }
                if (count < 10)
                {
                    lbl_noMoreResults.Text = "Ei lisää näytettäviä käyttäjiä!";
                    btn_next.IsEnabled = false;
                    await LoadUsers();
                }
                if (count == 0)
                {
                    lbl_noMoreResults.Text = "Ei lisää näytettäviä käyttäjiä!";
                    btn_next.IsEnabled = false;
                }
            }

        }
        private async void previousPage(object sender, EventArgs e)
        {
            if (skipHowMany > 0)
            {
                lbl_noMoreResults.Text = "";
                skipHowMany = skipHowMany - 10;
                btn_next.IsEnabled = true;
                await LoadUsers();
            }
            if (skipHowMany < 0)
            {
                lbl_noMoreResults.Text = "";
                skipHowMany = 10;
                btn_next.IsEnabled = true;
                await LoadUsers();
            }
            if (skipHowMany == 0)
            {
                btn_previous.IsEnabled = false;
                btn_next.IsEnabled = true;
            }

        }

        //***********************************************************************************************
        //ADMIN LOAD USERS
        //***********************************************************************************************
        private async Task LoadUsers()
        {
            userList.ItemsSource = new string[] { "Ladataan käyttäjiä..." };

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");

                AddUserModel getAllEvents = new AddUserModel
                {
                    AdminLogged = App._AdminLogged
                };

                string input = JsonConvert.SerializeObject(getAllEvents);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                HttpResponseMessage json = await client.PostAsync("/api/userinfo/adminlistusers", content);
                string reply = await json.Content.ReadAsStringAsync();
                var eventsData = JsonConvert.DeserializeObject<List<AddUserModel>>(reply);


                var allUsersList = new List<AddUserModel>();
                    foreach (var item in eventsData)
                    {
                        allUsersList.Add(new AddUserModel {
                            LoginId = item.LoginId,
                            FirstName = item.FirstName,
                            LastName = item.LastName,
                            Address = item.Address,
                            PostalCode = item.PostalCode,
                            City = item.City,
                            Phone = item.Phone,
                            Email = item.Email
                       });
                    }

                    //we need to sort our records from A-Ö
                    var sortOldestFirst = allUsersList.OrderBy(x => x.LastName)
                                                                .ToList();
                    //data to eventlist
                    userList.ItemsSource = sortOldestFirst.Skip(skipHowMany).Take(takeHowMany);

                //paging count here:
                var eventCount = Math.Ceiling((decimal)allUsersList.Count / 10); //decimal values rounds up to the next whole number
                lbl_eventCount.Text = eventCount.ToString();
                var pageCount = (skipHowMany / 10) + 1;
                if (pageCount > eventCount)
                {
                    lbl_pageCount.Text = eventCount.ToString();
                }
                if (pageCount == eventCount)
                {
                    btn_next.IsEnabled = false;
                    lbl_pageCount.Text = lbl_eventCount.Text;
                }
                else
                {
                    lbl_pageCount.Text = pageCount.ToString();
                }

                //same data to public list for search
                usersToShow = sortOldestFirst;


                    pro_loading.IsRunning = false;
                    pro_loading.IsVisible = false;
            }

            catch (Exception ex)
            {
                string error = ex.GetType().Name + ": " + ex.Message;
                userList.ItemsSource = new string[] { error };
            }
        }


        //Passing data to another page https://forums.xamarin.com/discussion/136704/how-to-get-the-id-of-selected-item-from-listview
        //*******************************************************************************************************************************
        private async void LoadUserEdit(object s, SelectedItemChangedEventArgs e)
        {
            try
            {
                var obj = (AddUserModel)e.SelectedItem;

                //*****************************************************************************************
                //getting data from login-table, is the user admin or not
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");
                
                AddUserModel getAdmin = new AddUserModel
                {
                    Email = obj.Email,
                    AdminLogged = App._AdminLogged
                };

                string input = JsonConvert.SerializeObject(getAdmin);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                HttpResponseMessage json = await client.PostAsync("/api/login/adminisadmin", content);
                string reply = await json.Content.ReadAsStringAsync();
                var adminData = JsonConvert.DeserializeObject<bool>(reply);

                //*****************************************************************************************

                var selectedUser = new AddUserModel
                {
                    FirstName = obj.FirstName,
                    LastName = obj.LastName,
                    Address = obj.Address,
                    PostalCode = obj.PostalCode,
                    City = obj.City,
                    Phone = obj.Phone,
                    Email = obj.Email,
                    LoginId = obj.LoginId,
                    Admin = adminData 
                };

                var EditUserPage = new AdminModifyUserPage();
                EditUserPage.BindingContext = selectedUser;
                await Navigation.PushAsync(EditUserPage);


            }
            catch
            {
                await DisplayAlert("Virhe", "Virhe tapahtui siirryttäessä käyttäjän muokkaukseen, ole hyvä ja yritä uudelleen", "OK");
            }



        }






        //****************************************************
        //CANCEL CLICK
        //****************************************************
        async void Cancel(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AdminManageUsers());
        }
    }
}