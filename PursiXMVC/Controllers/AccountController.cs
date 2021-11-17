using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using PursiXMVC.Data;
using PursiXMVC.Models;
using PursiXMVC.Models.User;

namespace PursiXMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly PursiDBContext _context;

        public AccountController(PursiDBContext context)
        {
            _context = context;
        }



        public IActionResult Index()
        {
            if (TempData["success"] != null)
            {
                ViewBag.success = TempData["success"].ToString();
            }
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Activation(RegisterViewModel emailData)
        {
            if (TempData["error"] != null)
            {
                ViewBag.error = TempData["error"].ToString();
            }
            return View(emailData);
        }

        //*************************************************************************
        //EMAIL ACTIVATION
        //*************************************************************************

        [HttpPost]
        public async Task<IActionResult> Activation(string username, int activationCode)
        {
            RegisterViewModel activationData = new RegisterViewModel()
            {
                Email = username,
                VerificationCode = activationCode
            };

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("yourapiipaddress");
            string activationInput = JsonConvert.SerializeObject(activationData);
            StringContent ActivationContent = new StringContent(activationInput, Encoding.UTF8, "application/json");
            HttpResponseMessage verification = await client.PutAsync("/api/login/activateaccount", ActivationContent);
            string verificationReply = await verification.Content.ReadAsStringAsync();
            bool verificationSuccess = JsonConvert.DeserializeObject<bool>(verificationReply);

            //if user input for verification code is valid
            if (verificationSuccess)
            {
                TempData["success"] = "Tilin aktivointi onnistui! Kirjaudu sisään tunnuksillasi.";
                return RedirectToAction("Index", "Account");

            }
            else
            {
                RegisterViewModel emailData = new RegisterViewModel()
                {
                    Email = username
                };
                TempData["error"] = "Tilin aktivointi epäonnistui, tarkasta syöttämäsi tiedot!";
                return RedirectToAction("Activation", "Account", emailData);
            }
        }

        //*************************************************************************
        //LOGIN
        //*************************************************************************

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (username != null && password != null)
            {
                Login logindata = new Login()
                {
                    PassWord = password,
                    Email = username,
                    UserName = username

                };

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");

                string input = JsonConvert.SerializeObject(logindata);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PostAsync("/api/login", content);
                string reply = await message.Content.ReadAsStringAsync();
                bool success = JsonConvert.DeserializeObject<bool>(reply);


                if (success)
                {
                    try
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
                                UserName = username,
                                Email = username

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
                                    string inputId = JsonConvert.SerializeObject(logindata);
                                    StringContent contentId = new StringContent(inputId, Encoding.UTF8, "application/json");
                                    HttpResponseMessage fetchId = await client.PostAsync("/api/login/getuserid", contentId);
                                    string replyId = await fetchId.Content.ReadAsStringAsync();

                                    //let's use static variables around the application like this
                                    HttpContext.Session.SetInt32("loginid", Convert.ToInt32(replyId));
                                    HttpContext.Session.SetString("username", username);

                                    string inputAdmin = JsonConvert.SerializeObject(logindata);
                                    StringContent contentAdmin = new StringContent(inputAdmin, Encoding.UTF8, "application/json");
                                    HttpResponseMessage fetchAdmin = await client.PostAsync("/api/login/adminlogin", contentAdmin);
                                    string replyAdmin = await fetchAdmin.Content.ReadAsStringAsync();
                                    if (success)
                                    {
                                        HttpContext.Session.SetString("isAdmin", replyAdmin);
                                    }

                                    ViewBag.error = "";
                                    return RedirectToAction("Index", "Events");

                                }
                                catch (Exception ex)
                                {
                                    ViewBag.error = "Kirjautumisen aikana tapahtui odottamaton virhe. Ole hyvä ja yritä uudelleen. Vian jatkuessa ole yhteydessä tukeen. Virhetiedot: " + ex;
                                    return RedirectToAction("Index", "Account");
                                }
                            }

                            //if account has not been activated
                            else
                            {
                                RegisterViewModel emailData = new RegisterViewModel()
                                {
                                    Email = username
                                };
                                return RedirectToAction("Activation", "Account", emailData);
                            }
                        }
                        else
                        {
                            ViewBag.error = "Ylläpitäjä ei ole vielä hyväksynyt käyttäjää " + username + ". Ole hyvä ja odota, tai ota yhteys ylläpitoon yourapplicationgmailaccount@gmail.com";
                            return View("Index");
                        }

                    }
                    catch
                    {
                        ViewBag.error = "Käyttäjätilin aktivointia todentaessa tapahtui virhe. Virheen toistuessa ota yhteyttä ylläpitoon yourapplicationgmailaccount@gmail.com.";
                        return View("Index");
                    }
                }
                else
                {
                    ViewBag.error = "Sähköpostiosoite tai salasana väärin, tarkasta syöttämäsi tiedot";
                    return View("Index");
                }
            }
            else
            {
                ViewBag.error = "Tarkasta syöttämäsi tiedot";
                return View("Index");
            }
        }

        [Route("logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            HttpContext.Session.Remove("loginid");
            HttpContext.Session.Remove("isAdmin");
            return RedirectToAction("Index");
        }


        //*************************************************************************
        //REGISTER
        //*************************************************************************

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel reg, string emailveri, string passwordveri)
        {
            if (ModelState.IsValid)
            {
                if (reg.Email != emailveri)
                {
                    ViewBag.emailError = "Sähköpostiosoitteet eivät täsmää, ole hyvä ja tarkasta osoitteet";
                    return View("Register", reg);
                }
                if (reg.PassWord != passwordveri)
                {
                    ViewBag.passwordError = "Salasanat eivät täsmää, ole hyvä ja kirjoita haluttu salasana uudelleen";
                    return View("Register", reg);
                }

                else
                {

                    //CHECK IF EMAIL EXISTS
                    RegisterViewModel checkEmail = new RegisterViewModel()
                    {
                        Email = reg.Email
                    };

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("yourapiipaddress");
                    string inputEmail = JsonConvert.SerializeObject(checkEmail);
                    StringContent contentEmail = new StringContent(inputEmail, Encoding.UTF8, "application/json");
                    HttpResponseMessage messageEmail = await client.PostAsync("/api/login/checkemail", contentEmail);
                    string replyEmail = await messageEmail.Content.ReadAsStringAsync();
                    bool successEmail = JsonConvert.DeserializeObject<bool>(replyEmail);

                    if (successEmail)
                    {
                        //https://stackoverflow.com/questions/33749543/unique-4-digit-random-number-in-c-sharp
                        //Generates random number for 4-digit activation code
                        int _min = 1000;
                        int _max = 9999;
                        Random _rdm = new Random();
                        int veriCode = _rdm.Next(_min, _max);


                        RegisterViewModel newUser = new RegisterViewModel()
                        {
                            FirstName = reg.FirstName,
                            LastName = reg.LastName,
                            Address = reg.Address,
                            PostalCode = reg.PostalCode,
                            City = reg.City.ToUpper(),
                            Phone = reg.Phone.ToString(),
                            Email = reg.Email,
                            PassWord = reg.PassWord,
                            VerificationCode = veriCode,
                            Confirmed = false

                        };

                        string input = JsonConvert.SerializeObject(newUser);
                        StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                        HttpResponseMessage message = await client.PostAsync("/api/login/register", content);
                        string reply = await message.Content.ReadAsStringAsync();
                        bool success = JsonConvert.DeserializeObject<bool>(reply);


                        if (success)
                        {
                            //*********************
                            //VERIFICATION EMAIL
                            //*********************
                            try
                            {
                                MailMessage mail = new MailMessage();
                                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                                mail.From = new MailAddress("yourapplicationgmailaccount@gmail.com");
                                mail.To.Add(reg.Email/*"yourapplicationgmailaccount@gmail.com"*/);
                                mail.Subject = "PursiX tilin aktivointi";
                                mail.Body =
                                    "Ohjelmiston ylläpito aktivoi tilisi mahdollisimman nopeasti, jonka jälkeen ohjelmisto on käytettävissänne. \n\n\n\n" +
                                    "Aktivointikoodisi on: " + veriCode.ToString() + ".\n\n" + //when user has paid the application fee, user could get the activation code from that way too, but until implemented into use we will use this.
                                    "Aktivointikoodi tulee syöttää ensimmäisellä kirjautumiskerralla PursiX -ohjelmistoon.\n\n" +
                                    "PursiX -ohjelmistoon tulee kirjautua sähköpostiosoitteella " + reg.Email + " ja syöttämällä tallentamasi salasana.\n\n\n\n" +
                                    "Vikatilanteissa ota yhteys ylläpitoon sähköpostiosoitteeseen yourapplicationgmailaccount@gmail.com";

                                SmtpServer.UseDefaultCredentials = false;
                                SmtpServer.Port = 587;
                                SmtpServer.Host = "smtp.gmail.com";
                                SmtpServer.Credentials = new System.Net.NetworkCredential("yourgmailaccount", "yourgmailpassword");
                                SmtpServer.EnableSsl = true;

                                SmtpServer.Send(mail);

                            }


                            catch (Exception ex)
                            {
                                ViewBag.error = "Virhe: " + ex.Message + ". Virhe lähetettäessä aktivointisähköpostia";
                                return View("Register", reg);
                            }

                            ViewBag.success = "Rekisteröintitiedot tallennettu onnistuneesti! Sähköpostivarmistus lähetetty osoitteeseen: " + reg.Email + " seuraa sähköpostissa olevia ohjeita aktivoidaksesi tunnuksesi.";
                            return View("Index");
                        }


                        else
                        {
                            ViewBag.error = "Tietojen vienti epäonnistui, ole hyvä ja yritä uudelleen.";
                            return View("Index");
                        }
                    }
                    else
                    {

                        ViewBag.emailError = "Sähköpostiosoite on jo käytössä, syötä toinen sähköpostiosoite tai ota yhteys tukeen yourapplicationgmailaccount@gmail.com";
                        return View("Register", reg);
                    }

                }

            }
            else
            {
                return View("Register", reg);
            }


        }

        //*************************************************************************
        //ADMIN REGISTER
        //*************************************************************************

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminRegister(RegisterViewModel reg, string emailveri, string passwordveri)
        {
            if (ModelState.IsValid)
            {
                if (reg.Email != emailveri)
                {
                    ViewBag.emailError = "Sähköpostiosoitteet eivät täsmää, ole hyvä ja tarkasta osoitteet";
                    return RedirectToAction("Create","Logins", reg);
                }
                if (reg.PassWord != passwordveri)
                {
                    ViewBag.passwordError = "Salasanat eivät täsmää, ole hyvä ja kirjoita haluttu salasana uudelleen";
                    return RedirectToAction("Create", "Logins", reg);
                }

                else
                {

                    //CHECK IF EMAIL EXISTS
                    RegisterViewModel checkEmail = new RegisterViewModel()
                    {
                        Email = reg.Email
                    };

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("yourapiipaddress");
                    string inputEmail = JsonConvert.SerializeObject(checkEmail);
                    StringContent contentEmail = new StringContent(inputEmail, Encoding.UTF8, "application/json");
                    HttpResponseMessage messageEmail = await client.PostAsync("/api/login/checkemail", contentEmail);
                    string replyEmail = await messageEmail.Content.ReadAsStringAsync();
                    bool successEmail = JsonConvert.DeserializeObject<bool>(replyEmail);

                    if (successEmail)
                    {

                        RegisterViewModel newUser = new RegisterViewModel()
                        {
                            FirstName = reg.FirstName,
                            LastName = reg.LastName,
                            Address = reg.Address,
                            PostalCode = reg.PostalCode,
                            City = reg.City.ToUpper(),
                            Phone = reg.Phone.ToString(),
                            Email = reg.Email,
                            PassWord = reg.PassWord,
                            VerificationCode = 1234,
                            Confirmed = reg.Confirmed,
                            EmailConfirmed = reg.EmailConfirmed,
                            Admin = reg.Admin,
                            AdminLogged = true

                        };

                        string input = JsonConvert.SerializeObject(newUser);
                        StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                        HttpResponseMessage message = await client.PostAsync("/api/login/adminadduser", content);
                        string reply = await message.Content.ReadAsStringAsync();
                        bool success = JsonConvert.DeserializeObject<bool>(reply);


                        if (success)
                        {
                            //*********************
                            //VERIFICATION EMAIL
                            //*********************
                            try
                            {
                                MailMessage mail = new MailMessage();
                                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                                mail.From = new MailAddress("yourapplicationgmailaccount@gmail.com");
                                mail.To.Add(/*input_emailAddress.Text*/"yourapplicationgmailaccount@gmail.com");
                                mail.Subject = "PursiX tilin aktivointi";
                                mail.Body =
                                    "Ohjelmiston ylläpito aktivoi tilisi mahdollisimman nopeasti, jonka jälkeen ohjelmisto on käytettävissänne. \n\n\n\n" +
                                    "Aktivointikoodisi on: 1234 \n\n" + //when user has paid the application fee, user could get the activation code from that way too, but until implemented into use we will use this.
                                    "Aktivointikoodi tulee syöttää ensimmäisellä kirjautumiskerralla PursiX -ohjelmistoon.\n\n" +
                                    "PursiX -ohjelmistoon tulee kirjautua sähköpostiosoitteella " + reg.Email + " ja syöttämällä tallentamasi salasana.\n\n\n\n" +
                                    "Vikatilanteissa ota yhteys ylläpitoon sähköpostiosoitteeseen yourapplicationgmailaccount@gmail.com";

                                SmtpServer.UseDefaultCredentials = false;
                                SmtpServer.Port = 587;
                                SmtpServer.Host = "smtp.gmail.com";
                                SmtpServer.Credentials = new System.Net.NetworkCredential("yourgmailaccount", "yourgmailpassword");
                                SmtpServer.EnableSsl = true;

                                SmtpServer.Send(mail);
                            }
                            catch (Exception ex)
                            {
                                TempData["error"] = "Virhe: " + ex.Message + ". Virhe lähetettäessä aktivointisähköpostia";
                                return RedirectToAction("Create", "Logins", reg);
                            }

                            TempData["success"] = "Rekisteröintitiedot tallennettu onnistuneesti!";
                            return RedirectToAction("Index", "Logins", reg);
                        }


                        else
                        {
                            TempData["error"] = "Tietojen vienti epäonnistui, ole hyvä ja yritä uudelleen.";
                            return RedirectToAction("Create", "Logins", reg);
                        }
                    }
                    else
                    {

                        TempData["error"] = "Sähköpostiosoite on jo käytössä, syötä toinen sähköpostiosoite tai ota yhteys tukeen yourapplicationgmailaccount@gmail.com";
                        return RedirectToAction("Create", "Logins", reg);
                    }

                }

            }
            else
            {
                TempData["error"] = "Täytä kentät huolellisesti.";
                return RedirectToAction("Create", "Logins", reg);
            }


        }




        //*************************************************************************
        //EDIT USER
        //*************************************************************************
        public async Task<IActionResult> EditUserInfo(int loginid)
        {

            try
            {
                EditUserInfoModel getUserInfo = new EditUserInfoModel
                {
                    LoginId = loginid,
                    isLogged = true
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

                return View(userInfoData);

            }
            catch
            {
                return RedirectToAction("Index", "Events");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserInfo(int loginid, [Bind("LoginId,Email,FirstName,LastName,Address,PostalCode,City,Phone,isLogged")] EditUserInfoModel userInfo)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(HttpContext.Session.GetString("username")))
                {
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    EditUserInfoModel updateUser = new EditUserInfoModel()
                    {
                        LoginId = userInfo.LoginId,
                        Email = userInfo.Email,
                        FirstName = userInfo.FirstName,
                        LastName = userInfo.LastName,
                        Address = userInfo.Address,
                        PostalCode = userInfo.PostalCode,
                        City = userInfo.City.ToUpper(),
                        Phone = userInfo.Phone.ToString(),
                        isLogged = true
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
                        TempData["success"] = "Yhteystiedot tallennettu onnistuneesti";
                        return RedirectToAction("Index", "Events");
                    }
                    else
                    {
                        TempData["error"] = "Yhteystietojen tallennuksessa tapahtui virhe, ole hyvä ja yritä uudelleen";
                        return RedirectToAction("Index", "Events");
                    }
                }
            }
            else
            {
                return View();
            }
        }



        //*************************************************************************
        //EDIT USER PASSWORD
        //*************************************************************************
        // GET: Events/Create
        public IActionResult EditUserPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserPassword(int loginid, string passwordveri, [Bind("LoginId,UserName,PassWord,Email,NewPassWord")] EditUserPasswordModel editPassword)
        {
            if (ModelState.IsValid)
            {
                if (editPassword.NewPassWord != passwordveri)
                {
                    ViewBag.passwordError = "Salasanat eivät täsmää, ole hyvä ja tarkasta salasanat";
                    return View(editPassword);
                }
                else
                {
                    try
                    {
                        EditUserPasswordModel changeUsrPwd = new EditUserPasswordModel
                        {
                            LoginId = editPassword.LoginId,
                            UserName = editPassword.UserName,
                            PassWord = editPassword.PassWord,
                            Email = editPassword.Email,
                            NewPassWord = editPassword.NewPassWord
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
                            TempData["success"] = "Salasana vaihdettu onnistuneesti!";
                            return RedirectToAction("Index", "Events");
                        }
                        else
                        {
                            TempData["error"] = "Salasanan tallennuksessa tapahtui virhe, ole hyvä ja yritä uudelleen";
                            return RedirectToAction("Index", "Events");
                        }

                    }
                    catch
                    {
                        TempData["error"] = "Salasanan tallennuksessa tapahtui virhe, ole hyvä ja yritä uudelleen";
                        return RedirectToAction("Index", "Events");
                    }
                }
            }
            else
            {
                return View();
            }
        }




        //*************************************************************************
        //ADMIN VIEW USERINFO
        //*************************************************************************
        public async Task<IActionResult> ViewUserInfo(int loginid)
        {

            try
            {
                EditUserInfoModel getUserInfo = new EditUserInfoModel
                {
                    LoginId = loginid,
                    isLogged = true
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

                return View(userInfoData);

            }
            catch
            {
                return RedirectToAction("Index", "Events");
            }

        }



    }
}
